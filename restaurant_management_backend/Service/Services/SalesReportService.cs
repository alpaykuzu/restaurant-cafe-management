using AutoMapper;
using Core.Dtos.RoleDtos;
using Core.Dtos.SalesReportDtos;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;

        public SalesReportService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<Payment> paymentRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<User> userRepository)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<SalesReportResponseDto>> GenerateReportAsync(CreateSalesReportRequestDto request)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<SalesReportResponseDto>.Fail("Kullanıcı bulunamadı.");
            var orders = await _orderRepository.FindAsync(o =>
                o.RestaurantId == user.Employee.RestaurantId &&
                o.OrderDate >= request.StartDate &&
                o.OrderDate <= request.EndDate);

            if (orders == null || !orders.Any())
            {
                return ApiResponse<SalesReportResponseDto>.Fail("Bu tarih aralığında sipariş bulunamadı.");
            }

            var totalOrders = orders.Count();
            var totalSales = orders.Sum(o => o.TotalAmount);
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;

            var report = new SalesReportResponseDto
            {
                ReportDate = DateTime.UtcNow,
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue
            };

            return ApiResponse<SalesReportResponseDto>.Ok(report, "Satış raporu oluşturuldu.");
        }
    }
}
