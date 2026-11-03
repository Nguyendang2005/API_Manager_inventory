using Microsoft.Extensions.Configuration;
using Moq;
using SmartWarehouse.Application.DTOs.Auth;
using SmartWarehouse.Application.Service;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SmartWarehouse.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s["Key"]).Returns("SmartWarehouse_Secret_Key_For_JWT_Authentication_2026_Secure_Token!");
            mockSection.Setup(s => s["Issuer"]).Returns("SmartWarehouse.API");
            mockSection.Setup(s => s["Audience"]).Returns("SmartWarehouse.Client");
            _mockConfig.Setup(c => c.GetSection("Jwt")).Returns(mockSection.Object);

            _authService = new AuthService(_mockConfig.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_WithManagerCredentials_ReturnsTokenWithWarehouseManagerRole()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "manager",
                Password = "Manager123!"
            };

            // Act
            var response = await _authService.AuthenticateAsync(dto);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("WarehouseManager", response.Role);
            Assert.Equal("manager", response.Username);
            Assert.False(string.IsNullOrWhiteSpace(response.Token));

            // Validate claims inside the token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.Token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
            Assert.NotNull(roleClaim);
            Assert.Equal("WarehouseManager", roleClaim.Value);
        }

        [Fact]
        public async Task AuthenticateAsync_WithStaffCredentials_ReturnsTokenWithWarehouseStaffRole()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "staff",
                Password = "Staff123!"
            };

            // Act
            var response = await _authService.AuthenticateAsync(dto);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("WarehouseStaff", response.Role);
            Assert.Equal("staff", response.Username);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.Token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
            Assert.NotNull(roleClaim);
            Assert.Equal("WarehouseStaff", roleClaim.Value);
        }

        [Theory]
        [InlineData("wronguser", "Manager123!")]
        [InlineData("manager", "WrongPass")]
        [InlineData("", "")]
        public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsNull(string username, string password)
        {
            // Arrange
            var dto = new LoginDto { Username = username, Password = password };

            // Act
            var response = await _authService.AuthenticateAsync(dto);

            // Assert
            Assert.Null(response);
        }
    }
}
