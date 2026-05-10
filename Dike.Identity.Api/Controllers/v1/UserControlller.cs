using Asp.Versioning;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : BaseController
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
            var userId = await _userService.RegisterStandardAsync(request);
            return CreatedResponse(new { id = userId }, "Usuario registrado con seguridad estándar.");
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
            var userId = await _userService.RegisterWithArgon2Async(request);
            return CreatedResponse(new { id = userId }, "Usuario registrado con seguridad Argon2id.");
        }

    }
}
