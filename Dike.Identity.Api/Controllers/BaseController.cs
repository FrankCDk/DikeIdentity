using Dike.Identity.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        // Método para respuestas exitosas (200 OK)
        protected IActionResult SuccessResponse<T>(T data, string message = "Success")
        {
            return Ok(BaseResponse<T>.Ok(data, message));
        }

        // Método para creaciones exitosas (201 Created)
        protected IActionResult CreatedResponse<T>(T data, string message = "Resource created successfully")
        {
            return StatusCode(201, BaseResponse<T>.Ok(data, message));
        }
    }
}
