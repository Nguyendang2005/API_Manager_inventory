using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Supplier;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// RESTful CRUD API cho Quản lý nhà cung cấp (Suppliers) — ASM-U02-01
    /// </summary>
    public class SuppliersController : BaseApiController
    {
        private readonly ISupplierService _svc;

        public SuppliersController(ISupplierService svc) => _svc = svc;

        /// <summary>
        /// Lấy danh sách nhà cung cấp hoặc lọc theo tên.
        /// GET /api/v1/suppliers?search={name}
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm theo tên nhà cung cấp</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Danh sách nhà cung cấp.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SupplierDto>>> GetAll(
            [FromQuery] string? search, CancellationToken ct)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? await _svc.GetAllAsync(ct)
                : await _svc.SearchByNameAsync(search, ct);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết nhà cung cấp theo ID.
        /// GET /api/v1/suppliers/{id}
        /// </summary>
        /// <param name="id">Mã định danh nhà cung cấp (tối thiểu 1)</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Thông tin chi tiết nhà cung cấp.</response>
        /// <response code="404">Không tìm thấy nhà cung cấp.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierDto>> GetById(int id, CancellationToken ct)
        {
            var supplier = await _svc.GetByIdAsync(id, ct);
            if (supplier is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Supplier with ID {id} was not found."
                });
            }

            return Ok(supplier);
        }

        /// <summary>
        /// Thêm mới nhà cung cấp với kiểm tra format mã số thuế, email, phone, supplier code.
        /// POST /api/v1/suppliers
        /// </summary>
        /// <param name="dto">Dữ liệu tạo nhà cung cấp</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="201">Tạo thành công nhà cung cấp với Location header.</response>
        /// <response code="400">Dữ liệu không hợp lệ.</response>
        /// <response code="409">Mã nhà cung cấp đã tồn tại.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SupplierDto>> Create(
            [FromBody] CreateSupplierDto dto, CancellationToken ct)
        {
            var created = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Cập nhật thông tin nhà cung cấp theo ID.
        /// PUT /api/v1/suppliers/{id}
        /// </summary>
        /// <param name="id">Mã định danh nhà cung cấp</param>
        /// <param name="dto">Dữ liệu cập nhật</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Cập nhật thành công (No Content).</response>
        /// <response code="400">Dữ liệu không hợp lệ.</response>
        /// <response code="404">Không tìm thấy nhà cung cấp.</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateSupplierDto dto, CancellationToken ct)
        {
            bool updated = await _svc.UpdateAsync(id, dto, ct);
            if (!updated)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Supplier with ID {id} was not found."
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa nhà cung cấp theo ID.
        /// DELETE /api/v1/suppliers/{id}
        /// </summary>
        /// <param name="id">Mã định danh nhà cung cấp</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Xóa thành công (No Content).</response>
        /// <response code="404">Không tìm thấy nhà cung cấp.</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            bool deleted = await _svc.DeleteAsync(id, ct);
            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Supplier with ID {id} was not found."
                });
            }

            return NoContent();
        }
    }
}
