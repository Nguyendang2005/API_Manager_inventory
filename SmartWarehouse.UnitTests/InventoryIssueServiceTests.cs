using Moq;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.DTOs.InventoryIssue;
using SmartWarehouse.Application.Service;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartWarehouse.UnitTests
{
    public class InventoryIssueServiceTests
    {
        private readonly Mock<IInventoryIssueRepository> _mockIssueRepo;
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly InventoryIssueService _service;

        public InventoryIssueServiceTests()
        {
            _mockIssueRepo = new Mock<IInventoryIssueRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockUow = new Mock<IUnitOfWork>();

            _service = new InventoryIssueService(
                _mockIssueRepo.Object,
                _mockProductRepo.Object,
                _mockUow.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenStockIsInsufficient_ThrowsInsufficientStockException_AndDoesNotCommit()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                SKU = "SKU-MILK-01",
                StockQuantity = 10,
                UnitPrice = 100
            };

            _mockProductRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(product);

            var dto = new CreateInventoryIssueDto
            {
                WarehouseId = 1,
                IssueDate = DateTime.UtcNow,
                Details = new List<CreateIssueDetailDto>
                {
                    new CreateIssueDetailDto { ProductId = 1, Quantity = 25 } // Demands 25, only 10 in stock
                }
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InsufficientStockException>(() => _service.CreateAsync(dto));
            Assert.Equal("SKU-MILK-01", ex.SKU);
            Assert.Equal(10, ex.Available);
            Assert.Equal(25, ex.Requested);

            // Verify no commit called
            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenStockIsSufficient_DeductsStock_AndCommitsAtomically()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                SKU = "SKU-BEER-01",
                StockQuantity = 50,
                UnitPrice = 200
            };

            _mockProductRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(product);

            var createdIssue = new InventoryIssue
            {
                Id = 10,
                IssueCode = "ISS-20261030-001",
                WarehouseId = 1,
                Status = IssueStatus.Draft,
                Details = new List<InventoryIssueDetail>
                {
                    new InventoryIssueDetail { ProductId = 1, Quantity = 20, UnitPrice = 200, Product = product }
                }
            };

            _mockIssueRepo.Setup(r => r.AddAsync(It.IsAny<InventoryIssue>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);

            _mockIssueRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(createdIssue);

            _mockUow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

            var dto = new CreateInventoryIssueDto
            {
                WarehouseId = 1,
                IssueDate = DateTime.UtcNow,
                Details = new List<CreateIssueDetailDto>
                {
                    new CreateIssueDetailDto { ProductId = 1, Quantity = 20 }
                }
            };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(30, product.StockQuantity); // 50 - 20 = 30 deducted in memory
            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
