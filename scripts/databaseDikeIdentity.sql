-- Extensión requerida para gen_random_uuid() en versiones de PostgreSQL < 13
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TYPE state_type AS ENUM('active', 'inactive');

CREATE TABLE roles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code CHAR(10) UNIQUE NOT NULL,
    name VARCHAR(80) NOT NULL,
    normalized_name VARCHAR(80) UNIQUE NOT NULL,
    description TEXT,
    is_default BOOLEAN DEFAULT FALSE,
    status state_type NOT NULL DEFAULT 'active',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NULL, -- UUID del usuario creador
    updated_at TIMESTAMPTZ NULL,
    updated_by UUID NULL
);
COMMENT ON TABLE roles IS 'Tabla para gestionar los niveles de acceso del Identity Provider.';
CREATE INDEX "IX_roles_normalized_name" ON roles(normalized_name);


CREATE TYPE action_type AS ENUM('read', 'create', 'update', 'delete', 'manage'); -- Tipos de acciones
CREATE TYPE resource_type AS ENUM('user', 'role', 'token', 'audit', 'system'); -- Tipo de recurso del permiso

CREATE TABLE permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    action action_type NOT NULL,
    resource resource_type NOT NULL,
    name VARCHAR(100) UNIQUE NOT NULL,
    normalized_name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    status state_type NOT NULL DEFAULT 'active',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NULL,
    updated_at TIMESTAMPTZ NULL,
    updated_by UUID NULL
);
COMMENT ON TABLE permissions IS 'Tabla para gestionar los permisos de uso.';


CREATE TABLE role_permissions (
    role_id UUID NOT NULL,
    permission_id UUID NOT NULL,
    
    PRIMARY KEY (role_id, permission_id),
    
    CONSTRAINT fk_role FOREIGN KEY (role_id) 
        REFERENCES roles(id) ON DELETE CASCADE,
        
    CONSTRAINT fk_permission FOREIGN KEY (permission_id) 
        REFERENCES permissions(id) ON DELETE CASCADE
);
COMMENT ON TABLE role_permissions IS 'Tabla para gestionar los permisos de uso.';


CREATE TABLE applications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(20) UNIQUE NOT NULL, -- Código corto identificativo (ej: 'CRM', 'MKT')
    name VARCHAR(100) UNIQUE NOT NULL,
    secret_hash TEXT NOT NULL,        -- Hash del secreto/clave de la app
    status state_type NOT NULL DEFAULT 'active',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NULL,
    updated_at TIMESTAMPTZ NULL,
    updated_by UUID NULL
);

CREATE TABLE application_redirect_uris (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    application_id UUID NOT NULL,
    redirect_uri TEXT NOT NULL,
    description VARCHAR(100) NULL, -- Ej: 'Localhost Desarrollo', 'Producción'
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_application_redirect FOREIGN KEY (application_id) 
        REFERENCES applications(id) ON DELETE CASCADE
);

-- Índice para validar rápidamente si una URI de redirección enviada pertenece a la app
CREATE INDEX idx_app_redirect_uri ON application_redirect_uris(application_id, redirect_uri);

CREATE TABLE application_cors_origins (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    application_id UUID NOT NULL,
    origin_url TEXT NOT NULL, -- Ej: 'http://localhost:4200', 'https://mi-app.com'
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_application_cors FOREIGN KEY (application_id) 
        REFERENCES applications(id) ON DELETE CASCADE
);

-- Índice para consultas de CORS ultra rápidas por aplicación
CREATE INDEX idx_app_cors_origin ON application_cors_origins(application_id);


CREATE TYPE auth_provider_type AS ENUM('local', 'google', 'github');

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    
    -- Datos de identidad
    email VARCHAR(120) UNIQUE NOT NULL,
    normalized_email VARCHAR(120) UNIQUE NOT NULL,
    password_hash TEXT NULL, -- Null si usa Google/Github (Argon2 irá aqui)
    auth_provider auth_provider_type NOT NULL DEFAULT 'local',

    -- Datos del perfil
    name VARCHAR(120) NOT NULL,
    lastname VARCHAR(120) NOT NULL,
    state state_type NOT NULL DEFAULT 'active',

    -- Banderas de seguridad
    email_confirmed BOOLEAN NOT NULL DEFAULT FALSE, 
    failed_login_attempts INT NOT NULL DEFAULT 0, -- Para bloquear por fuerza brita
    is_locked BOOLEAN NOT NULL DEFAULT FALSE,
	lockout_end TIMESTAMPTZ NULL,
    
    -- Auditoria
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NULL, -- UUID del usuario creador
    updated_at TIMESTAMPTZ NULL,
    updated_by UUID NULL -- UUID del usuario que modifica
);
COMMENT ON TABLE users IS 'Tabla central de identidades. Almacena credenciales y estado de seguridad.';
CREATE INDEX "IX_users_normalized_email" ON users(normalized_email);


CREATE TABLE user_applications (
    user_id UUID NOT NULL,
    application_id UUID NOT NULL,
    role_id UUID NOT NULL, -- El rol se asignara por aplicación
    status state_type NOT NULL DEFAULT 'active',
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    assigned_by UUID NULL,

    PRIMARY KEY (user_id, application_id),
    
    CONSTRAINT fk_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE,
        
    CONSTRAINT fk_application FOREIGN KEY (application_id) 
        REFERENCES applications(id) ON DELETE CASCADE,
        
    CONSTRAINT fk_role FOREIGN KEY (role_id) 
        REFERENCES roles(id) ON DELETE RESTRICT
);
-- Índice para búsquedas invertidas (de aplicación hacia usuarios)
CREATE INDEX idx_user_applications_app ON user_applications(application_id);
COMMENT ON TABLE user_applications IS 'Define a qué proyectos tiene acceso un usuario y qué rol tiene en cada uno.';


CREATE TABLE user_refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    application_id UUID NOT NULL, -- ◄ El nuevo campo para segmentar por aplicación
    token TEXT NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMPTZ NULL,

    -- Relaciones / Llaves Foráneas
    CONSTRAINT fk_refresh_token_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE,
        
    CONSTRAINT fk_refresh_token_application FOREIGN KEY (application_id) 
        REFERENCES applications(id) ON DELETE CASCADE
);
-- Comentario descriptivo para mantener documentada la base de datos
COMMENT ON TABLE user_refresh_tokens IS 'Almacena los tokens de actualización vinculados por usuario y aplicación cliente.';

-- Índices recomendados para que las búsquedas de tokens sean instantáneas
CREATE UNIQUE INDEX idx_user_refresh_tokens_token ON user_refresh_tokens(token);
CREATE INDEX idx_user_refresh_tokens_user_app ON user_refresh_tokens(user_id, application_id);


CREATE TYPE log_severity AS ENUM('info', 'warning', 'error', 'critical');

CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NULL, -- El usuario que hizo la acción (Puede ser NULL si fue un intento fallido de alguien que no existe)
    application_id UUID NULL, -- La aplicación donde ocurrió el evento (NULL si fue un evento global)
    action VARCHAR(100) NOT NULL, -- Qué pasó exactamente (Ej: 'login_success', 'login_failed', 'password_changed', 'user_locked')
    severity log_severity NOT NULL DEFAULT 'info',
    details JSONB NULL, -- Guardamos archivo JSON de la solicitud
    
    -- Datos de Red (Vitales en ciberseguridad)
    ip_address INET NULL, -- IP del solicitante (IPv4 o IPv6)
    user_agent TEXT NULL, -- El navegador o dispositivo (Ej: 'Mozilla/5.0... Safari')
    
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Relaciones (Opcionales, pero recomendadas para mantener integridad)
    CONSTRAINT fk_audit_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
    CONSTRAINT fk_audit_app FOREIGN KEY (application_id) REFERENCES applications(id) ON DELETE SET NULL
);

-- Índices cruciales para cuando tengas millones de logs y necesites buscar rápido
CREATE INDEX "IX_audit_logs_user_id" ON audit_logs(user_id);
CREATE INDEX "IX_audit_logs_action" ON audit_logs(action);
CREATE INDEX "IX_audit_logs_created_at" ON audit_logs(created_at);
-- Índice GIN: Este es avanzado. Permite buscar súper rápido DENTRO del JSON.
CREATE INDEX "IX_audit_logs_details" ON audit_logs USING GIN (details);

COMMENT ON TABLE audit_logs IS 'Rastro de auditoría inmutable para eventos de seguridad y negocio.';