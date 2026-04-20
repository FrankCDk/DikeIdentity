using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Security;
using Microsoft.IdentityModel.Tokens;

namespace Dike.Identity.Providers.Jwt
{
    public class JwtProvider : IJwtProvider
    {

        // En un entorno real, cargarías la llave privada desde un archivo .pem o Key Vault
        // Para el MVP, usaremos una llave generada en tiempo de ejecución o una simétrica temporal
        private readonly byte[] _secretKey = Encoding.UTF8.GetBytes("EstaEsUnaLlaveSuperSecretaDe32Bytes!");

        public AuthResponse GenerateTokens(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.Name),
                new Claim("last_name", user.LastName),
                new Claim("provider", user.AuthProvider.ToString())
            };

            var expiration = DateTime.UtcNow.AddMinutes(15);

            // Por ahora, usaremos firma simétrica (HS256) para que puedas probarlo YA.
            // Cambiar a RS256 requiere cargar un archivo .pem (podemos hacerlo en el siguiente paso).
            var key = new SymmetricSecurityKey(_secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "DikeIdentity",
                audience: "DikeClients",
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse(
                AccessToken: tokenString,
                RefreshToken: Guid.NewGuid().ToString(), // MVP: Token aleatorio
                Expiration: expiration
            );
        }
    }
}
