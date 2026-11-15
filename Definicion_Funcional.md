# Dike_Identity
# Documento de Análisis Funcional

| Información | Detalle |
|---|---|
| **Proyecto** | Dike_Identity |
| **Documento** | Análisis Funcional |
| **Versión** | 1.0 |
| **Estado** | Definición funcional inicial |
| **Etapa** | Etapa 1 - Gestión de identidades y acceso a aplicaciones |

---

## 1. Introducción

### 1.1. Descripción general

Dike_Identity es una plataforma centralizada de gestión de identidades y autenticación, cuyo propósito es administrar los usuarios y controlar el acceso a diferentes aplicaciones pertenecientes a un mismo ecosistema de proyectos.

Las aplicaciones clientes conservarán sus propias interfaces de inicio de sesión, pero delegarán el proceso de autenticación en Dike_Identity mediante sus servicios API.

La plataforma será responsable de centralizar la administración de usuarios, aplicaciones, asignaciones de acceso, autenticación y gestión de credenciales.

### 1.2. Problema identificado

Al desarrollar diferentes aplicaciones, surge la necesidad de administrar independientemente los usuarios y mecanismos de autenticación de cada proyecto.

Esta situación puede ocasionar:

- Duplicación de registros de usuarios.
- Administración independiente de credenciales.
- Repetición de funcionalidades de autenticación.
- Dificultad para mantener una gestión centralizada de accesos.
- Falta de uniformidad en los mecanismos de seguridad.

Dike_Identity busca solucionar estos problemas mediante una plataforma centralizada que permita reutilizar los servicios de identidad entre diferentes aplicaciones.

### 1.3. Objetivo general

Desarrollar una plataforma que permita centralizar la gestión de identidades y autenticación para diferentes aplicaciones, proporcionando mecanismos de administración de usuarios, control de acceso y consulta de información de perfil.

### 1.4. Objetivos específicos

1. Administrar usuarios desde un único sistema.
2. Administrar las aplicaciones que utilizan el servicio de identidad.
3. Controlar el acceso de los usuarios a cada aplicación.
4. Proporcionar mecanismos centralizados de autenticación.
5. Gestionar credenciales de acceso y renovación.
6. Proporcionar información del perfil del usuario autenticado.
7. Registrar eventos relacionados con la autenticación y los accesos.
8. Facilitar la integración de nuevas aplicaciones al ecosistema.

---

## 2. Alcance del sistema

### 2.1. Alcance general

Dike_Identity permitirá administrar centralizadamente los usuarios y aplicaciones del ecosistema.

Cada aplicación cliente conservará su propio formulario de inicio de sesión y será responsable de recopilar las credenciales del usuario. Su backend deberá comunicarse con Dike_Identity mediante los servicios API disponibles para solicitar la autenticación y validar las condiciones de acceso.

Dike_Identity será responsable de validar las credenciales, el estado del usuario, el estado de la aplicación solicitante y la asignación de acceso correspondiente.

La aplicación cliente será responsable de gestionar las operaciones propias de su negocio, mientras que Dike_Identity centralizará la gestión de identidades y autenticación.

El acceso a cada aplicación será independiente y deberá ser asignado previamente por un administrador.


### 2.2. Actores del sistema

| Actor | Descripción | Responsabilidades |
|---|---|---|
| Administrador | Responsable de la administración de Dike_Identity. | Gestionar usuarios, aplicaciones y asignaciones de acceso. |
| Usuario | Persona que utiliza una o varias aplicaciones del ecosistema. | Autenticarse y consultar su información de perfil. |
| Aplicación cliente | Sistema externo que consume los servicios de Dike_Identity. | Solicitar autenticación y consumir información del usuario. |
| Dike_Identity | Plataforma central de identidad. | Administrar identidades, autenticación y acceso a aplicaciones. |

---

## 3. Decisiones arquitectónicas

Esta sección documenta las decisiones arquitectónicas que establecen restricciones o criterios de diseño para Dike_Identity y las aplicaciones que consumirán sus servicios.

### DA-01. Formulario de autenticación por aplicación

Cada aplicación cliente deberá conservar su propio formulario de inicio de sesión y delegar la validación de credenciales a Dike_Identity mediante su backend.

Dike_Identity será responsable de validar las credenciales y las condiciones de acceso del usuario, mientras que cada aplicación conservará el control de su propia interfaz de autenticación y lógica de negocio.

### DA-02. Endpoint de notificaciones por aplicación

Cada aplicación cliente deberá proporcionar una dirección de backend destinada a recibir las notificaciones enviadas por Dike_Identity.

Dike_Identity utilizará dicha dirección para comunicar cambios relevantes en el estado de la aplicación y otros acontecimientos que requieran coordinación con su backend.

La disponibilidad del endpoint de notificaciones no deberá condicionar la ejecución de las operaciones administrativas en Dike_Identity.

## 4. Reglas de negocio

Las reglas de negocio que determinan las condiciones y restricciones del dominio de Dike_Identity se encuentran definidas en el documento `Analisis_Dominio.md`, específicamente en la sección 05. Reglas de negocio.

Los requerimientos funcionales descritos en este documento deberán cumplir con las reglas allí establecidas.

Cuando corresponda, cada requerimiento funcional podrá hacer referencia a los identificadores de las reglas de negocio aplicables, evitando duplicar sus definiciones.

---

## 5. Requerimientos funcionales

### RF-01. Gestión de usuarios

El sistema permitirá al administrador gestionar los usuarios registrados en Dike_Identity, incluyendo su creación, consulta, actualización y desactivación, manteniendo la integridad de la información y los registros históricos asociados a cada usuario.

**Funcionalidades:**

- Registrar usuarios.
- Consultar usuarios registrados.
- Consultar información individual de un usuario.
- Actualizar información de usuarios.
- Desactivar usuarios.
- Consultar el estado de una cuenta.

**Reglas de negocio y consideraciones:**

1. El registro, actualización y desactivación de usuarios serán operaciones exclusivas del administrador.
2. Cada usuario deberá contar con un identificador único dentro del sistema.
3. El sistema deberá validar los datos obligatorios y las restricciones de unicidad antes de registrar un usuario.
4. No se permitirá registrar usuarios con identificadores o atributos únicos que ya se encuentren asociados a otro usuario.
5. La desactivación de un usuario no implicará la eliminación física de su registro ni de la información histórica asociada.
6. Un usuario desactivado no podrá iniciar nuevas autenticaciones en las aplicaciones clientes.
7. La desactivación de un usuario deberá impedir la renovación de sus credenciales de acceso.
8. Los tokens de acceso emitidos previamente podrán utilizarse hasta alcanzar su fecha de expiración, de acuerdo con la política de vigencia establecida para Dike_Identity.
9. El sistema deberá conservar el estado de la cuenta para permitir identificar si un usuario se encuentra activo o desactivado.

**Restricciones:**
- Solamente un administrador autenticado y autorizado podrá ejecutar operaciones de gestión de usuarios.
- No se permitirá eliminar fisicamente un usuario mediante las operaciones habituales de administración.
- Las operaciones de gestión deberán mantener la integridad de las relaciones existentes entre usuario y aplicaciones.

**Resultado esperado:**

El sistema deberá permitir administrar el ciclo de vida de los usuarios, controlar su estado y conservar su información histórica, evitando que las cuentas desactivadas puedan generar nuevas autenticaciones o renovar credenciales de acceso.


### RF-02. Gestión de aplicaciones

El sistema permitirá administrar las aplicaciones que utilizarán los servicios de Dike_Identity.

**Funcionalidades:**

- Registrar aplicaciones.
- Consultar aplicaciones registradas.
- Consultar información individual de una aplicación.
- Actualizar información de aplicaciones.
- Desactivar aplicaciones.
- Consultar el estado de cada aplicación.

**Consideraciones:**

- Solamente el administrador podrá gestionar las aplicaciones.
- Una aplicación deberá encontrarse registrada y activa para utilizar los servicios de autenticación.
- La desactivación de una aplicación no eliminará sus registros históricos.
- Una aplicación desactivada no podrá realizar nuevas autenticaciones.

### RF-03. Asignación de aplicaciones a usuarios

El sistema permitirá controlar a qué aplicaciones puede acceder cada usuario.

**Funcionalidades:**

- Asignar una aplicación a un usuario.
- Consultar las aplicaciones asignadas a un usuario.
- Consultar los usuarios asociados a una aplicación.
- Retirar la asignación de una aplicación a un usuario.

**Consideraciones:**

- Un usuario podrá tener acceso a múltiples aplicaciones.
- Una aplicación podrá tener múltiples usuarios.
- La asignación será administrada exclusivamente por el administrador.
- El usuario solamente podrá autenticarse en aplicaciones a las que tenga acceso.
- La asignación de acceso no implicará automáticamente la asignación de roles o permisos específicos.

### RF-04. Autenticación

El sistema permitirá que las aplicaciones registradas soliciten la autenticación de sus usuarios.

**Funcionalidades:**

- Recibir solicitudes de autenticación.
- Validar las credenciales del usuario.
- Validar el estado de la cuenta.
- Validar el estado de la aplicación solicitante.
- Validar la asignación de acceso del usuario a la aplicación.
- Retornar el resultado de la autenticación.

**Consideraciones:**

La autenticación solamente será exitosa cuando:

1. La aplicación solicitante se encuentre registrada y activa.
2. El usuario exista en Dike_Identity.
3. La cuenta del usuario se encuentre activa.
4. Las credenciales proporcionadas sean válidas.
5. El usuario tenga asignado el acceso a la aplicación solicitante.

### RF-05. Gestión de tokens

El sistema permitirá gestionar las credenciales utilizadas para mantener el acceso autenticado.

**Funcionalidades:**

- Emitir tokens de acceso.
- Gestionar credenciales de renovación.
- Renovar credenciales de acceso.
- Controlar la expiración de tokens.
- Validar la vigencia de las credenciales.

**Consideraciones:**

- Los tokens deberán estar asociados a un contexto de autenticación válido.
- Un token expirado no deberá permitir el acceso.
- La renovación deberá considerar el estado actual del usuario y de la aplicación.
- Los mecanismos de revocación y cierre de sesiones deberán especificarse detalladamente.

### RF-06. Información del perfil

El sistema proporcionará información del usuario autenticado a las aplicaciones clientes.

**Funcionalidades:**

- Consultar información del perfil.
- Identificar al usuario autenticado.
- Proporcionar los datos de perfil autorizados para su consumo.

**Consideraciones:**

- La información proporcionada corresponderá al usuario autenticado.
- Las aplicaciones podrán utilizar esta información para mostrar datos del usuario dentro de sus interfaces.

### RF-07. Registro de eventos y auditoría

El sistema registrará eventos relevantes relacionados con la autenticación y la administración.

**Eventos iniciales considerados:**

- Inicios de sesión exitosos.
- Intentos fallidos de autenticación.
- Renovaciones de tokens.
- Desactivaciones de usuarios.
- Desactivaciones de aplicaciones.
- Cambios en las asignaciones de acceso.

**Consideraciones:**

- Los registros deberán permitir identificar el evento y su fecha de ocurrencia.
- Los eventos administrativos deberán permitir identificar al administrador responsable.
- Los registros históricos deberán conservarse aunque se desactive un usuario o aplicación.

---

## 6. Requerimientos no funcionales

| Código | Requerimiento | Descripción |
|---|---|---|
| RNF-01 | Seguridad | Proteger el almacenamiento y tratamiento de credenciales. |
| RNF-02 | Autenticación | Proteger los mecanismos de autenticación y emisión de tokens. |
| RNF-03 | Disponibilidad | Proporcionar disponibilidad del servicio para las aplicaciones clientes. |
| RNF-04 | Integridad | Mantener la integridad de los registros de usuarios y aplicaciones. |
| RNF-05 | Trazabilidad | Registrar eventos administrativos y de autenticación. |
| RNF-06 | Integración | Permitir la integración con diferentes aplicaciones clientes. |
| RNF-07 | Mantenibilidad | Facilitar la evolución y mantenimiento del sistema. |

---

## 7. Alcance del MVP

La primera versión de Dike_Identity estará enfocada en resolver la gestión centralizada de identidades y el acceso a aplicaciones.

### 7.1. Funcionalidades incluidas

| Módulo | Funcionalidades |
|---|---|
| Usuarios | Registro, consulta, actualización y desactivación. |
| Aplicaciones | Registro, consulta, actualización y desactivación. |
| Accesos | Asignación y retiro de aplicaciones a usuarios. |
| Autenticación | Validación centralizada de credenciales. |
| Tokens | Emisión, renovación y expiración. |
| Perfil | Consulta de información del usuario autenticado. |
| Auditoría | Registro de eventos relevantes. |

### 7.2. Funcionalidades excluidas

Las siguientes funcionalidades no formarán parte de la primera etapa:

- Gestión de roles.
- Gestión de permisos específicos.
- Asignación de permisos por aplicación.
- Autorización granular sobre funcionalidades internas.
- Single Sign-On (SSO).
- Registro público de usuarios.
- Administración autónoma de cuentas por parte de los usuarios.

Estas funcionalidades podrán incorporarse en etapas posteriores.

---

## 8. Criterios de aceptación

Los siguientes escenarios permitirán validar el comportamiento funcional del MVP.

| Código | Escenario | Resultado esperado |
|---|---|---|
| CA-01 | Registrar un usuario. | El usuario queda registrado y disponible en el sistema. |
| CA-02 | Actualizar un usuario. | La información del usuario queda actualizada. |
| CA-03 | Desactivar un usuario. | El usuario no puede iniciar nuevas autenticaciones. |
| CA-04 | Registrar una aplicación. | La aplicación queda registrada y habilitada según su estado. |
| CA-05 | Actualizar una aplicación. | La información de la aplicación queda actualizada. |
| CA-06 | Desactivar una aplicación. | Se impiden nuevas autenticaciones y se conservan los registros históricos. |
| CA-07 | Asignar una aplicación a un usuario. | El usuario obtiene autorización para autenticarse en dicha aplicación. |
| CA-08 | Retirar una asignación. | El usuario pierde el acceso a la aplicación correspondiente. |
| CA-09 | Autenticarse sin asignación. | El sistema rechaza la autenticación. |
| CA-10 | Autenticarse con credenciales incorrectas. | El sistema rechaza la autenticación. |
| CA-11 | Consultar el perfil. | Se devuelve la información del usuario autenticado. |
| CA-12 | Renovar credenciales. | Se valida la vigencia y elegibilidad de la renovación. |
| CA-13 | Utilizar un token expirado. | El sistema rechaza el acceso. |
| CA-14 | Consultar auditoría. | Se pueden identificar los eventos registrados. |

---

## 9. Decisiones pendientes de especificación

Los siguientes aspectos requieren mayor análisis antes de definir completamente los requerimientos detallados.

| Área | Aspecto pendiente |
|---|---|
| Usuarios | Definición de campos obligatorios y opcionales. |
| Identificación | Determinar si el correo electrónico será único y obligatorio. |
| Contraseñas | Definir los procesos de recuperación y cambio de contraseña. |
| Aplicaciones | Determinar los datos necesarios para registrar una aplicación. |
| Credenciales | Definir duración de tokens de acceso y renovación. |
| Sesiones | Definir mecanismos de revocación inmediata de tokens existentes. |
| Desactivaciones | Definir el comportamiento de tokens existentes cuando se desactive un usuario. |
| Auditoría | Definir eventos definitivos, información registrada y periodo de conservación. |
| Administración | Definir la protección y recuperación de la cuenta administradora. |

---

## 10. Consideraciones para futuras etapas

### Etapa 2 - Gestión de roles y permisos

Se incorporará un sistema de autorización que permita administrar los privilegios de los usuarios de manera independiente por aplicación.

Se contemplan inicialmente:

- Registro y administración de roles.
- Registro y administración de permisos.
- Asignación de roles a usuarios por aplicación.
- Asociación de permisos a roles.
- Control de acceso basado en roles y permisos.
- Validación de permisos para funcionalidades específicas.

### Etapa 3 - Evolución y seguridad

Se evaluarán nuevas funcionalidades de acuerdo con las necesidades del ecosistema:

- Gestión avanzada de sesiones.
- Políticas de seguridad de cuentas.
- Mejoras de auditoría.
- Mecanismos adicionales de autenticación.
- Otras capacidades de gestión de identidad y acceso.

---

## 11. Conclusión

Dike_Identity busca proporcionar una solución centralizada para la administración de identidades y autenticación de diferentes aplicaciones.

La primera etapa estará enfocada en establecer las funcionalidades fundamentales para gestionar usuarios, aplicaciones y asignaciones de acceso, además de proporcionar servicios de autenticación, gestión de tokens, información de perfil y auditoría.

La segunda etapa ampliará las capacidades del sistema incorporando una gestión independiente de roles y permisos por aplicación.

Esta separación permitirá desarrollar el producto progresivamente, manteniendo delimitadas las responsabilidades funcionales de cada etapa y evitando incorporar complejidad innecesaria desde su primera versión.

---

**Fin del documento - Dike_Identity v1.0**