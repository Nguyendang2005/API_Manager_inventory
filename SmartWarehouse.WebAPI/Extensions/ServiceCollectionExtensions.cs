using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SmartWarehouse.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "SmartWarehouse_Secret_Key_For_JWT_Authentication_2026_Secure_Token!";
            var issuer = jwtSettings["Issuer"] ?? "SmartWarehouse.API";
            var audience = jwtSettings["Audience"] ?? "SmartWarehouse.Client";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("WarehouseStaff", policy => policy.RequireRole("WarehouseStaff", "WarehouseManager"));
                options.AddPolicy("WarehouseManager", policy => policy.RequireRole("WarehouseManager"));
            });

            return services;
        }
    }
}
