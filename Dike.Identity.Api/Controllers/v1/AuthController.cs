using Asp.Versioning;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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
    
        /// <summary>
        /// Refresh Token: Permite a los usuarios obtener un nuevo token de acceso utilizando un token de actualización válido, sin necesidad de volver a ingresar sus credenciales.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            return SuccessResponse(response, "Token refrescado exitosamente.");
        }

    }
}
