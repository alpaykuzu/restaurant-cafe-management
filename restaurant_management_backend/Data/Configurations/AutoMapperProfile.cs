using AutoMapper;
using Core.Dtos.AuthDtos;
using Core.Dtos.CategoryDtos;
using Core.Dtos.EmployeeDtos;
using Core.Dtos.InventoryItemDtos;
using Core.Dtos.InventoryTransactionDtos;
using Core.Dtos.InvoiceDtos;
using Core.Dtos.MenuItemDtos;
using Core.Dtos.OrderDtos;
using Core.Dtos.PaymentDtos;
using Core.Dtos.ReservationDtos;
using Core.Dtos.RestaurantDtos;
using Core.Dtos.RoleDtos;
using Core.Dtos.SalesReportDtos;
using Core.Dtos.TableDtos;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configurations
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Auth
            CreateMap<User, UserResponseDto>();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    dest.PasswordHash = context.Items["PasswordHash"] as string ?? "";
                });

            //Employee
            CreateMap<Employee, EmployeeResponseDto>();
            CreateMap<UpdateEmployeeRequestDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CreateEmployeeRequestDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            //Restaurant
            CreateMap<Restaurant, RestaurantResponseDto>();
            CreateMap<UpdateRestaurantRequestDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CreateRestaurantRequestDto, Restaurant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Table
            CreateMap<Table, TableResponseDto>();
            CreateMap<UpdateTableRequestDto, Table>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CreateTableRequestDto, Table>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            //Category
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<UpdateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CreateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            //Role
            CreateMap<Role, RoleResponseDto>();
            CreateMap<UpdateRoleRequestDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CreateRoleRequestDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //InventoryItem
            CreateMap<InventoryItem, InventoryItemResponseDto>();
            CreateMap<UpdateInventoryItemRequestDto, InventoryItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CreateInventoryItemRequestDto, InventoryItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //InventoryTransaction
            CreateMap<InventoryTransaction, InventoryItemTransactionResponseDto>();
            CreateMap<CreateInventoryTransactionRequestDto, InventoryTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //MenuItem
            CreateMap<MenuItem, MenuItemResponsDto>();
            CreateMap<UpdateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CreateMenuItemRequestDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            //Order
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
            CreateMap<CreateOrderRequestDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => new Random().Next(100000, 999999)))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));

            //OrderItem
            CreateMap<CreateOrderItemRequestDto, OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Payment
            CreateMap<Payment, PaymentResponseDto>();

            //Reservation
            CreateMap<Reservation, ReservationResponseDto>();

            //User
            CreateMap<User, UserResponseDto>();
        }
    }
}
