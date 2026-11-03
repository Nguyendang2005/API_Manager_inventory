using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Common;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.InventoryIssue;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// API Quản lý xuất kho an toàn với phân quyền JWT Role-Based Policies — ASM-U05 &amp; ASM-U06
    /// </summary>
    [Route("api/v1/inventory-issues")]
    public class InventoryIssuesController : BaseApiController
    {
        private readonly IInventoryIssueService _svc;

        public InventoryIssuesController(IInventoryIssueService svc) => _svc = svc;

        /// <summary>
        /// Lấy danh sách phiếu xuất kho có phân trang.
        /// GET /api/v1/inventory-issues?pageIndex=1&amp;pageSize=10
        /// </summary>
        /// <param name="pageIndex">Số thứ tự trang (mặc định: 1)</param>
        /// <param name="pageSize">Số bản ghi mỗi trang (mặc định: 10)</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Danh sách phiếu xuất kho phân trang.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PageResult<InventoryIssueDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResult<InventoryIssueDto>>> GetPaged(
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
            => Ok(await _svc.GetPagedAsync(pageIndex, pageSize, ct));

        /// <summary>
        /// Lấy chi tiết thông tin phiếu xuất kho theo ID.
        /// GET /api/v1/inventory-issues/{id}
        /// </summary>
        /// <param name="id">Mã định danh phiếu xuất</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Thông tin chi tiết phiếu xuất.</response>
        /// <response code="404">Không tìm thấy phiếu xuất.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(InventoryIssueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryIssueDto>> GetById(int id, CancellationToken ct)
        {
            var issue = await _svc.GetByIdAsync(id, ct);
            if (issue is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Inventory Issue with ID {id} was not found."
                });
            }

            return Ok(issue);
        }

        /// <summary>
        /// Tạo phiếu xuất kho và trừ hàng tồn kho nguyên tử (Atomic Stock Deduction qua Unit of Work).
        /// Quyền yêu cầu: WarehouseStaff hoặc WarehouseManager.
        /// POST /api/v1/inventory-issues
        /// </summary>
        /// <param name="dto">Dữ liệu phiếu xuất</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="201">Tạo phiếu xuất thành công.</response>
        /// <response code="400">Dữ liệu yêu cầu không hợp lệ.</response>
        /// <response code="401">Chưa xác thực Bearer Token.</response>
        /// <response code="403">Không có quyền thao tác.</response>
        /// <response code="409">Không đủ tồn kho (InsufficientStockException).</response>
        [HttpPost]
        [Authorize(Roles = "WarehouseStaff,WarehouseManager")]
        [ProducesResponseType(typeof(InventoryIssueDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InventoryIssueDto>> Create(
            [FromBody] CreateInventoryIssueDto dto, CancellationToken ct)
        {
            var created = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Duyệt phiếu xuất kho (Approve).
        /// Quyền yêu cầu: CHỈ DÀNH CHO WarehouseManager.
        /// PUT /api/v1/inventory-issues/{id}/approve
        /// </summary>
        /// <param name="id">Mã định danh phiếu xuất</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Duyệt phiếu xuất thành công.</response>
        /// <response code="401">Chưa xác thực Bearer Token.</response>
        /// <response code="403">Không có quyền WarehouseManager (Forbidden).</response>
        /// <response code="404">Không tìm thấy phiếu xuất hoặc trạng thái không hợp lệ.</response>
        [HttpPut("{id:int:min(1)}/approve")]
        [Authorize(Roles = "WarehouseManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(int id, CancellationToken ct)
        {
            bool approved = await _svc.ApproveAsync(id, ct);
            if (!approved)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found or Invalid Status",
                    Detail = $"Inventory Issue with ID {id} was not found or is not in Draft status."
                });
            }

            return NoContent();
        }
    }
}
