using AutoMapper;
using Core.Dtos.MenuItemDtos;
using Core.Dtos.PaymentDtos;
using Core.Extensions;
using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHubContext<RestaurantHub> _hubContext;

        public PaymentService(IGenericRepository<Payment> paymentRepository, IGenericRepository<Order> orderRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<PaymentResponseDto>> MakePaymentAsync(CreatePaymentRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<PaymentResponseDto>.Fail("Kullanıcı bulunamadı.");
            var order = await _orderRepository.GetByIdAsync(req.OrderId);
            if (order == null)
                return ApiResponse<PaymentResponseDto>.Fail("Sipariş bulunamadı.");
            if(order.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<PaymentResponseDto>.Fail("Yetkisiz erişim.");
            if (order.TotalAmount <= 0)
                return ApiResponse<PaymentResponseDto>.Fail("Siparişin ödenecek bir tutarı yok.");

            // External ödeme entegrasyonu burada simüle edilebilir
            var status = "Success"; 

            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalAmount, 
                PaymentMethod = req.PaymentMethod,
                PaymentDate = DateTime.UtcNow,
                Status = status
            };

            await _paymentRepository.AddAsync(payment);

            order.Status = "Completed"; // Siparişi tamamlandı olarak işaretle
            await _orderRepository.UpdateAsync(order);

            var dto = _mapper.Map<PaymentResponseDto>(payment);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("MakePayment");
            return ApiResponse<PaymentResponseDto>.Ok(dto);
        }


        public async Task<ApiResponse<PaymentResponseDto>> GetPaymentByIdAsync(int paymentId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<PaymentResponseDto>.Fail("Kullanıcı bulunamadı.");
            var payment = await _paymentRepository.Query().Include(o => o.Order).FirstOrDefaultAsync(p => p.Id == paymentId);
            if (payment == null)
                return ApiResponse<PaymentResponseDto>.Fail("Ödeme bulunamadı.");
            if(payment.Order.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<PaymentResponseDto>.Fail("Yetkisiz erişim.");
            var dto = _mapper.Map<PaymentResponseDto>(payment);
            return ApiResponse<PaymentResponseDto>.Ok(dto);
        }

        public async Task<ApiResponse<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<PaymentResponseDto>.Fail("Kullanıcı bulunamadı.");
            var payment = await _paymentRepository.Query().Include(o => o.Order).FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (payment == null)
                return ApiResponse<PaymentResponseDto>.Fail("Ödeme bulunamadı.");
            if(payment.Order.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<PaymentResponseDto>.Fail("Yetkisiz erişim.");
            var dto = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentDate = payment.PaymentDate,
                Status = payment.Status
            };
            return ApiResponse<PaymentResponseDto>.Ok(dto);
        }

        public async Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<IEnumerable<PaymentResponseDto>>.Fail("Kullanıcı bulunamadı.");
            var payments = await _paymentRepository
                .FindAsync(p => p.Order.RestaurantId == user.Employee.RestaurantId);

            var dtos = payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                Status = p.Status
            });

            return ApiResponse<IEnumerable<PaymentResponseDto>>.Ok(dtos);
        }
    }
}
