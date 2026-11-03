using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouse.Application.Contracts.Services;
using SmartWarehouse.Application.DTOs.Product;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>RESTful CRUD API cho Quản lý sản phẩm (Products)</summary>
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _svc;

        public ProductsController(IProductService svc) => _svc = svc;

        /// <summary>Lấy danh sách tất cả sản phẩm đang hoạt động</summary>
        /// <response code="200">Danh sách sản phẩm.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken ct)
            => Ok(await _svc.GetAllAsync(ct));

        /// <summary>Lấy chi tiết sản phẩm theo ID</summary>
        /// <param name="id">Mã sản phẩm</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="200">Chi tiết sản phẩm.</response>
        /// <response code="404">Không tìm thấy sản phẩm.</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
        {
            var p = await _svc.GetByIdAsync(id, ct);
            return p is null ? NotFound() : Ok(p);
        }

        /// <summary>Tạo mới sản phẩm</summary>
        /// <param name="dto">Dữ liệu tạo sản phẩm</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="201">Tạo thành công sản phẩm.</response>
        /// <response code="400">Dữ liệu không hợp lệ.</response>
        /// <response code="409">Mã SKU bị trùng.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductDto>> Create(
            [FromBody] CreateProductDto dto, CancellationToken ct)
        {
            var created = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Cập nhật thông tin sản phẩm</summary>
        /// <param name="id">Mã sản phẩm</param>
        /// <param name="dto">Thông tin cập nhật</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Cập nhật thành công.</response>
        /// <response code="404">Không tìm thấy sản phẩm.</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
        {
            bool updated = await _svc.UpdateAsync(id, dto, ct);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>Xóa sản phẩm theo ID</summary>
        /// <param name="id">Mã sản phẩm</param>
        /// <param name="ct">Cancellation token</param>
        /// <response code="204">Xóa thành công.</response>
        /// <response code="404">Không tìm thấy sản phẩm.</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            bool deleted = await _svc.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
