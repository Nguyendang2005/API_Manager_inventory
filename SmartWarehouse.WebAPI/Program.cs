using SmartWarehouse.Application;
using SmartWarehouse.Infrastructure;
using SmartWarehouse.WebAPI.Extensions;
using SmartWarehouse.WebAPI.Middlewares;

namespace SmartWarehouse.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Đăng ký Infrastructure (DbContext, Repositories, Unit of Work)
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // 2. Đăng ký Application Services (Use cases & Business services)
            builder.Services.AddApplicationServices();

            // 3. Đăng ký Security (JWT Bearer & Role-Based Policies - ASM-U06)
            builder.Services.AddApiSecurity(builder.Configuration);

            // 4. Controllers & JSON options
            builder.Services.AddControllers();

            // 5. Swagger/OpenAPI with Bearer Authentication modal & XML Comments (ASM-U06)
            builder.Services.AddSwaggerWithAuth();

            var app = builder.Build();

            // Middlewares pipeline theo Clean Architecture
            app.UseMiddleware<RequestCorrelationMiddleware>(); // X-Correlation-ID tracing
            app.UseMiddleware<GlobalExceptionMiddleware>();     // RFC 7807 ProblemDetails handling

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Warehouse API v1");
                    // Route chuẩn: https://localhost:7090/swagger hoặc /swagger/index.html
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            // Bắt buộc UseAuthentication trước UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
