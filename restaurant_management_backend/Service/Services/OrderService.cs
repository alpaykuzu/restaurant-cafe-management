using AutoMapper;
using Core.Dtos.OrderDtos;
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
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IGenericRepository<MenuItem> _menuItemRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Table> _tableRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<RestaurantHub> _hubContext;
        public OrderService(IGenericRepository<Order> orderRepository, IGenericRepository<OrderItem> orderItemRepository, IMapper mapper, IGenericRepository<MenuItem> menuItemRepository, IGenericRepository<Employee> employeeRepository, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext, IGenericRepository<Table> tableRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
            _menuItemRepository = menuItemRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _tableRepository = tableRepository;
        }
        public async Task<ApiResponse<NoContent>> CreateOrderAsync(CreateOrderFullRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var employee = await _employeeRepository.GetFirstOrDefaultAsync(e => e.UserId == userId);
            if(employee == null)
            {
                return ApiResponse<NoContent>.Fail("Çalışan bulunamadı.");
            }
            decimal totalAmount = 0;
            foreach (var orderItem in req.OrderItems)
            {
                    var menuItem = await _menuItemRepository.GetByIdAsync(orderItem.MenuItemId);
                    if (menuItem == null)
                    {
                        return ApiResponse<NoContent>.Fail("Menu bulunamadı.");
                    }
                    var price = menuItem.Price; 
                    var amount = price * orderItem.Quantity;
                    totalAmount += amount;
            }
            var order = _mapper.Map<Order>(req.Order);
            order.Status = "Pending";
            order.TotalAmount = totalAmount;
            order.EmployeeId = employee.Id;
            order.RestaurantId = employee.RestaurantId;
            await _orderRepository.AddAsync(order);
            foreach (var itemReq in req.OrderItems)
            {
                    var menuItem = await _menuItemRepository.GetByIdAsync(itemReq.MenuItemId);
                    var price = menuItem?.Price ?? 0;
                    var orderItem = _mapper.Map<OrderItem>(itemReq);
                    orderItem.OrderId = order.Id;
                    orderItem.UnitPrice = price;
                    await _orderItemRepository.AddAsync(orderItem);
            }
            await _hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("OrderChanged");
            return ApiResponse<NoContent>.Ok("Sipariş oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var order = await _orderRepository.Query().Include(t => t.Table).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return ApiResponse<NoContent>.Fail("Sipariş bulunamadı.");
            }
            if(order.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz işlem.");
            }
            if(newStatus == "Cancelled")
            {
                order.Table.Status = "Available";
                await _tableRepository.UpdateAsync(order.Table);
            }
            if (newStatus == "Cancelled")
            {
                order.Table.Status = "Available";
                await _tableRepository.UpdateAsync(order.Table);
            }
            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);
            await _hubContext.Clients.Group(order.RestaurantId.ToString()).SendAsync("OrderChanged");
            return ApiResponse<NoContent>.Ok("Sipariş durumu güncellendi.");
        }
        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var orders = await _orderRepository.Query()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == user.Employee.RestaurantId)
                .ToListAsync();

            if (!orders.Any())
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Sipariş bulunamadı.");
            }

            var orderDtos = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                RestaurantId = order.RestaurantId,
                EmployeeId = order.EmployeeId,
                TableId = order.TableId,
                TableNumber = order.Table.Number,
                Status = order.Status,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orderDtos);
        }

        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByDateAsync(DateTime startDate)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var orders = await _orderRepository.Query()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.RestaurantId == user.Employee.RestaurantId && o.OrderDate >= startDate && o.OrderDate < startDate.AddDays(1))
                .ToListAsync();

            if (!orders.Any())
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Sipariş bulunamadı.");
            }

            var orderDtos = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                RestaurantId = order.RestaurantId,
                EmployeeId = order.EmployeeId,
                TableId = order.TableId,
                TableNumber = order.Table.Number,
                Status = order.Status,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orderDtos);
        }

        public async Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<OrderResponseDto>.Fail("Kullanıcı bulunamadı.");
            }
            var order = await _orderRepository.Query()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem) 
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return ApiResponse<OrderResponseDto>.Fail("Sipariş bulunamadı.");
            }
            if(order.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<OrderResponseDto>.Fail("Yetkisiz işlem.");
            }
            var orderDto = _mapper.Map<OrderResponseDto>(order);
            orderDto.TableNumber = order.Table.Number;

            orderDto.OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
            {
                MenuItemName = oi.MenuItem.Name,   
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();

            return ApiResponse<OrderResponseDto>.Ok(orderDto);
        }
        public async Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(string status)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var orders = await _orderRepository.Query()
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == status && o.RestaurantId == user.Employee.RestaurantId)
                .ToListAsync();

            if (!orders.Any())
            {
                return ApiResponse<IEnumerable<OrderResponseDto>>.Fail("Sipariş bulunamadı.");
            }

            var orderDtos = orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                RestaurantId = order.RestaurantId,
                TableId = order.TableId,
                TableNumber = order.Table.Number,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orderDtos);
        }

    }
}
