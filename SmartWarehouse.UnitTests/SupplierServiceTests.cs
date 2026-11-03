using Moq;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.DTOs.Supplier;
using SmartWarehouse.Application.Exceptions;
using SmartWarehouse.Application.Service;
using SmartWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartWarehouse.UnitTests
{
    public class SupplierServiceTests
    {
        private readonly Mock<ISupplierRepository> _mockRepo;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _mockRepo = new Mock<ISupplierRepository>();
            _service = new SupplierService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsSupplierDto()
        {
            // Arrange
            var dto = new CreateSupplierDto
            {
                SupplierCode = "SUP-VIN01",
                Name = "Vinamilk Logistics",
                TaxCode = "0100109106",
                ContactEmail = "contact@vinamilk.com.vn",
                ContactPhone = "+84901234567"
            };

            _mockRepo.Setup(r => r.CodeExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SUP-VIN01", result.SupplierCode);
            Assert.Equal("contact@vinamilk.com.vn", result.ContactEmail);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateCode_ThrowsDuplicateWarehouseCodeException()
        {
            // Arrange
            var dto = new CreateSupplierDto
            {
                SupplierCode = "SUP-EXIST",
                Name = "Existing Supplier",
                ContactEmail = "test@exist.com"
            };

            _mockRepo.Setup(r => r.CodeExistsAsync("SUP-EXIST", null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateWarehouseCodeException>(() => _service.CreateAsync(dto));
        }

        [Theory]
        [InlineData("123")] // Too short
        [InlineData("123456789012")] // Too long
        [InlineData("ABCDEFGHIJ")] // Letters instead of digits
        public async Task CreateAsync_WithInvalidTaxCode_ThrowsArgumentException(string invalidTaxCode)
        {
            // Arrange
            var dto = new CreateSupplierDto
            {
                SupplierCode = "SUP-TAX01",
                Name = "Tax Test Supplier",
                TaxCode = invalidTaxCode,
                ContactEmail = "tax@test.com"
            };

            _mockRepo.Setup(r => r.CodeExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }
    }
}
