using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.InventoryReceipt;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// API quản lý phiếu nhập kho (Master-Detail) — ASM-U04-01
    /// </summary>
    public class InventoryReceiptsController : BaseApiController
    {
        private readonly IInventoryReceiptService _svc;

        public InventoryReceiptsController(IInventoryReceiptService svc) => _svc = svc;

        /// <summary>
        /// Lấy thông tin chi tiết phiếu nhập kho bao gồm danh sách mặt hàng và sản phẩm (Eager Loading chống N+1).
        /// GET /api/v1/inventoryreceipts/{id}
        /// </summary>
        /// <param name="id">Mã định danh phiếu nhập</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Chi tiết phiếu nhập kèm danh sách hàng hóa.</response>
        /// <response code="404">Không tìm thấy phiếu nhập.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(InventoryReceiptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryReceiptDto>> GetById(int id, CancellationToken ct)
        {
            var r = await _svc.GetByIdAsync(id, ct);
            if (r is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Inventory Receipt with ID {id} was not found."
                });
            }

            return Ok(r);
        }

        /// <summary>
        /// Lấy danh sách phiếu nhập kho theo kho.
        /// GET /api/v1/inventoryreceipts?warehouseId=1
        /// </summary>
        /// <param name="warehouseId">Mã kho hàng</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Danh sách các phiếu nhập của kho.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<InventoryReceiptDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InventoryReceiptDto>>> GetByWarehouse(
            [FromQuery] int warehouseId, CancellationToken ct)
            => Ok(await _svc.GetByWarehouseAsync(warehouseId, ct));

        /// <summary>
        /// Báo cáo thống kê nhập kho hàng tháng theo kho (dùng LINQ projection với AsNoTracking).
        /// GET /api/v1/inventoryreceipts/summary?warehouseId=1&amp;year=2026&amp;month=10
        /// </summary>
        /// <param name="warehouseId">Mã kho</param>
        /// <param name="year">Năm</param>
        /// <param name="month">Tháng</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Báo cáo tổng hợp số lượng và chi phí nhập kho theo tháng.</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ReceiptMonthlySummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReceiptMonthlySummaryDto>> GetMonthlySummary(
            [FromQuery] int warehouseId, [FromQuery] int year, [FromQuery] int month, CancellationToken ct)
        {
            var summary = await _svc.GetMonthlyReceiptSummaryAsync(warehouseId, year, month, ct);
            return Ok(summary);
        }

        /// <summary>
        /// Tạo phiếu nhập kho ở trạng thái Draft.
        /// POST /api/v1/inventoryreceipts
        /// </summary>
        /// <param name="dto">Dữ liệu phiếu nhập</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="201">Tạo phiếu nhập thành công.</response>
        /// <response code="400">Dữ liệu không hợp lệ.</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryReceiptDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InventoryReceiptDto>> Create(
            [FromBody] CreateInventoryReceiptDto dto, CancellationToken ct)
        {
            var created = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Xác nhận phiếu nhập kho và tự động cộng dồn tồn kho cho các sản phẩm.
        /// POST /api/v1/inventoryreceipts/{id}/confirm
        /// </summary>
        /// <param name="id">Mã định danh phiếu nhập</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Xác nhận thành công.</response>
        /// <response code="404">Không tìm thấy phiếu hoặc phiếu không ở trạng thái Draft.</response>
        [HttpPost("{id:int:min(1)}/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(int id, CancellationToken ct)
        {
            bool confirmed = await _svc.ConfirmAsync(id, ct);
            if (!confirmed)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found or Invalid State",
                    Detail = $"Receipt ID {id} was not found or is not in Draft status."
                });
            }

            return NoContent();
        }
    }
}
