using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Common;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Warehouse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// RESTful CRUD API cho Quản lý kho hàng — ASM-U01-01
    /// </summary>
    public class WarehousesController : BaseApiController
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        /// <summary>
        /// Lấy danh sách tất cả các kho đang hoạt động.
        /// GET /api/v1/warehouses
        /// </summary>
        /// <response code="200">Trả về danh sách các kho đang hoạt động.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<WarehouseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<WarehouseDto>>> GetAll()
        {
            var result = await _warehouseService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết thông tin một kho theo ID.
        /// GET /api/v1/warehouses/{id}
        /// </summary>
        /// <param name="id">Mã định danh kho</param>
        /// <response code="200">Trả về thông tin chi tiết của kho.</response>
        /// <response code="404">Không tìm thấy kho với ID chỉ định.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseDto>> GetById(int id)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id);
            if (warehouse is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Warehouse with ID {id} was not found."
                });
            }

            return Ok(warehouse);
        }

        /// <summary>
        /// Tạo mới một kho hàng với kiểm tra tính duy nhất của mã kho.
        /// POST /api/v1/warehouses
        /// </summary>
        /// <param name="dto">Dữ liệu tạo kho mới</param>
        /// <response code="201">Tạo mới kho thành công kèm header Location.</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
        /// <response code="409">Mã kho đã tồn tại (Conflict).</response>
        [HttpPost]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto dto)
        {
            var createdWarehouse = await _warehouseService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdWarehouse.Id },
                createdWarehouse);
        }

        /// <summary>
        /// Cập nhật thông tin kho hàng theo ID.
        /// PUT /api/v1/warehouses/{id}
        /// </summary>
        /// <param name="id">Mã định danh kho</param>
        /// <param name="dto">Thông tin kho cần cập nhật</param>
        /// <response code="204">Cập nhật thành công (No Content).</response>
        /// <response code="400">Dữ liệu đầu vào không hợp lệ.</response>
        /// <response code="404">Không tìm thấy kho cần cập nhật.</response>
        /// <response code="409">Mã kho cập nhật trùng với kho khác (Conflict).</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            bool updated = await _warehouseService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Warehouse with ID {id} was not found."
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Xóa kho hàng theo ID.
        /// DELETE /api/v1/warehouses/{id}
        /// </summary>
        /// <param name="id">Mã định danh kho</param>
        /// <response code="204">Xóa thành công (No Content).</response>
        /// <response code="404">Không tìm thấy kho cần xóa.</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            bool deleted = await _warehouseService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"Warehouse with ID {id} was not found."
                });
            }

            return NoContent();
        }
    }
}
