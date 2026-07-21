namespace Dike.Identity.Core.Common
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Error? Error { get; set; } // Ahora el fallo contiene el objeto Error estructurado

        public static Response<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message };

        // Acepta el objeto Error y opcionalmente un diccionario con detalles de validación (de FluentValidation, por ejemplo)
        public static Response<T> Failure(Error error, IDictionary<string, string[]>? validationErrors = null)
        {
            if (validationErrors != null)
            {
                error.ValidationErrors = validationErrors;
            }

            return new Response<T> { Success = false, Error = error };
        }
    }

    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public IDictionary<string, string[]>? ValidationErrors { get; set; }

        public Error(string code, string message, IDictionary<string, string[]>? validationErrors = null)
        {
            Code = code;
            Message = message;
            ValidationErrors = validationErrors;
        }
    }

    public static class ApplicationErrors
    {
        public static readonly Error AlreadyExists = new("APP_001", "El código de aplicación ya está registrado en el sistema.");
        public static readonly Error NotFound = new("APP_002", "La aplicación solicitada no existe.");
        public static readonly Error InvalidSecret = new("APP_003", "Las credenciales de la aplicación son incorrectas.");
    }

    public static class UserErrors
    {
        public static readonly Error EmailAlreadyExists = new("USR_001", "El correo ya está registrado.");
        public static readonly Error ErrorRegister = new("USR_002", "Error al registrar el usuario. Intente nuevamente.");
    }

    public static class AuthErrors
    {
        public static readonly Error InvalidCredentials = new("AUTH_001", "Credenciales inválidas.");
        public static readonly Error AccountLocked = new("AUTH_002", "Cuenta bloqueada por múltiples intentos fallidos. Intente nuevamente más tarde.");
        public static readonly Error TokenNotFound = new("AUTH_003", "El Refresh Token no existe.");
        public static readonly Error TokenExpired = new("AUTH_004", "El Refresh Token ha expirado.");
        public static readonly Error TokenRevoked = new("AUTH_005", "El Refresh Token ha sido revocado.");
        public static readonly Error AssociatedUserNotFound = new("AUTH_006", "El usuario asociado al Refresh Token no existe.");
    }
}
