using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Infrastructure.Data;
using SmartWarehouse.Infrastructure.Repositories;

namespace SmartWarehouse.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // ── Generic Repository & Unit of Work (ASM-U05) ──
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Specialized Repositories ──
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInventoryReceiptRepository, InventoryReceiptRepository>();
            services.AddScoped<IInventoryIssueRepository, InventoryIssueRepository>();

            return services;
        }
    }
}