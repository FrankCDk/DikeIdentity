using Asp.Versioning;
using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Role;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RolesController : ControllerBase
    {

        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleRegisterRequest roleName)
        {
            await _service.RegisterAsync(roleName);

            return Ok(new Response<string> { Message = "Rol creado exitosamente." });
        }



    }
}
