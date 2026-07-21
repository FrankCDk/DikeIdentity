using Asp.Versioning;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registro del usuario utilizando un método de seguridad estándar, que incluye hashing de contraseñas con bcrypt y validación básica.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register/classic")]
        public async Task<IActionResult> RegisterClassic([FromBody] RegisterRequest request)
        {
            var response = await _userService.RegisterStandardAsync(request);

            if (!response.Success)
            {
                return BadRequest(response); // Retorna 400 con el objeto Error estructurado
            }

            // Retorna 200 Ok con la data (el Guid del usuario) y el mensaje de éxito
            return Ok(response);

        }


        /// <summary>
        /// Registro del usuario utilizando el algoritmo de hashing Argon2id, que ofrece una mayor resistencia a ataques de fuerza bruta y 
        /// es recomendado para aplicaciones que requieren un nivel adicional de seguridad.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register/hardened")]
        public async Task<IActionResult> RegisterHardened([FromBody] RegisterRequest request)
        {
            var response = await _userService.RegisterWithArgon2Async(request);

            if (!response.Success)
            {
                return BadRequest(response); // Retorna 400 con el objeto Error estructurado
            }

            // Retorna 200 Ok con la data (el Guid del usuario) y el mensaje de éxito
            return Ok(response);
        }

    }
}
