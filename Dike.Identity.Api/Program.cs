using Asp.Versioning;
using Dike.Identity.Api.Configurations;
using Dike.Identity.Api.Middlewares;
using Dike.Identity.Core;
using Dike.Identity.Core.Enums;
using Dike.Identity.Providers.Jwt;
using Dike.Identity.Providers.Persistence;
using Dike.Identity.Providers.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogConfiguration(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("AuthDb") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

#region Creamos un DataSource de Npgsql y mapeamos todos tus Enums aquí
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<StateStatus>("state_type");
dataSourceBuilder.MapEnum<PermissionAction>("action_type");
dataSourceBuilder.MapEnum<PermissionResource>("resource_type");
dataSourceBuilder.MapEnum<AuthProvider>("auth_provider_type");
dataSourceBuilder.MapEnum<LogSeverity>("log_severity");
var dataSource = dataSourceBuilder.Build();
//builder.Services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(dataSource));
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(dataSource, npgsqlOptions =>
        npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ));
#endregion

#region Configuración de versionado de la API

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; //Si el cliente no especifica una versión usamos la versión por defecto (linea de abajo)
    options.DefaultApiVersion = new ApiVersion(1, 0); //Indica la versión predeterminada
    options.ReportApiVersions = true; //Devuelve en las cabeceras las versiones disponibles de la API
    options.ApiVersionReader = ApiVersionReader.Combine( //Aqui definimos como el cliente puede enviar la versión de la Api
        new UrlSegmentApiVersionReader(), //Versión en la URL
        new HeaderApiVersionReader("X-Version"), //Versión en la cabecera personalizada
        new MediaTypeApiVersionReader("ver") //Versión en el Content-Type o Accept como parámetro
    );
}).AddApiExplorer(options => //Habilita el soporte para ApiExplorer (Swagger)
{
    options.GroupNameFormat = "'v'VVV"; //Define el formato del grupo de versión en Swagger
    options.SubstituteApiVersionInUrl = true; //Hace que el placeholder {version} en tus rutas se reemplace automáticamente por la versión correspondiente (En los Controllers)
});

#endregion

#region Desactivamos la validación automática de los modelos
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
#endregion

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistenceInfrastructure();
builder.Services.AddCoreServices();
builder.Services.AddSecurityInfrastructure();
builder.Services.AddJwtInfrastructure();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuditMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
