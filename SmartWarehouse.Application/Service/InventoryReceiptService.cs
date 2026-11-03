using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.InventoryReceipt;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Service
{
    /// <summary>
    /// Business logic phiếu nhập kho.
    /// Khi Confirm → cộng StockQuantity cho từng Product.
    /// </summary>
    public class InventoryReceiptService : IInventoryReceiptService
    {
        private readonly IInventoryReceiptRepository _receiptRepo;
        private readonly IProductRepository          _productRepo;

        public InventoryReceiptService(
            IInventoryReceiptRepository receiptRepo,
            IProductRepository productRepo)
        {
            _receiptRepo = receiptRepo;
            _productRepo = productRepo;
        }

        public async Task<InventoryReceiptDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var receipt = await _receiptRepo.GetReceiptByIdAsync(id, ct);
            return receipt is null ? null : MapToDto(receipt);
        }

        public async Task<List<InventoryReceiptDto>> GetByWarehouseAsync(int warehouseId, CancellationToken ct = default)
        {
            var list = await _receiptRepo.GetByWarehouseAsync(warehouseId, ct);
            return list.Select(MapToDto).ToList();
        }

        public async Task<ReceiptMonthlySummaryDto?> GetMonthlyReceiptSummaryAsync(
            int warehouseId, int year, int month, CancellationToken ct = default)
        {
            return await _receiptRepo.GetMonthlyReceiptSummaryAsync(warehouseId, year, month, ct);
        }

        public async Task<InventoryReceiptDto> CreateAsync(CreateInventoryReceiptDto dto, CancellationToken ct = default)
        {
            string code = $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";

            var receipt = new InventoryReceipt
            {
                ReceiptCode  = code,
                WarehouseId  = dto.WarehouseId,
                SupplierId   = dto.SupplierId,
                ReceivedDate = dto.ReceivedDate,
                Status       = ReceiptStatus.Draft,
                Notes        = dto.Notes,
                CreatedAt    = DateTime.UtcNow
            };

            foreach (var d in dto.Details)
            {
                receipt.Details.Add(new InventoryReceiptDetail
                {
                    ProductId = d.ProductId,
                    Quantity  = d.Quantity,
                    UnitCost  = d.UnitCost
                });
            }

            await _receiptRepo.AddAsync(receipt, ct);
            await _receiptRepo.SaveChangesAsync(ct);

            var created = await _receiptRepo.GetReceiptByIdAsync(receipt.Id, ct);
            return MapToDto(created!);
        }

        public async Task<bool> ConfirmAsync(int id, CancellationToken ct = default)
        {
            var receipt = await _receiptRepo.GetReceiptByIdAsync(id, ct);
            if (receipt is null || receipt.Status != ReceiptStatus.Draft)
                return false;

            foreach (var detail in receipt.Details)
            {
                var product = await _productRepo.GetByIdAsync(detail.ProductId, ct);
                if (product is not null)
                {
                    product.StockQuantity += detail.Quantity;
                }
            }

            receipt.Status = ReceiptStatus.Confirmed;
            await _receiptRepo.SaveChangesAsync(ct);
            return true;
        }

        private static InventoryReceiptDto MapToDto(InventoryReceipt r) => new()
        {
            Id            = r.Id,
            ReceiptCode   = r.ReceiptCode,
            WarehouseId   = r.WarehouseId,
            WarehouseName = r.Warehouse?.Name ?? string.Empty,
            SupplierId    = r.SupplierId,
            SupplierName  = r.Supplier?.Name ?? string.Empty,
            ReceivedDate  = r.ReceivedDate,
            Status        = r.Status,
            Notes         = r.Notes,
            CreatedAt     = r.CreatedAt,
            Details       = r.Details.Select(d => new InventoryReceiptDetailDto
            {
                ProductId   = d.ProductId,
                ProductName = d.Product?.Name ?? string.Empty,
                SKU         = d.Product?.SKU ?? string.Empty,
                Quantity    = d.Quantity,
                UnitCost    = d.UnitCost
            }).ToList()
        };
    }
}
