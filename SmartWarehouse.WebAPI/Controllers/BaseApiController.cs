using Microsoft.AspNetCore.Mvc;

namespace SmartWarehouse.WebAPI.Controllers
{
    /// <summary>
    /// Controller cơ sở: cung cấp [ApiController] + route baseline.
    /// Tất cả controller kế thừa để đảm bảo LSP và nhất quán HTTP behavior.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseApiController : ControllerBase { }
}
