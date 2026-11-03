using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<AuthResponseDto?> AuthenticateAsync(LoginDto dto)
        {
            // Demo authentication for Assignment 05
            // warehousemanager/manager123 -> WarehouseManager
            // warehousestaff/staff123 -> WarehouseStaff
            string? role = null;

            if (dto.Username.Equals("manager", StringComparison.OrdinalIgnoreCase) && dto.Password == "Manager123!")
            {
                role = "WarehouseManager";
            }
            else if (dto.Username.Equals("staff", StringComparison.OrdinalIgnoreCase) && dto.Password == "Staff123!")
            {
                role = "WarehouseStaff";
            }

            if (role is null)
            {
                return Task.FromResult<AuthResponseDto?>(null);
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"] ?? "SmartWarehouse_Secret_Key_For_JWT_Authentication_2026_Secure_Token!";
            var issuer = jwtSettings["Issuer"] ?? "SmartWarehouse.API";
            var audience = jwtSettings["Audience"] ?? "SmartWarehouse.Client";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(2);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            var response = new AuthResponseDto
            {
                Token = tokenString,
                Username = dto.Username,
                Role = role,
                ExpiresAt = expires
            };

            return Task.FromResult<AuthResponseDto?>(response);
        }
    }
}
