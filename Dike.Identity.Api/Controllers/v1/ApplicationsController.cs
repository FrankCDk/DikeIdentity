using Asp.Versioning;
using Dike.Identity.Core.DTOs.Application;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers.v1
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _service;

        public ApplicationsController(IApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] RegisterApplicationRequest request)
        {
            var result = await _service.RegisterApplicationAsync(request);

            if(!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Endpoint M2M para que los Workers de los proyectos clientes (ej: DikePortal) 
        /// sincronicen sus llaves de firma y reglas de CORS/Redirect en memoria RAM.
        /// </summary>
        [HttpPost("sync")]
        [AllowAnonymous]
        public async Task<IActionResult> Sync([FromBody] ApplicationSyncRequest request)
        {
            var result = await _service.SyncConfigurationAsync(request);

            if (!result.Success)
            {
                return Unauthorized(result); // Devolvemos 401 si las credenciales de la app fallan
            }

            return Ok(result);
        }

        /// <summary>
        /// Actualiza de forma masiva los orígenes CORS permitidos para una aplicación.
        /// </summary>
        [HttpPut("cors")]
        public async Task<IActionResult> UpdateCors([FromBody] UpdateCorsOriginsRequest request)
        {
            var result = await _service.UpdateCorsOriginsAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza de forma masiva las URIs de redirección válidas para el inicio de sesión.
        /// </summary>
        [HttpPut("redirect-uris")]
        public async Task<IActionResult> UpdateRedirectUris([FromBody] UpdateRedirectUrisRequest request)
        {
            var result = await _service.UpdateRedirectUrisAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

    }
}
