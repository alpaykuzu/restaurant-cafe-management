using Core.Dtos.InvoiceDtos;
using Core.Extensions;
using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Service.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenericRepository<Invoice> _invoiceRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHubContext<RestaurantHub> _hubContext;
        public InvoiceService(IGenericRepository<Invoice> invoiceRepository, IGenericRepository<Order> orderRepository, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(int orderId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<InvoiceResponseDto>.Fail("Kullanıcı bulunamadı.");
            }
            var order = await _orderRepository.Query().Include(o => o.OrderItems).ThenInclude(i => i.MenuItem).FirstOrDefaultAsync(o => o.Id == orderId && o.RestaurantId == user.Employee.RestaurantId);
            if (order == null)
                return ApiResponse<InvoiceResponseDto>.Fail("Sipariş bulunamadı.");

            // OrderItem → InvoiceItem dönüştür
            var invoiceItems = order.OrderItems.Select(oi => new InvoiceItem
            {
                ItemName = oi.MenuItem.Name,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
            }).ToList();

            var totalAmount = invoiceItems.Sum(i => i.LineTotal);

            var invoice = new Invoice
            {
                OrderId = orderId,
                IssuedAt = DateTime.Now,
                TotalAmount = totalAmount,
                Items = invoiceItems
            };

            await _invoiceRepository.AddAsync(invoice);

            var dto = new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderNumber = order.OrderNumber,
                IssuedAt = invoice.IssuedAt,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ItemName = i.ItemName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                })
            };
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("CreateInvoice");
            return ApiResponse<InvoiceResponseDto>.Ok(dto);
        }

        public async Task<ApiResponse<InvoiceResponseDto>> GetInvoiceByIdAsync(int invoiceId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<InvoiceResponseDto>.Fail("Kullanıcı bulunamadı.");
            }
            var invoice = await _invoiceRepository.Query().Include(i => i.Items).Include(o => o.Order).FirstOrDefaultAsync(i => i.Id == invoiceId && i.Order.RestaurantId == user.Employee.RestaurantId);
            if (invoice == null)
                return ApiResponse<InvoiceResponseDto>.Fail("Fatura bulunamadı.");

            var order = await _orderRepository.GetByIdAsync(invoice.OrderId);

            var dto = new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderNumber = order.OrderNumber,
                IssuedAt = invoice.IssuedAt,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ItemName = i.ItemName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                })
            };

            return ApiResponse<InvoiceResponseDto>.Ok(dto);
        }

        public async Task<ApiResponse<IEnumerable<InvoiceResponseDto>>> GetInvoicesByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            var user = await _userRepository.Query()
                .Include(u => u.Employee)
                .ThenInclude(e => e.Restaurant)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Employee?.RestaurantId == null)
                return ApiResponse<IEnumerable<InvoiceResponseDto>>.Fail("Kullanıcı veya restoran bulunamadı.");

            var invoices = await _invoiceRepository.Query()
                .Include(i => i.Items)
                .Include(i => i.Order) 
                .Where(i => i.Order.RestaurantId == user.Employee.RestaurantId)
                .ToListAsync();

            var dtos = invoices.Select(invoice => new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderNumber = invoice.Order.OrderNumber,
                IssuedAt = invoice.IssuedAt,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ItemName = i.ItemName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            }).ToList();

            return ApiResponse<IEnumerable<InvoiceResponseDto>>.Ok(dtos);
        }

        public async Task<ApiResponse<IEnumerable<InvoiceResponseDto>>> GetInvoicesByDateAsync(DateTime startDate)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();

            var user = await _userRepository.Query()
                .Include(u => u.Employee)
                .ThenInclude(e => e.Restaurant)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Employee?.RestaurantId == null)
                return ApiResponse<IEnumerable<InvoiceResponseDto>>.Fail("Kullanıcı veya restoran bulunamadı.");

            var invoices = await _invoiceRepository.Query()
                .Include(i => i.Items)
                .Include(i => i.Order).ThenInclude(p => p.Payment)
                .Where(i => i.Order.RestaurantId == user.Employee.RestaurantId && i.Order.Payment.PaymentDate >= startDate && i.Order.Payment.PaymentDate < startDate.AddDays(1))
                .ToListAsync();

            var dtos = invoices.Select(invoice => new InvoiceResponseDto
            {
                Id = invoice.Id,
                OrderNumber = invoice.Order.OrderNumber,
                IssuedAt = invoice.IssuedAt,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    ItemName = i.ItemName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            }).ToList();

            return ApiResponse<IEnumerable<InvoiceResponseDto>>.Ok(dtos);
        }
    }
}
