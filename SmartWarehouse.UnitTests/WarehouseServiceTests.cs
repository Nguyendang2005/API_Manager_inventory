using Moq;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.DTOs.Warehouse;
using SmartWarehouse.Application.Exceptions;
using SmartWarehouse.Application.Service;
using SmartWarehouse.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartWarehouse.UnitTests
{
    public class WarehouseServiceTests
    {
        private readonly Mock<IWarehouseRepository> _mockRepo;
        private readonly WarehouseService _service;

        public WarehouseServiceTests()
        {
            _mockRepo = new Mock<IWarehouseRepository>();
            _service = new WarehouseService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsWarehouseDto()
        {
            // Arrange
            var dto = new CreateWarehouseDto
            {
                Code = "WH-HN-01",
                Name = "Hanoi Central Warehouse",
                Address = "123 Hanoi Road"
            };

            _mockRepo.Setup(r => r.CodeExistsAsync("WH-HN-01", null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WH-HN-01", result.Code);
            Assert.Equal("Hanoi Central Warehouse", result.Name);
            Assert.True(result.IsActive);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateCode_ThrowsDuplicateWarehouseCodeException()
        {
            // Arrange
            var dto = new CreateWarehouseDto
            {
                Code = "WH-HN-01",
                Name = "Hanoi Warehouse"
            };

            _mockRepo.Setup(r => r.CodeExistsAsync("WH-HN-01", null, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateWarehouseCodeException>(() => _service.CreateAsync(dto));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Warehouse>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("", "Valid Name")]
        [InlineData("WH-01", "")]
        [InlineData("   ", "Valid Name")]
        public async Task CreateAsync_WithEmptyCodeOrName_ThrowsArgumentException(string code, string name)
        {
            // Arrange
            var dto = new CreateWarehouseDto
            {
                Code = code,
                Name = name
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFoundOrInactive_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Warehouse?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}
