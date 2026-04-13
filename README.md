## Arquitectura y Diseño

**DikeIdentity** está desarrollado siguiendo los principios de **Arquitectura Limpia (Clean Architecture)**
Este diseño garantiza que la lógica de negocio central permanezca completamente aislada de
frameworks externos, bases de datos o mecanismos de entrega, resultando en un Proveedor de Identidad (IdP)
altamente mantenible, testeable y escalable.

### 📂 Estructura del proyecto

La solución esta dividida en capas distintas para aplicar la separación de responsabilidades:

* **`Dike.Identity.Core` (Capa de Dominio y Aplicación):**
	Contiene las entidades del dominio (Usuarios, Roles, Permisos, Aplicaciones), las reglas
	de negocio y las interfaces de los repositorios. Este proyecto tiene **cero dependencias**
	de frameworks externos o detalles de infraestructura.
	
* **`Dike.Identity.Providers.` (Capa de infraestructura): **
	Estos proyectos actúan como los "Adaptadores" que implementan las interfaces definidas en el Core. Pueden ser
	intercambiados sin afectar la lógica de negocio.
	* `Providers.Persistence`: Maneja el acceso a datos usando **PostgreSQL**. Implementa un enfoque de ORM dual (Entity Framework Core para escrituras/cambios de estado, y Dapper para lecturas de alto rendimiento).
	* `Providers.Jwt`: Encapsula la lógica para generar, firmar y validar los JSON Web Tokens (JWT) y Refresh Tokens.
	* `Providers.Cache`: Gestiona la caché distribuida (ej. Redis) para optimizar lecturas de datos de alta frecuencia, como la validación de permisos.

* **`Dike.Identity.Api` (Capa de Presentación):**
  Una API RESTful que actúa como el punto de entrada al sistema. Maneja las peticiones HTTP, la validación de los datos de entrada (payloads), el enrutamiento y delega las operaciones de negocio al Core.
  
   
### 💡 Decisiones Clave de Arquitectura 

Para asegurar una calidad de nivel empresarial, el sistema implementa los siguientes patrones y decisiones:

* **Control de Acceso Basado en Roles (RBAC) impulsado por Datos:** La gestión de privilegios y autorizaciones se realiza de forma centralizada y relacional a nivel de base de datos. Esto permite mantener las entidades de dominio en C# fuertemente tipadas, limpias y predecibles, delegando la evaluación de los permisos en tiempo de ejecución al pipeline de seguridad nativo de .NET Identity.
* **Preparado para Multi-Tenant (SaaS):** El esquema de la base de datos incluye la entidad `applications`, permitiendo que una sola instancia de DikeIdentity sirva como el Proveedor de Identidad centralizado para múltiples proyectos de software o entornos distintos.
* **Rastro de Auditoría Inmutable:** Los eventos de seguridad (inicios de sesión, asignaciones de roles) se registran en una tabla `audit_logs` aprovechando el tipo de dato `JSONB` de PostgreSQL con indexación `GIN`, proporcionando consultas de altísima velocidad sobre detalles de logs no estructurados.
* **Estandarización Global de Tiempo:** Todos los datos temporales se almacenan estrictamente en UTC utilizando `TIMESTAMPTZ` de PostgreSQL, eliminando los errores de conversión de zonas horarias y asegurando la consistencia global.
* **Seguridad Criptográfica:** Las contraseñas nunca se almacenan en texto plano. El sistema utiliza **Argon2** para el hash seguro de contraseñas y estandariza el uso de `UUIDs` (v4) para todas las llaves primarias, previniendo ataques de enumeración de IDs.


## 🔐 Flujo de Autenticación y Seguridad (JWT - Json Web Token)

Como Proveedor de Identidad, el núcleo de **DikeIdentity** es la emisión segura de credenciales. El sistema implementa un flujo de autenticación robusto basado en el estándar OAuth2 / OpenID Connect, utilizando **JSON Web Tokens (JWT)**.

### Estrategia de Tokens (Access & Refresh)
Para equilibrar la seguridad con la experiencia del usuario, el sistema no emite tokens de larga duración. En su lugar, utiliza un patrón de doble token:

* **Access Token (JWT):** Un token de corta duración (ej. 15 minutos) que viaja en las cabeceras HTTP (Bearer). Contiene los *Claims* del usuario (ID, Email) y los permisos estructurados (Roles/Actions) necesarios para que las APIs consumidoras autoricen las peticiones sin consultar a la base de datos.
* **Refresh Token:** Un token opaco, criptográficamente seguro y de larga duración, almacenado en la base de datos (PostgreSQL). Permite al cliente solicitar un nuevo Access Token cuando el anterior expira, sin obligar al usuario a iniciar sesión nuevamente. Esto permite revocar accesos en tiempo real si una cuenta es comprometida.

### Firma Criptográfica (RS256)

A diferencia de los enfoques básicos que usan firmas simétricas (HS256), DikeIdentity está diseñado para soportar **RS256 (RSA Signature with SHA-256)**. 
* El IdP firma los tokens con una **Clave Privada**.
* Las aplicaciones cliente (APIs externas) pueden validar la autenticidad del token utilizando una **Clave Pública**, garantizando que el token no ha sido alterado sin necesidad de compartir el secreto principal.

  
  
## 🚀 Tecnologías Utilizadas

El proyecto está construido sobre el ecosistema moderno de Microsoft, priorizando el rendimiento y la seguridad multiplataforma:

* **Framework:** [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (C# 12)
* **Base de Datos:** PostgreSQL 16+
* **Acceso a Datos (Dual-ORM):**
  * *Entity Framework Core:* Para migraciones, seguimiento de estado (Tracking) y escrituras complejas.
  * *Dapper:* Para consultas de lectura (Queries) de ultra-alta velocidad.
* **Seguridad y Criptografía:** * Algoritmo de Hashing: **Argon2** (Estándar recomendado por OWASP).
  * Firmas de Tokens: **RS256** (Asimétrico con llaves públicas/privadas).
* **Validación de Datos:** FluentValidation.

---

## ⚙️ Configuración y Ejecución Local

Para levantar el proyecto en tu entorno local, sigue estos pasos:

### 1. Clonar el repositorio

```bash
git clone [https://github.com/FrankCDk/DikeIdentity.git](https://github.com/FrankCDk/DikeIdentity.git)
cd DikeIdentity
```

### 2. Configurar la Base de Datos

El proyecto requiere una instancia de PostgreSQL en ejecución.
Crea una base de datos vacía llamada dike_identity_db.

Ejecuta el script de inicialización (/scripts/init_schema.sql) para generar las tablas y los índices (o utiliza las migraciones de EF Core si están habilitadas).
  
### 3. Configurar Variables de Envío (appsettings.json)

Navega al proyecto Dike.Identity.Api y renombra el archivo appsettings.Development.template.json a appsettings.Development.json. Configura tus credenciales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=dike_identity_db; Username=postgres; Password=TU_PASSWORD"
  },
  "JwtSettings": {
    "Issuer": "DikeIdentityServer",
    "Audience": "DikeIdentityClients",
    "PrivateKeyPath": "Ruta/A/Tu/ClavePrivada.pem"
  }
}
```

### 4. Compilar y Ejecutar

Puedes iniciar la API usando Visual Studio, Rider, o mediante la CLI de .NET:

```bash
dotnet build
dotnet run --project src/Dike.Identity.Api/Dike.Identity.Api.csproj
```

La documentación interactiva de la API estará disponible en: https://localhost:port/swagger


### 🛣️ Roadmap (Próximos Pasos)
El desarrollo de DikeIdentity está estructurado en fases incrementales:

[x] Fase 0: Arquitectura y Diseño de Datos. (Esquema Multi-Tenant, RBAC, Logs Inmutables en PostgreSQL).

[ ] Fase 1 (MVP): Núcleo de Identidad. Registro de usuarios locales, Login, validación de contraseñas con Argon2 y emisión de JWT/Refresh Tokens.

[ ] Fase 2: Middleware de Seguridad. Implementación de políticas de autorización basadas en datos (RBAC) y registro automatizado en la tabla de audit_logs.

[ ] Fase 3: Proveedores Externos. Integración del flujo de autorización OAuth2 para permitir inicio de sesión con Google y GitHub.

[ ] Fase 4: DevOps. Dockerización completa de la API y la base de datos mediante docker-compose para un despliegue en 1-clic.




