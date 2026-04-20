using Asp.Versioning;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dike.Identity.Api.Controllers.v1
{

    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Register User
        /// <summary>
        /// Registro del usuario utilizando un método de seguridad estándar, que incluye hashing de contraseñas con bcrypt y validación básica.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register/classic")]
        public async Task<IActionResult> RegisterClassic([FromBody] RegisterRequest request)
        {
            await _authService.RegisterStandardAsync(request);
            return CreatedResponse(true, "Usuario registrado con seguridad estándar.");
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
            await _authService.RegisterWithArgon2Async(request);
            return CreatedResponse(true, "Usuario registrado con seguridad Argon2id.");
        }
        #endregion

        #region Login
        /// <summary>
        /// JWT Clásico: Autenticación tradicional con email/username y contraseña, devolviendo un token JWT simple. Ideal para la mayoría de los casos de uso estándar.
        /// </summary>
        /// <returns></returns>
        [HttpPost("login/classic")]
        public async Task<IActionResult> LoginClassic([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginStandardAsync(request);
            return SuccessResponse(result, "Autenticación estándar exitosa.");
        }


        /// <summary>
        /// Argon2id: Autenticación de alta seguridad utilizando el algoritmo de hashing Argon2id para proteger las contraseñas. 
        /// Recomendado para aplicaciones que requieren un nivel adicional de seguridad, aunque puede ser más lento que el método clásico.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login/hardened")]
        public async Task<IActionResult> LoginHardened([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginWithArgon2Async(request);
            return SuccessResponse(result, "Autenticación de alta seguridad (Argon2id) exitosa.");
        }
        #endregion

    }
}
