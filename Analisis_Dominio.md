# Análisis del dominio

## 01. Introducción

### 1.1. Propósito del documento

El presente documento tiene como propósito analizar y describir el dominio de Dike Identity, identificando el problema que busca resolver, los conceptos principales del negocio, sus relaciones, las reglas que lo gobiernan y los procesos involucrados.

Este análisis servirá como base para comprender el funcionamiento del negocio y establecer los fundamentos necesarios para las siguientes etapas de análisis y diseño del sistema.

### 1.2. Alcance del análisis

El análisis comprende la identificación y descripción de los elementos que conforman el dominio de Dike Identity, incluyendo sus conceptos principales, relaciones, reglas de negocio, procesos y límites de responsabilidad frente a las aplicaciones externas.

El análisis se centra en la gestión centralizada de identidades, la autenticación de usuarios y la administración de sus accesos a las aplicaciones del ecosistema.

### 1.3. Contexto del negocio

Actualmente, cada aplicación del ecosistema cuenta con su propio sistema de gestión de usuarios y autenticación. Esta situación genera duplicidad de información, inconsistencias en los datos y dificultades para mantener actualizada la información de los usuarios.

Ante esta situación, surge la necesidad de centralizar la gestión de identidades y autenticación mediante una plataforma común que permita administrar los usuarios y controlar sus accesos a las diferentes aplicaciones.

## 02. Descripción del dominio

### 2.1. Problema que se busca resolver

Actualmente, cada aplicación cuenta con su propio sistema de gestión de usuarios, lo que provoca duplicidad de información, inconsistencias en los datos y dificultades para mantener actualizada la información de los usuarios.

Esta situación dificulta la administración centralizada de las identidades y el control de acceso a las diferentes aplicaciones.

### 2.2. Objetivos del sistema

Dike Identity tiene como propósito proporcionar un sistema centralizado de gestión de identidades, autenticación y control de acceso a las aplicaciones.

Permitirá a los administradores gestionar los usuarios desde un único sistema, mantener la información centralizada y consistente, controlar qué aplicaciones pueden utilizar los usuarios y conservar los registros históricos asociados a su gestión.

### 2.3. Actores involucrados

Los actores involucrados representan a las personas y sistemas externos que interactúan con Dike Identity para cumplir sus respectivos objetivos.

| Actor | Descripción | Responsabilidades |
|---|---|---|
| Administrador | Persona responsable de la administración de Dike Identity. | Gestionar usuarios, aplicaciones y asignaciones de acceso. |
| Usuario | Persona que utiliza una o varias aplicaciones del ecosistema. | Autenticarse mediante Dike Identity y consultar su información de perfil. |
| Aplicación cliente | Sistema externo que consume los servicios proporcionados por Dike Identity. | Solicitar la autenticación de usuarios y consumir la información de identidad necesaria para su funcionamiento. |

**Nota:** Dike Identity no se considera un actor, ya que es el sistema cuyo dominio estamos analizando. Los actores son las entidades externas que interactúan con él.

## 03. Conceptos principales

| Concepto | Descripción |
|---|---|
| Usuario | Persona cuya identidad es administrada centralmente por Dike Identity. |
| Aplicación | Sistema que forma parte del ecosistema y que consume los servicios de Dike Identity. |
| Asignación de acceso | Relación que determina si un usuario tiene acceso habilitado a una aplicación. |
| Credencial | Información utilizada para verificar la identidad de un usuario durante la autenticación. |
| Autenticación | Proceso mediante el cual se verifica la identidad de un usuario. |
| Token | Credencial temporal emitida después de un proceso de autenticación exitoso, asociada a una sesión y contexto de acceso. |
| Perfil de usuario | Conjunto de información que permite identificar y describir al usuario. |
| Administrador | Persona responsable de gestionar los usuarios, aplicaciones y asignaciones de acceso. |
| Sesión | Contexto de autenticación establecido cuando un usuario completa satisfactoriamente un proceso de autenticación en Dike Identity. |
| Auditoría | Registro histórico de las acciones y eventos relevantes que ocurren dentro de Dike Identity. |

### Usuario

**Definición:**

Representa la identidad de una persona registrada y administrada por Dike Identity, independientemente de que tenga credenciales configuradas o acceso asignado a alguna aplicación.

**Responsabilidad:**

Representar y mantener la identidad centralizada de una persona, permitiendo administrar su información, estado y registros históricos.

**Características relevantes:**

- Una persona tiene una identidad centralizada en Dike Identity.
- Puede existir sin tener acceso asignado a ninguna aplicación.
- Puede tener acceso a una o varias aplicaciones.
- Su estado es independiente del estado de sus asignaciones de acceso.
- Su información y registros históricos se conservan aunque sea desactivada.

### Aplicación

**Definición:**

Representa un sistema externo perteneciente al ecosistema que consume los servicios proporcionados por Dike Identity para gestionar la autenticación de sus usuarios y validar sus accesos.

**Responsabilidad:**

Identificarse como un consumidor autorizado de los servicios de Dike Identity y mantener su propio estado dentro de la plataforma.

**Características relevantes:**

- Debe estar registrada en Dike Identity para consumir sus servicios.
- Tiene un estado propio, independiente del estado de los usuarios asignados.
- Puede tener uno o varios usuarios con acceso asignado.
- Su desactivación impide realizar nuevas autenticaciones y operaciones de acceso mediante Dike Identity.
- Su desactivación no elimina los usuarios ni sus asignaciones históricas.
- Puede tener configurada una dirección de backend para recibir notificaciones de Dike Identity sobre cambios relevantes en su estado.

### Asignación de acceso

**Definición:**

Representa la relación entre un usuario y una aplicación, mediante la cual se establece si dicho usuario tiene habilitado el acceso a esa aplicación.

**Responsabilidad:**

Administrar el acceso de un usuario a una aplicación de manera independiente al estado de su identidad y al estado de la aplicación.

**Características relevantes:**

- Un usuario puede tener asignaciones de acceso a varias aplicaciones.
- Una aplicación puede tener asignaciones de acceso de varios usuarios.
- Cada asignación tiene un estado propio que determina si el acceso está habilitado o deshabilitado.
- Un usuario solamente puede tener una asignación por aplicación.
- La desactivación de una asignación no elimina la relación entre el usuario y la aplicación.
- Si se requiere habilitar nuevamente el acceso, se actualizará la asignación existente.
- Se debe conservar la información histórica de las asignaciones.
- Las modificaciones de una asignación deben permitir identificar quién realizó el cambio y cuándo ocurrió.

### Mecanismo de autenticación

**Definición:**

Representa el método mediante el cual un usuario puede demostrar su identidad ante Dike Identity, utilizando credenciales propias o servicios de proveedores externos de identidad.

**Responsabilidad:**

Proporcionar los medios necesarios para verificar la identidad de un usuario durante los procesos de autenticación.

**Características relevantes:**

- Un usuario puede disponer de uno o varios mecanismos de autenticación.
- Inicialmente, Dike Identity utilizará autenticación mediante correo electrónico y contraseña.
- Se podrán incorporar proveedores externos de identidad, como Google.
- Los diferentes mecanismos de autenticación deben estar vinculados a una misma identidad centralizada.
- La vinculación de una cuenta de Google será realizada exclusivamente por un administrador durante el MVP.
- El correo electrónico verificado por Google deberá coincidir con el correo registrado en Dike Identity.
- La vinculación de Google no deshabilitará el mecanismo de autenticación mediante contraseña.

### Token

**Definición:**

Representa una credencial temporal emitida por Dike Identity después de una autenticación exitosa, que permite a las aplicaciones consumidoras identificar al usuario autenticado y validar su contexto de acceso.

**Responsabilidad:**

Permitir que las aplicaciones consumidoras comprueben las condiciones necesarias para aceptar el acceso de un usuario.

**Características relevantes:**

- Es emitido y administrado por Dike Identity.
- Tiene un período de vigencia determinado.
- Puede ser revocado antes de su expiración.
- Está asociado a una sesión y al contexto de acceso correspondiente.
- Su aceptación depende de que el usuario, la aplicación y la asignación de acceso continúen habilitados.
- Debe dejar de ser válido cuando se desactive el usuario, la aplicación o su asignación de acceso.
- La renovación genera un nuevo token y el anterior deja de ser válido.

### Perfil de usuario

**Definición:**

Representa el conjunto de información personal y de identidad asociada a un usuario, administrada centralmente por Dike Identity.

**Responsabilidad:**

Proporcionar información centralizada y consistente sobre la identidad de un usuario a las aplicaciones consumidoras que estén autorizadas para consultarla.

**Características relevantes:**

- Contiene información propia de la identidad del usuario.
- Su información es administrada centralmente por Dike Identity.
- Puede ser consultado por las aplicaciones consumidoras mediante los servicios proporcionados por Dike Identity.
- No incluye información específica del negocio de cada aplicación.
- No requiere exponer al usuario final las aplicaciones a las que tiene acceso.

### Administrador de Dike Identity

**Definición:**

Representa a una persona autorizada para administrar la plataforma Dike Identity.

**Responsabilidad:**

Gestionar las identidades de los usuarios, las aplicaciones registradas y las asignaciones de acceso.

**Características relevantes:**

- Dispone de una cuenta administrativa independiente de las cuentas de usuario de las aplicaciones.
- Tiene sus propios mecanismos de autenticación.
- Su estado es independiente del estado de las cuentas de usuario de las aplicaciones.
- Su desactivación no afecta las identidades ni los accesos que pueda tener como usuario de las aplicaciones.
- Durante el MVP, es el único responsable de vincular proveedores externos de identidad a las cuentas de usuario.

### Auditoría

**Definición:**

Representa el registro histórico de las actividades y acontecimientos relevantes que ocurren dentro de Dike Identity, permitiendo conocer qué acciones se realizaron, quién las ejecutó y cuándo ocurrieron.

**Responsabilidad:**

Mantener la trazabilidad de las operaciones administrativas, los cambios en las identidades, la gestión de aplicaciones, las asignaciones de acceso y los acontecimientos relacionados con la autenticación y las sesiones.

**Características relevantes:**

- Registra las acciones relevantes realizadas por los administradores.
- Registra los acontecimientos relacionados con la autenticación y las sesiones.
- Registra acontecimientos relevantes generados por aplicaciones y procesos internos.
- Permite identificar al responsable de una operación cuando corresponda.
- Conserva la fecha y hora de los acontecimientos.
- Permite consultar el historial de las operaciones.
- Los registros históricos no deben modificarse ni eliminarse mediante las operaciones administrativas habituales.

### Sesión

**Definición:**

Representa el contexto de autenticación establecido cuando un usuario o administrador completa satisfactoriamente un proceso de autenticación en Dike Identity.

**Responsabilidad:**

Mantener el estado y el ciclo de vida de una autenticación, permitiendo gestionar su continuidad, renovación y finalización.

**Características relevantes:**

- Se establece después de una autenticación exitosa.
- Está asociada a una identidad y al contexto de acceso correspondiente.
- Puede estar relacionada con uno o varios tokens durante su ciclo de vida.
- Permite gestionar la renovación de tokens.
- Puede finalizar mediante un cierre de sesión o una revocación.
- Su estado debe permitir determinar si continúa siendo válida.
- Sus acontecimientos relevantes deben quedar registrados en la auditoría.
- Cada cuenta de usuario podrá mantener una sola sesión activa simultáneamente.
- Cada cuenta administrativa podrá mantener una sola sesión administrativa activa simultáneamente.
- Las sesiones administrativas y las sesiones de usuario son independientes entre sí.

## 04. Relaciones entre conceptos

Las relaciones entre conceptos permiten identificar cómo se vinculan los elementos principales del dominio de Dike Identity, estableciendo sus cardinalidades y las restricciones que deben respetarse.

### 4.1. Usuario y Asignación de acceso

Un usuario puede tener asignaciones de acceso a varias aplicaciones. Cada asignación pertenece a un único usuario.

**Cardinalidad:** Uno a muchos (1:N).

**Características de la relación:**

- Un usuario puede tener cero o varias asignaciones de acceso.
- Cada asignación de acceso pertenece a un único usuario.
- La desactivación de un usuario no elimina sus asignaciones.
- Las asignaciones conservan su propio estado, independientemente del estado del usuario.

### 4.2. Aplicación y Asignación de acceso

Una aplicación puede tener asignaciones de acceso correspondientes a varios usuarios. Cada asignación pertenece a una única aplicación.

**Cardinalidad:** Uno a muchos (1:N).

**Características de la relación:**

- Una aplicación puede tener cero o varias asignaciones de acceso.
- Cada asignación de acceso pertenece a una única aplicación.
- La desactivación de una aplicación no elimina las asignaciones existentes.
- El estado de la aplicación es independiente del estado de sus asignaciones.

**Restricción de unicidad entre Usuario y Aplicación:**

Un usuario solo puede tener una asignación de acceso por aplicación.

La desactivación de una asignación no elimina la relación entre el usuario y la aplicación. Si posteriormente se requiere habilitar nuevamente el acceso, se deberá actualizar la asignación existente, conservando su historial de modificaciones.

### 4.3. Usuario y Mecanismo de autenticación

Un usuario puede disponer de varios mecanismos de autenticación asociados a una misma identidad. Cada mecanismo pertenece a un único usuario.

**Cardinalidad:** Uno a muchos (1:N).

**Características de la relación:**

- Un usuario puede tener uno o varios mecanismos de autenticación.
- Cada mecanismo de autenticación está asociado a un único usuario.
- Inicialmente, Dike Identity permitirá la autenticación mediante correo electrónico y contraseña.
- Se podrán incorporar proveedores externos de identidad, como Google.
- Un usuario podrá vincular su cuenta de Google si el correo electrónico verificado por dicho proveedor coincide con el correo registrado en Dike Identity.
- La vinculación de un proveedor externo no reemplaza ni deshabilita los mecanismos de autenticación existentes.
- Todos los mecanismos asociados permiten acceder a una misma identidad centralizada.
- Durante el MVP, la vinculación será realizada exclusivamente por un administrador.

### 4.4. Usuario y Sesión

Un usuario puede tener diferentes sesiones a lo largo del tiempo, pero solamente podrá mantener una sesión activa simultáneamente.

**Cardinalidad:** Uno a muchos (1:N), considerando el historial de sesiones.

**Características de la relación:**

- Un usuario puede tener cero o varias sesiones históricas.
- Cada sesión pertenece a un único usuario.
- Un usuario solo puede mantener una sesión activa simultáneamente.
- Si inicia una nueva sesión mientras existe otra activa, la sesión anterior deberá finalizarse antes de establecer la nueva.
- Las sesiones finalizadas o revocadas deberán conservarse como registros históricos.

### 4.5. Sesión y Token

Una sesión puede estar asociada a uno o varios tokens durante su ciclo de vida. Cada token pertenece a una única sesión.

**Cardinalidad:** Uno a muchos (1:N).

**Características de la relación:**

- Una sesión puede generar uno o varios tokens.
- Cada token está asociado a una única sesión.
- Cada token tiene un período de vigencia determinado.
- La renovación de un token genera uno nuevo asociado a la misma sesión.
- Los tokens anteriores deberán dejar de ser válidos cuando sean reemplazados.
- La finalización o revocación de una sesión deberá invalidar todos los tokens asociados a ella.

### 4.6. Administrador y Sesión

Un administrador puede tener diferentes sesiones a lo largo del tiempo, pero solamente podrá mantener una sesión administrativa activa simultáneamente.

**Cardinalidad:** Uno a muchos (1:N), considerando el historial de sesiones.

**Características de la relación:**

- Un administrador puede tener cero o varias sesiones administrativas históricas.
- Cada sesión administrativa pertenece a un único administrador.
- Un administrador solo puede mantener una sesión administrativa activa simultáneamente.
- Si inicia una nueva sesión administrativa mientras existe otra activa, la anterior deberá finalizarse antes de establecer la nueva.
- Las sesiones administrativas finalizadas o revocadas deberán conservarse como registros históricos.

**Independencia de las sesiones:**

Las sesiones administrativas y las sesiones de usuario de las aplicaciones son independientes entre sí.

Una persona que disponga de una cuenta administrativa y otra cuenta de usuario podrá mantener una sesión activa en cada contexto simultáneamente. La finalización o revocación de una sesión no deberá afectar a la otra.

### 4.7. Actores, procesos internos y Auditoría

Los acontecimientos relevantes generados por los actores y los procesos internos de Dike Identity deberán quedar registrados en la auditoría.

**Relación conceptual:** Un origen puede generar múltiples registros de auditoría (1:N).

**Características de la relación:**

- Los administradores pueden generar registros mediante sus operaciones administrativas.
- Los usuarios pueden generar registros mediante acontecimientos relacionados con su autenticación y sesiones.
- Las aplicaciones cliente pueden generar registros mediante las solicitudes realizadas a Dike Identity.
- Los procesos internos pueden generar registros como consecuencia de operaciones automáticas.
- Cada registro deberá permitir identificar su origen, fecha, hora, tipo de operación y resultado.
- Cuando el acontecimiento sea generado por una persona, deberá identificarse al responsable de la operación.
- Cuando sea generado automáticamente, deberá identificarse el proceso o componente que lo produjo.
- Los registros históricos no deberán modificarse ni eliminarse mediante las operaciones administrativas habituales.

### 4.8. Aplicación y Endpoint de notificaciones

Una aplicación puede tener configurada una dirección de backend destinada a recibir notificaciones de Dike Identity.

**Cardinalidad:** Una aplicación puede tener cero o un endpoint de notificaciones configurado (1:0..1).

**Características de la relación:**

- El endpoint pertenece a una única aplicación.
- Dike Identity utilizará el endpoint para notificar cambios relevantes en el estado de la aplicación.
- La disponibilidad del endpoint no condicionará la ejecución de las operaciones administrativas.
- Si una notificación no puede entregarse, deberá quedar pendiente de procesamiento.

### 4.9. Resumen de relaciones conceptuales

| Conceptos relacionados | Cardinalidad | Restricción principal |
|---|---|---|
| Usuario - Asignación de acceso | 1:N | Un usuario puede tener varias asignaciones. |
| Aplicación - Asignación de acceso | 1:N | Una aplicación puede tener varios usuarios asignados. |
| Usuario - Mecanismo de autenticación | 1:N | Una identidad puede disponer de varios mecanismos. |
| Usuario - Sesión | 1:N | Máximo una sesión activa por usuario. |
| Sesión - Token | 1:N | Cada renovación genera un token nuevo. |
| Administrador - Sesión | 1:N | Máximo una sesión administrativa activa. |
| Aplicación - Endpoint de notificaciones | 1:0..1 | Una aplicación puede tener un endpoint configurado. |
| Origen del acontecimiento - Auditoría | 1:N | Un actor o proceso puede generar múltiples registros. |

**Consideración general:**

Las sesiones administrativas y las sesiones de usuario corresponden a contextos independientes, debido a que las cuentas administrativas y las cuentas de usuario de las aplicaciones tienen ciclos de vida separados.

Las relaciones descritas representan el modelo conceptual del dominio. Las decisiones sobre tablas, claves, restricciones de base de datos y mecanismos de implementación serán abordadas durante las etapas de diseño.

## 05. Reglas de negocio

Las siguientes reglas establecen las condiciones que gobiernan la gestión de identidades, autenticación y acceso a las aplicaciones de Dike Identity.

### RN-01. Identidad centralizada

Cada persona deberá contar con una única identidad dentro de Dike Identity, independientemente de la cantidad de aplicaciones a las que tenga acceso o de los mecanismos de autenticación que utilice.

### RN-02. Acceso independiente por aplicación

La existencia de un usuario en Dike Identity no le concede automáticamente acceso a todas las aplicaciones registradas.

El acceso a cada aplicación deberá asignarse de manera individual.

### RN-03. Administración de asignaciones

Solamente un administrador autorizado podrá crear, desactivar o reactivar las asignaciones de acceso entre usuarios y aplicaciones.

### RN-04. Administración de usuarios

Solamente un administrador autorizado podrá registrar, actualizar y desactivar usuarios.

La desactivación de un usuario no implicará su eliminación física ni la eliminación de sus registros históricos.

### RN-05. Aplicaciones registradas

Una aplicación deberá encontrarse registrada y activa en Dike Identity para utilizar sus servicios de autenticación y validación de acceso.

### RN-06. Administración de aplicaciones

Solamente un administrador autorizado podrá registrar, actualizar, activar y desactivar aplicaciones.

### RN-07. Conservación de información histórica

La desactivación de un usuario, una aplicación o una asignación de acceso no deberá eliminar sus registros ni sus relaciones históricas.

### RN-08. Condiciones de autenticación

Una autenticación será exitosa únicamente cuando se cumplan todas las condiciones siguientes:

- La aplicación solicitante se encuentre registrada y activa.
- El usuario exista y se encuentre activo.
- Las credenciales proporcionadas sean válidas.
- El usuario tenga una asignación de acceso activa para la aplicación solicitante.

### RN-09. Independencia de estados

El estado de un usuario, una aplicación y una asignación de acceso son independientes entre sí.

Sin embargo, para permitir el acceso, los tres elementos deberán encontrarse habilitados.

### RN-10. Invalidación inmediata de tokens

La desactivación de un usuario, una aplicación o una asignación de acceso deberá impedir inmediatamente la aceptación de los tokens asociados al contexto afectado, sin depender de su fecha de expiración.

La desactivación de una asignación solamente afectará el acceso del usuario a la aplicación correspondiente. La desactivación global de un usuario afectará sus accesos a todas las aplicaciones.

### RN-11. Independencia de cuentas administrativas y de usuario

Las cuentas administrativas de Dike Identity y las cuentas de usuario de las aplicaciones son independientes, incluso cuando pertenezcan a una misma persona.

La desactivación de una cuenta no deberá afectar el estado, las sesiones ni los accesos de la otra.

### RN-12. Sesiones independientes

Cada cuenta de usuario podrá mantener una sola sesión activa simultáneamente. Cada cuenta administrativa también podrá mantener una sola sesión administrativa activa.

Las sesiones administrativas y las sesiones de usuario serán independientes entre sí.

Cuando se establezca una nueva sesión para una misma cuenta, la sesión activa anterior deberá finalizar antes de establecer la nueva.

### RN-13. Renovación de tokens

La renovación de un token deberá generar una nueva credencial asociada a la misma sesión, invalidando el token anterior.

La renovación solamente podrá realizarse si el contexto de acceso continúa siendo válido.

### RN-14. Vinculación de proveedores externos

Durante el MVP, solamente un administrador autorizado podrá vincular mecanismos de autenticación de proveedores externos a una identidad existente.

Para vincular una cuenta de Google, el correo electrónico verificado por el proveedor deberá coincidir con el correo registrado en Dike Identity.

La vinculación no deberá deshabilitar los mecanismos de autenticación existentes.

### RN-15. Auditoría de acontecimientos relevantes

Las operaciones administrativas y los acontecimientos relevantes de autenticación, sesiones y acceso deberán quedar registrados en la auditoría.

Cada registro deberá permitir identificar el tipo de acontecimiento, su fecha y hora, su resultado y el actor o proceso responsable cuando corresponda.

### RN-16. Notificación de cambios de estado

Cuando se modifique el estado de una aplicación, Dike Identity deberá notificar el cambio al endpoint configurado por dicha aplicación.

La falta de disponibilidad del endpoint no deberá impedir la operación administrativa. Las notificaciones que no puedan entregarse deberán quedar pendientes de procesamiento.

### RN-17. Independencia de roles y permisos

La asignación de acceso a una aplicación no implica la asignación de roles ni permisos específicos dentro de ella.

Cada aplicación conservará la responsabilidad de gestionar sus propios roles y permisos.

## 06. Procesos del negocio

Los procesos del negocio describen las operaciones principales mediante las cuales Dike Identity cumple su propósito de centralizar la gestión de identidades y controlar el acceso a las aplicaciones.

### 6.1. Registro de usuario

1. El administrador solicita registrar un nuevo usuario.
2. Dike Identity valida la información proporcionada.
3. Se verifica que no exista una identidad duplicada según los atributos únicos definidos.
4. Se registra la identidad del usuario.
5. Se registra el mecanismo de autenticación inicial cuando corresponda.
6. Se confirma el registro.

**Resultado:** El usuario queda registrado en Dike Identity y puede recibir asignaciones de acceso.

### 6.2. Asignación de acceso a una aplicación

1. El administrador selecciona un usuario y una aplicación.
2. Dike Identity verifica que ambos registros existan.
3. Se comprueba que no exista una asignación activa o inactiva previa para esa pareja.
4. Si no existe, se registra la asignación.
5. Si existe una asignación inactiva, se actualiza su estado a activo.
6. Se registra la operación en la auditoría.

**Resultado:** El usuario dispone de una asignación de acceso para la aplicación correspondiente.

### 6.3. Autenticación de usuario

1. El usuario ingresa sus credenciales en el formulario de la aplicación cliente.
2. El backend de la aplicación envía una solicitud de autenticación a Dike Identity.
3. Dike Identity identifica la aplicación solicitante.
4. Se validan las credenciales del usuario.
5. Se verifica el estado del usuario, la aplicación y la asignación de acceso.
6. Si todas las condiciones son válidas, se establece una sesión.
7. Dike Identity emite el token correspondiente.
8. Se registra el acontecimiento en la auditoría.
9. Se devuelve el resultado de autenticación a la aplicación.

**Resultado:** El usuario obtiene acceso a la aplicación solicitante si cumple las condiciones establecidas.

### 6.4. Renovación de token

1. La aplicación solicita la renovación de las credenciales de acceso.
2. Dike Identity verifica la validez de la credencial de renovación.
3. Se comprueba el estado de la sesión y del contexto de acceso.
4. Se verifica que el usuario, la aplicación y la asignación continúen habilitados.
5. Se genera un nuevo token.
6. Se invalida el token anterior.
7. Se registra la operación en la auditoría.

**Resultado:** La sesión continúa activa con una nueva credencial de acceso.

### 6.5. Desactivación de usuario

1. El administrador solicita desactivar un usuario.
2. Dike Identity verifica que el usuario exista.
3. Se cambia el estado del usuario a inactivo.
4. Se invalidan las sesiones y tokens asociados a sus accesos.
5. Se registra la operación en la auditoría.

**Resultado:** El usuario no puede iniciar nuevas autenticaciones ni continuar utilizando tokens previamente emitidos.

### 6.6. Desactivación de asignación de acceso

1. El administrador selecciona al usuario y la aplicación correspondiente.
2. Dike Identity identifica la asignación existente.
3. Se cambia el estado de la asignación a inactivo.
4. Se invalidan las sesiones y tokens asociados al acceso de ese usuario a esa aplicación.
5. Se registra la operación en la auditoría.

**Resultado:** El usuario pierde el acceso a la aplicación correspondiente, sin afectar sus accesos a otras aplicaciones.

### 6.7. Desactivación de aplicación

1. El administrador solicita desactivar una aplicación.
2. Dike Identity verifica que la aplicación exista.
3. Se cambia el estado de la aplicación a inactivo.
4. Se invalidan las sesiones y tokens asociados a esa aplicación.
5. Se registra la operación en la auditoría.
6. Dike Identity intenta notificar el cambio al endpoint configurado.
7. Si la notificación no puede entregarse, queda pendiente de procesamiento.

**Resultado:** La aplicación deja de poder utilizar los servicios de autenticación y validación de acceso de Dike Identity. Su backend deberá impedir la ejecución de operaciones mientras permanezca desactivada.

## 07. Límites de responsabilidad

Dike Identity centraliza la gestión de identidades y autenticación, pero no asume la responsabilidad de administrar las funcionalidades internas de cada aplicación.

La separación de responsabilidades permite mantener una plataforma de identidad independiente de las reglas particulares de cada sistema.

### 7.1. Responsabilidades de Dike Identity

- Registrar y administrar las identidades de los usuarios.
- Administrar los mecanismos de autenticación.
- Administrar las aplicaciones registradas.
- Gestionar las asignaciones de acceso.
- Validar las credenciales de los usuarios.
- Verificar las condiciones de acceso a cada aplicación.
- Gestionar sesiones y tokens.
- Proporcionar información del perfil de usuario.
- Registrar acontecimientos relevantes en la auditoría.
- Notificar cambios de estado a las aplicaciones mediante sus endpoints configurados.

### 7.2. Responsabilidades de las aplicaciones cliente

- Mantener su propio formulario de inicio de sesión.
- Recopilar las credenciales proporcionadas por el usuario.
- Comunicarse con Dike Identity mediante su backend para solicitar la autenticación.
- Gestionar las funcionalidades propias de su negocio.
- Administrar sus propios roles y permisos.
- Controlar el acceso a sus funcionalidades internas.
- Recibir y procesar las notificaciones enviadas por Dike Identity.
- Impedir la ejecución de operaciones cuando su aplicación se encuentre desactivada.

### 7.3. Límites del sistema

Dike Identity no será responsable de:

- Administrar la información específica del negocio de cada aplicación.
- Gestionar los roles y permisos internos de las aplicaciones.
- Controlar las operaciones propias de cada sistema.
- Definir las reglas particulares de autorización de cada aplicación.

## 08. Áreas funcionales

El dominio de Dike Identity comprende las siguientes áreas principales:

| Área funcional | Descripción |
|---|---|
| Gestión de usuarios | Administración de las identidades, información y estado de los usuarios. |
| Gestión de aplicaciones | Registro, configuración y administración de las aplicaciones consumidoras. |
| Gestión de accesos | Administración de las asignaciones de acceso entre usuarios y aplicaciones. |
| Autenticación | Verificación de la identidad de los usuarios mediante los mecanismos disponibles. |
| Gestión de sesiones y tokens | Administración del ciclo de vida de las sesiones y sus credenciales de acceso. |
| Gestión de perfiles | Consulta y distribución de información centralizada de identidad. |
| Auditoría | Registro y consulta de acontecimientos relevantes del sistema. |
| Integración con aplicaciones | Comunicación entre Dike Identity y los backends de las aplicaciones consumidoras, incluyendo notificaciones de cambios de estado. |

## 09. Evaluación del modelo conceptual

El modelo conceptual definido permite representar los principales elementos del dominio de Dike Identity y establecer sus relaciones y restricciones fundamentales.

Se identifican tres elementos principales para el control de acceso: el usuario, la aplicación y la asignación de acceso. La combinación de sus estados determina si una autenticación puede ser aceptada.

También se diferencia la identidad del usuario de sus mecanismos de autenticación, permitiendo que una misma identidad utilice diferentes métodos sin generar registros duplicados.

Las sesiones y los tokens representan conceptos independientes pero relacionados. La sesión mantiene el contexto de autenticación, mientras que los tokens representan credenciales temporales asociadas a ella.

Finalmente, se establece una separación entre las cuentas administrativas y las cuentas de usuario de las aplicaciones, permitiendo mantener ciclos de vida y sesiones independientes.

El modelo constituye una base conceptual suficiente para avanzar hacia el diseño del sistema. Los detalles de implementación, como la estructura de persistencia, la validación de tokens y los mecanismos de notificación, serán definidos durante las siguientes etapas.

## 10. Conclusiones

Dike Identity busca proporcionar una solución centralizada para la gestión de identidades y autenticación de diferentes aplicaciones pertenecientes a un mismo ecosistema.

El análisis del dominio permitió identificar los conceptos principales, establecer sus relaciones y definir las condiciones que gobiernan el acceso de los usuarios a las aplicaciones.

Se establece que la identidad de un usuario será independiente de sus asignaciones de acceso y que cada aplicación conservará la responsabilidad de administrar sus funcionalidades internas, roles y permisos.

Asimismo, se establece que Dike Identity será responsable de gestionar las sesiones y tokens, invalidando las credenciales cuando cambien las condiciones que permiten el acceso.

El modelo definido proporciona una base suficiente para continuar con el diseño y desarrollo del MVP, manteniendo delimitadas las responsabilidades del sistema y evitando incorporar complejidad innecesaria en esta primera etapa.

---

**Fin del documento - Análisis del dominio de Dike Identity**