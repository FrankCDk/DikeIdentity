using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Application;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Security;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dike.Identity.Core.UseCases
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IPasswordHasher _password;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IApplicationRepository repository,
            [FromKeyedServices("Classic")] IPasswordHasher password,
            ILogger<ApplicationService> logger)
        {
            _repository = repository;
            _password = password;
            _logger = logger;
        }

        public async Task<Response<RegisterApplicationResponse>> RegisterApplicationAsync(RegisterApplicationRequest request)
        {
            var exists = await _repository.ExistsByCodeAsync(request.Code);

            if (exists)
            {
                return Response<RegisterApplicationResponse>.Failure(ApplicationErrors.AlreadyExists);
            }

            // 2. Generar el Secret en texto plano
            string rawSecret = $"sk_{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}";

            // 3. Hashear el secret usando tu implementación de Argon2id
            string secretHash = _password.HashPassword(rawSecret);

            Application application = new Application
            {
                Code = request.Code,
                Name = request.Name,
                SecretHash = secretHash,
                Status = StateStatus.active,
                CreatedAt = DateTime.Now,
                RedirectUris = request.RedirectUris.Select(uri => new ApplicationRedirectUris
                {
                    RedirectUri = uri.Uri,
                    Description = uri.Description
                }).ToList(),
                CorsOrigins = request.CorsOrigins.Select(origin => new ApplicationCorsOrigins
                {
                    OriginUrl = origin
                }).ToList()
            };

            await _repository.AddAsync(application);

            return Response<RegisterApplicationResponse>.Ok(new RegisterApplicationResponse(application.Id, rawSecret));
        }

        public async Task<Response<ApplicationSyncResponse>> SyncConfigurationAsync(ApplicationSyncRequest request)
        {
            var app = await _repository.GetByIdAsync(request.ClientId);

            if(app == null)
            {
                _password.VerifyPassword(request.ClientSecret, "$2a$12$HashFalsoParaEnganarAlAtacanteDeApps...");
                return Response<ApplicationSyncResponse>.Failure(new Error("APP_005", "Credenciales de aplicación inválidas."));
            }

            if(app.Status != StateStatus.active)
            {
                return Response<ApplicationSyncResponse>.Failure(new Error("APP_006", "La aplicación no está activa."));
            }

            // Verificar el ClientSecret enviado contra el Hash de la base de datos
            if (!_password.VerifyPassword(request.ClientSecret, app.SecretHash))
            {
                return Response<ApplicationSyncResponse>.Failure(new Error("APP_005", "Credenciales de aplicación inválidas."));
            }

            var responseData = new ApplicationSyncResponse(
                Id: app.Id,
                Code: app.Code,
                Name: app.Name,
                SecretHash: app.SecretHash,
                CorsOrigins: app.CorsOrigins.Select(c => c.OriginUrl).ToList(),
                RedirectUris: app.RedirectUris.Select(r => r.RedirectUri).ToList()
            );

            return Response<ApplicationSyncResponse>.Ok(responseData, "Sincronización exitosa.");

        }

        public async Task<Response<bool>> UpdateCorsOriginsAsync(UpdateCorsOriginsRequest request)
        {
            var app = await _repository.GetByIdWithCorsAsync(request.ApplicationId);

            if(app == null)
                return Response<bool>.Failure(ApplicationErrors.NotFound);

            _logger.LogInformation("Actualizando CORS para la aplicación {AppName}", app.Name);

            app.CorsOrigins.Clear(); // Limpiamos los CORS antiguos

            var cleanOrigins = request.NewOrigins
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => url.Trim())
                .Distinct();

            foreach (var origin in cleanOrigins)
            {
                app.CorsOrigins.Add(new ApplicationCorsOrigins
                {
                    OriginUrl = origin
                });
            }

            await _repository.UpdateAsync(app);
            return Response<bool>.Ok(true, "Origenes CORS actualizado correctamente.");

        }

        public async Task<Response<bool>> UpdateRedirectUrisAsync(UpdateRedirectUrisRequest request)
        {
            var app = await _repository.GetByIdWithRedirectUrisAsync(request.ApplicationId);

            if(app == null)
            {
                return Response<bool>.Failure(ApplicationErrors.NotFound);
            }

            _logger.LogInformation("Actualizando URI de redirección para la aplicación {AppName}", app.Name);

            app.RedirectUris.Clear(); // Limpiamos las URI de redirección antiguas

            var cleanUris = request.NewRedirectUris
                .Where(item => !string.IsNullOrWhiteSpace(item.Uri))
                .DistinctBy(item => item.Uri.Trim().ToLower());

            foreach (var item in cleanUris)
            {
                app.RedirectUris.Add(new ApplicationRedirectUris
                {
                    RedirectUri = item.Uri.Trim(),
                    Description = item.Description!.Trim()
                });
            }

            await _repository.UpdateAsync(app);
            return Response<bool>.Ok(true, "URI de redirección actualizado correctamente.");
        }
    }
}