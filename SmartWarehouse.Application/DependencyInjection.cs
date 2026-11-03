using Microsoft.Extensions.DependencyInjection;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.Service;

namespace SmartWarehouse.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryReceiptService, InventoryReceiptService>();
            services.AddScoped<IInventoryIssueService, InventoryIssueService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}