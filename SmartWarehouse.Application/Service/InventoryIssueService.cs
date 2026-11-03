using SmartWarehouse.Application.Common;
using SmartWarehouse.Application.Contracts.Persistence;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.InventoryIssue;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using SmartWarehouse.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.Application.Service
{
    /// <summary>
    /// Business logic xuất kho với đảm bảo nguyên tử (atomic) qua Unit of Work.
    /// Nếu bất kỳ item nào thiếu stock → ném InsufficientStockException → không commit.
    /// </summary>
    public class InventoryIssueService : IInventoryIssueService
    {
        private readonly IInventoryIssueRepository _issueRepo;
        private readonly IProductRepository        _productRepo;
        private readonly IUnitOfWork               _unitOfWork;

        public InventoryIssueService(
            IInventoryIssueRepository issueRepo,
            IProductRepository productRepo,
            IUnitOfWork unitOfWork)
        {
            _issueRepo   = issueRepo;
            _productRepo = productRepo;
            _unitOfWork  = unitOfWork;
        }

        public async Task<PageResult<InventoryIssueDto>> GetPagedAsync(
            int pageIndex, int pageSize, CancellationToken ct = default)
        {
            var items = await _issueRepo.GetPagedAsync(pageIndex, pageSize, ct);
            int total = await _issueRepo.GetTotalCountAsync(ct);

            return new PageResult<InventoryIssueDto>
            {
                Items      = items.Select(MapToDto).ToList(),
                PageNumber = pageIndex,
                PageSize   = pageSize,
                TotalCount = total
            };
        }

        public async Task<InventoryIssueDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var issue = await _issueRepo.GetByIdAsync(id, ct);
            return issue is null ? null : MapToDto(issue);
        }

        /// <summary>
        /// Tạo phiếu xuất kho ATOMIC:
        ///   1. Kiểm tra stock từng sản phẩm
        ///   2. Trừ stock
        ///   3. Tạo phiếu + chi tiết
        ///   4. CommitAsync() một lần duy nhất
        /// </summary>
        public async Task<InventoryIssueDto> CreateAsync(
            CreateInventoryIssueDto dto, CancellationToken ct = default)
        {
            string code = $"ISS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpperInvariant()}";

            var issue = new InventoryIssue
            {
                IssueCode   = code,
                WarehouseId = dto.WarehouseId,
                IssueDate   = dto.IssueDate,
                Status      = IssueStatus.Draft,
                Notes       = dto.Notes,
                CreatedAt   = DateTime.UtcNow
            };

            // Kiểm tra và trừ stock — nếu thiếu sẽ ném exception (không commit gì cả)
            foreach (var d in dto.Details)
            {
                var product = await _productRepo.GetByIdAsync(d.ProductId, ct)
                    ?? throw new InvalidOperationException($"Product ID {d.ProductId} không tồn tại.");

                if (product.StockQuantity < d.Quantity)
                    throw new InsufficientStockException(product.SKU, product.StockQuantity, d.Quantity);

                // Trừ stock ngay trong memory — chỉ persist khi CommitAsync
                product.StockQuantity -= d.Quantity;

                issue.Details.Add(new InventoryIssueDetail
                {
                    ProductId = d.ProductId,
                    Quantity  = d.Quantity,
                    UnitPrice = product.UnitPrice
                });
            }

            await _issueRepo.AddAsync(issue, ct);

            // CommitAsync() — gọi SaveChangesAsync() một lần duy nhất qua UnitOfWork
            await _unitOfWork.CommitAsync(ct);

            var created = await _issueRepo.GetByIdAsync(issue.Id, ct);
            return MapToDto(created!);
        }

        public async Task<bool> ApproveAsync(int id, CancellationToken ct = default)
        {
            var issue = await _issueRepo.GetByIdAsync(id, ct);
            if (issue is null || issue.Status != IssueStatus.Draft)
                return false;

            issue.Status = IssueStatus.Approved;
            await _unitOfWork.CommitAsync(ct);
            return true;
        }

        private static InventoryIssueDto MapToDto(InventoryIssue i) => new()
        {
            Id            = i.Id,
            IssueCode     = i.IssueCode,
            WarehouseId   = i.WarehouseId,
            WarehouseName = i.Warehouse?.Name ?? string.Empty,
            IssueDate     = i.IssueDate,
            Status        = i.Status,
            Notes         = i.Notes,
            CreatedAt     = i.CreatedAt,
            Details       = i.Details.Select(d => new InventoryIssueDetailDto
            {
                ProductId   = d.ProductId,
                ProductName = d.Product?.Name ?? string.Empty,
                SKU         = d.Product?.SKU ?? string.Empty,
                Quantity    = d.Quantity,
                UnitPrice   = d.UnitPrice
            }).ToList()
        };
    }
}
