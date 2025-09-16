using Api.Filters;
using Api.Middleware;
using AutoMapper;
using Core.Dtos.AuthDtos;
using Core.Dtos.MenuItemDtos;
using Core.Dtos.RestaurantDtos;
using Core.Dtos.RoleDtos;
using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Validators.Auth;
using Core.Validators.Category;
using Core.Validators.Employee;
using Core.Validators.MenuItem;
using Core.Validators.Order;
using Core.Validators.Payment;
using Core.Validators.Restaurant;
using Core.Validators.Role;
using Core.Validators.SalesReport;
using Core.Validators.Table;
using Data;
using Data.Configurations;
using Data.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//CORS ayarlarý
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

//JWT Authentication ayarlarý
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

//DbContext ayarlarý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant Management API", Version = "v1" });

    // JWT Authentication için taným
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Dependency Injection
builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();
builder.Services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();
builder.Services.AddScoped<IUserService, UserService>();

//Validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEmployeeValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMenuItemValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMenuItemValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderItemValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateRestaurantValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRestaurantValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSalesReportValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTableValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTableValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTableStatusValidator>();

//HttpContextAccessor
builder.Services.AddHttpContextAccessor();

//SignalR
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<IGenericRepository<User>>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

    var adminEmail = "admin@admin.com";
    var adminUser = await service.GetFirstOrDefaultAsync(u => u.Email == adminEmail);

    if (adminUser == null)
    {
        await authService.RegisterAsync(new RegisterRequestDto
        {
            FirstName = "admin",
            LastName = "admin",
            Email = adminEmail,
            Password = "123456",
        });
        var admin = await service.GetFirstOrDefaultAsync(u => u.Email == adminEmail);
        await roleService.CreateRoleAsync(new CreateRoleRequestDto
        {
            UserId = admin.Id,
            Name = "Admin"
        });
    }
}

//SignalR Hub route
app.MapHub<RestaurantHub>("/hubs");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors();

app.UseMiddleware<CustomExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
