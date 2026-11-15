# Dike_Identity
# Documento de Especificación de Requerimientos

| Información | Detalle |
|---|---|
| **Proyecto** | Dike_Identity |
| **Documento** | Especificación de Requerimientos |
| **Versión** | 1.0 |
| **Estado** | Definición funcional inicial |
| **Etapa** | Etapa 1 - Gestión de identidades y acceso a aplicaciones |
| **Documento relacionado** | Análisis del dominio |

---

# 01. Introducción

## 1.1. Propósito del documento

El presente documento tiene como propósito especificar los requerimientos funcionales y no funcionales de Dike_Identity, estableciendo las funcionalidades, reglas de negocio, restricciones y condiciones de calidad que deberá cumplir el sistema.

Esta especificación servirá como referencia para las siguientes etapas del ciclo de desarrollo de software, incluyendo el modelado del sistema, diseño de arquitectura, implementación y pruebas.

## 1.2. Alcance

Los requerimientos definidos corresponden a la primera etapa de Dike_Identity, orientada a la gestión centralizada de identidades, autenticación de usuarios y administración de accesos a las aplicaciones del ecosistema.

El alcance comprende:

- Gestión de usuarios.
- Gestión de aplicaciones.
- Gestión de asignaciones de acceso.
- Gestión de mecanismos de autenticación.
- Autenticación de usuarios.
- Gestión de sesiones y tokens.
- Consulta de información del perfil.
- Gestión de cuentas administrativas.
- Registro y consulta de auditoría.
- Integración y notificación a aplicaciones clientes.

## 1.3. Actores involucrados

| Actor | Descripción |
|---|---|
| Administrador | Persona autorizada para administrar Dike_Identity. |
| Usuario | Persona cuya identidad es administrada por Dike_Identity y que utiliza las aplicaciones del ecosistema. |
| Aplicación cliente | Sistema externo que consume los servicios de identidad y autenticación de Dike_Identity. |
| Proveedor externo de identidad | Servicio externo que permite verificar la identidad de un usuario, como Google. |

---

# 02. Requerimientos funcionales

## RF-01. Gestión de usuarios

**Descripción:**

El sistema permitirá al administrador gestionar los usuarios registrados en Dike_Identity, incluyendo su creación, consulta, actualización y desactivación, manteniendo la integridad de la información y los registros históricos asociados a cada identidad.

**Funcionalidades:**

- Registrar nuevos usuarios.
- Consultar el listado de usuarios registrados.
- Consultar la información individual de un usuario.
- Actualizar la información general de un usuario.
- Desactivar usuarios.
- Consultar el estado de una cuenta.
- Consultar el historial de modificaciones de un usuario.

**Reglas de negocio y consideraciones:**

1. El registro, actualización y desactivación de usuarios serán operaciones exclusivas del administrador.
2. Cada usuario deberá contar con un identificador único dentro de Dike_Identity.
3. El sistema deberá validar los datos obligatorios y las restricciones de unicidad antes de registrar un usuario.
4. No se permitirá registrar usuarios cuando alguno de los atributos definidos como únicos ya se encuentre asociado a otra identidad.
5. La actualización de información general no deberá modificar automáticamente las credenciales ni los mecanismos de autenticación del usuario.
6. La desactivación de un usuario no implicará la eliminación física de su registro ni de la información histórica asociada.
7. Un usuario desactivado no podrá iniciar nuevas autenticaciones.
8. La desactivación de un usuario deberá invalidar sus sesiones y los tokens asociados a ellas.
9. La desactivación de un usuario no deberá eliminar sus asignaciones de acceso.
10. El estado de un usuario será independiente del estado de sus asignaciones y de las aplicaciones a las que tenga acceso.

**Restricciones:**

- Solamente un administrador autenticado y autorizado podrá ejecutar operaciones de gestión de usuarios.
- No se permitirá eliminar físicamente un usuario mediante las operaciones habituales de administración.
- Las operaciones de gestión deberán mantener la integridad de las relaciones existentes entre usuarios, mecanismos de autenticación, sesiones y asignaciones de acceso.

**Criterios de aceptación:**

- El sistema permite registrar un usuario cuando los datos proporcionados cumplen las validaciones establecidas.
- El sistema rechaza el registro cuando se incumple una restricción de unicidad.
- El sistema permite consultar y actualizar la información general de un usuario existente.
- El sistema impide que un usuario desactivado inicie nuevas autenticaciones.
- El sistema invalida las sesiones y tokens asociados cuando se desactiva un usuario.
- El sistema conserva la información histórica y las asignaciones de acceso después de la desactivación.

---

## RF-02. Gestión de aplicaciones

**Descripción:**

El sistema permitirá al administrador registrar y administrar las aplicaciones que consumirán los servicios proporcionados por Dike_Identity, controlando su identificación, configuración y estado.

**Funcionalidades:**

- Registrar aplicaciones.
- Consultar el listado de aplicaciones registradas.
- Consultar la información individual de una aplicación.
- Actualizar la información de una aplicación.
- Activar y desactivar aplicaciones.
- Consultar el estado de una aplicación.
- Gestionar las credenciales de integración de las aplicaciones.
- Configurar el endpoint de notificaciones de una aplicación.

**Reglas de negocio y consideraciones:**

1. Solamente un administrador autorizado podrá registrar, actualizar, activar o desactivar aplicaciones.
2. Cada aplicación deberá contar con un identificador único dentro de Dike_Identity.
3. Cada aplicación deberá disponer de credenciales que permitan identificarla y verificar su identidad al consumir los servicios de Dike_Identity.
4. Las credenciales de integración deberán ser administradas de manera independiente a las credenciales de los usuarios.
5. Una aplicación deberá encontrarse registrada y activa para consumir los servicios de autenticación y validación de acceso.
6. La desactivación de una aplicación no implicará la eliminación física de su registro.
7. La desactivación de una aplicación no deberá eliminar las asignaciones de acceso existentes.
8. La desactivación de una aplicación deberá invalidar las sesiones y los tokens asociados a dicha aplicación.
9. La modificación de la información de una aplicación no deberá alterar automáticamente sus asignaciones de acceso.
10. Una aplicación podrá tener configurado un endpoint de backend para recibir notificaciones de Dike_Identity.

**Restricciones:**

- Las credenciales de integración no deberán exponerse en las respuestas habituales de consulta de aplicaciones.
- Las operaciones administrativas deberán estar restringidas a administradores autorizados.
- Una aplicación desactivada no podrá realizar nuevas operaciones de autenticación o validación de acceso.

**Criterios de aceptación:**

- El sistema permite registrar una aplicación con identificadores únicos.
- El sistema permite consultar y actualizar la información de una aplicación.
- El sistema permite activar y desactivar una aplicación.
- El sistema rechaza las solicitudes de integración provenientes de aplicaciones desactivadas.
- El sistema invalida las sesiones y tokens asociados cuando se desactiva una aplicación.
- La desactivación conserva las asignaciones y los registros históricos.
- El sistema permite configurar el endpoint de notificaciones de una aplicación.

---

## RF-03. Asignación de aplicaciones a usuarios

**Descripción:**

El sistema permitirá al administrador gestionar las asignaciones de acceso entre usuarios y aplicaciones, determinando qué usuarios tienen habilitado el acceso a cada aplicación del ecosistema.

**Funcionalidades:**

- Asignar una aplicación a un usuario.
- Consultar las aplicaciones asignadas a un usuario.
- Consultar los usuarios asociados a una aplicación.
- Consultar el estado de una asignación.
- Desactivar una asignación de acceso.
- Reactivar una asignación existente.
- Consultar el historial de modificaciones de una asignación.

**Reglas de negocio y consideraciones:**

1. Solamente un administrador autorizado podrá gestionar las asignaciones de acceso.
2. Un usuario podrá tener asignaciones de acceso a múltiples aplicaciones.
3. Una aplicación podrá tener asignaciones de acceso correspondientes a múltiples usuarios.
4. Cada combinación de usuario y aplicación podrá tener una única asignación.
5. No se permitirá crear una asignación cuando ya exista una relación entre el usuario y la aplicación.
6. Si una asignación se encuentra desactivada, su reactivación deberá realizarse actualizando el estado de la asignación existente.
7. La desactivación de una asignación no implicará la eliminación de la relación entre el usuario y la aplicación.
8. Un usuario solamente podrá autenticarse en una aplicación cuando exista una asignación activa entre ambos.
9. La desactivación de una asignación deberá invalidar las sesiones y los tokens asociados al acceso del usuario a la aplicación correspondiente.
10. La desactivación de una asignación no deberá afectar los accesos del usuario a otras aplicaciones.
11. La asignación de acceso no implicará la asignación automática de roles o permisos específicos dentro de la aplicación.
12. Las modificaciones de una asignación deberán permitir identificar al administrador responsable y la fecha en que se realizaron.

**Restricciones:**

- No se permitirá asignar una aplicación inexistente o desactivada.
- No se permitirá asignar una aplicación a un usuario inexistente o desactivado.
- No se permitirá eliminar físicamente una asignación mediante las operaciones habituales de administración.

**Criterios de aceptación:**

- El sistema permite asignar una aplicación activa a un usuario activo.
- El sistema impide crear asignaciones duplicadas.
- El sistema permite consultar las asignaciones de un usuario y los usuarios asociados a una aplicación.
- El sistema permite desactivar una asignación existente.
- El sistema permite reactivar una asignación desactivada sin crear una nueva relación.
- El sistema invalida las sesiones y tokens del contexto de acceso afectado cuando se desactiva una asignación.
- El sistema conserva el historial de modificaciones de las asignaciones.

---

## RF-04. Gestión de mecanismos de autenticación

**Descripción:**

El sistema permitirá administrar los mecanismos de autenticación asociados a las identidades de los usuarios, proporcionando diferentes medios para verificar su identidad sin generar identidades duplicadas.

**Funcionalidades:**

- Registrar mecanismos de autenticación para un usuario.
- Consultar los mecanismos de autenticación asociados a un usuario.
- Actualizar las credenciales de un mecanismo de autenticación.
- Vincular proveedores externos de identidad.
- Consultar el estado de los mecanismos de autenticación.
- Deshabilitar mecanismos de autenticación cuando corresponda.

**Reglas de negocio y consideraciones:**

1. Una identidad podrá tener uno o varios mecanismos de autenticación.
2. Cada mecanismo de autenticación deberá estar asociado a una única identidad de usuario.
3. Inicialmente, el sistema permitirá la autenticación mediante correo electrónico y contraseña.
4. El sistema deberá permitir incorporar proveedores externos de identidad, como Google.
5. La vinculación de un proveedor externo deberá asociarse a una identidad existente en Dike_Identity.
6. Durante el MVP, la vinculación de cuentas de Google será realizada exclusivamente por un administrador autorizado.
7. El correo electrónico verificado por Google deberá coincidir con el correo registrado en Dike_Identity.
8. La vinculación de una cuenta de Google no deberá deshabilitar automáticamente el mecanismo de autenticación mediante contraseña.
9. Un proveedor externo no deberá generar una nueva identidad cuando se vincule a un usuario existente.
10. Las credenciales deberán almacenarse y tratarse de manera segura.

**Restricciones:**

- No se permitirá vincular una cuenta externa que ya se encuentre asociada a otra identidad.
- No se permitirá vincular una cuenta de Google cuando el correo electrónico verificado no coincida con el registrado en Dike_Identity.
- La vinculación de proveedores externos estará restringida a administradores autorizados durante el MVP.

**Criterios de aceptación:**

- El sistema permite registrar un mecanismo de autenticación para una identidad existente.
- El sistema permite consultar los mecanismos asociados a un usuario.
- El sistema permite actualizar las credenciales mediante las operaciones correspondientes.
- El sistema permite vincular una cuenta de Google cuando se cumplen las condiciones establecidas.
- El sistema rechaza la vinculación de una cuenta externa asociada a otra identidad.
- El sistema conserva la identidad centralizada al incorporar mecanismos adicionales.

---

## RF-05. Autenticación

**Descripción:**

El sistema permitirá que las aplicaciones clientes soliciten la autenticación de sus usuarios mediante los mecanismos disponibles en Dike_Identity, verificando la identidad del usuario y las condiciones necesarias para permitir su acceso a la aplicación solicitante.

**Funcionalidades:**

- Recibir solicitudes de autenticación provenientes de aplicaciones clientes.
- Identificar y verificar la identidad de la aplicación solicitante.
- Recibir y validar las credenciales del usuario.
- Validar el estado del usuario.
- Validar el estado de la aplicación solicitante.
- Validar la asignación de acceso entre el usuario y la aplicación.
- Autenticar usuarios mediante proveedores externos de identidad habilitados.
- Retornar el resultado del proceso de autenticación.
- Registrar los acontecimientos relacionados con la autenticación.

**Reglas de negocio y consideraciones:**

1. Una solicitud de autenticación deberá provenir de una aplicación registrada y activa.
2. La aplicación solicitante deberá demostrar su identidad mediante las credenciales de integración correspondientes.
3. El usuario deberá existir y encontrarse activo en Dike_Identity.
4. Las credenciales proporcionadas deberán corresponder a un mecanismo de autenticación válido y habilitado.
5. El usuario deberá contar con una asignación de acceso activa para la aplicación solicitante.
6. La autenticación solamente será exitosa cuando se cumplan todas las condiciones anteriores.
7. Una autenticación exitosa deberá establecer una sesión asociada al usuario y a la aplicación correspondiente.
8. El sistema deberá impedir que una misma cuenta mantenga más de una sesión activa simultáneamente.
9. Si existe una sesión activa al momento de establecer una nueva, la anterior deberá finalizar antes de establecer la nueva.
10. Los intentos de autenticación exitosos y fallidos deberán quedar registrados en la auditoría.
11. Los errores de autenticación no deberán exponer información sensible sobre las credenciales ni detalles internos del sistema.

**Restricciones:**

- No se permitirá autenticar usuarios desactivados.
- No se permitirá autenticar usuarios en aplicaciones desactivadas.
- No se permitirá autenticar usuarios que no tengan una asignación de acceso activa.
- No se permitirá establecer una sesión cuando las condiciones de autenticación no sean válidas.

**Criterios de aceptación:**

- El sistema autentica correctamente a un usuario cuando todas las condiciones son válidas.
- El sistema rechaza solicitudes provenientes de aplicaciones no registradas o desactivadas.
- El sistema rechaza credenciales incorrectas.
- El sistema rechaza usuarios desactivados.
- El sistema rechaza usuarios sin asignación activa para la aplicación solicitante.
- El sistema establece una sesión después de una autenticación exitosa.
- El sistema finaliza la sesión anterior cuando se establece una nueva para la misma cuenta.
- El sistema registra los intentos exitosos y fallidos.

---

## RF-06. Gestión de sesiones

**Descripción:**

El sistema permitirá administrar las sesiones de autenticación establecidas por los usuarios y administradores, controlando su vigencia, renovación y finalización.

**Funcionalidades:**

- Crear sesiones después de una autenticación exitosa.
- Consultar el estado de una sesión.
- Identificar las sesiones activas.
- Finalizar sesiones mediante el cierre de sesión.
- Revocar sesiones.
- Asociar tokens a las sesiones correspondientes.
- Conservar el historial de sesiones.
- Registrar los acontecimientos relevantes relacionados con las sesiones.

**Reglas de negocio y consideraciones:**

1. Una sesión se establecerá después de una autenticación exitosa.
2. Cada sesión deberá estar asociada a una única identidad y a su contexto de acceso.
3. Las sesiones de usuarios y las sesiones administrativas deberán mantenerse en contextos independientes.
4. Cada cuenta de usuario podrá mantener una sola sesión activa simultáneamente.
5. Cada cuenta administrativa podrá mantener una sola sesión administrativa activa simultáneamente.
6. Una nueva sesión deberá finalizar la sesión activa anterior de la misma cuenta y del mismo contexto.
7. Una sesión podrá finalizar mediante cierre de sesión, revocación o por una condición de invalidación establecida por el sistema.
8. La finalización de una sesión deberá invalidar todos los tokens asociados a ella.
9. La desactivación de un usuario deberá finalizar sus sesiones.
10. La desactivación de una aplicación deberá finalizar las sesiones asociadas a ella.
11. La desactivación de una asignación deberá finalizar las sesiones correspondientes al acceso afectado.
12. Las sesiones finalizadas o revocadas deberán conservarse como registros históricos.

**Criterios de aceptación:**

- El sistema crea una sesión después de una autenticación exitosa.
- El sistema permite identificar el estado de una sesión.
- El sistema impide mantener dos sesiones activas para una misma cuenta dentro del mismo contexto.
- El sistema permite finalizar una sesión.
- La finalización de una sesión invalida todos sus tokens asociados.
- El sistema conserva el historial de sesiones finalizadas y revocadas.

---

## RF-07. Gestión de tokens

**Descripción:**

El sistema permitirá emitir, validar, renovar e invalidar los tokens utilizados por las aplicaciones clientes para mantener y verificar el contexto de autenticación de los usuarios.

**Funcionalidades:**

- Emitir tokens de acceso.
- Emitir y gestionar credenciales de renovación.
- Validar la vigencia de los tokens.
- Renovar tokens.
- Invalidar tokens.
- Asociar tokens a sesiones y contextos de acceso.
- Controlar la expiración de los tokens.

**Reglas de negocio y consideraciones:**

1. Los tokens serán emitidos exclusivamente por Dike_Identity después de una autenticación exitosa.
2. Cada token deberá estar asociado a una sesión y a un contexto de acceso determinado.
3. Los tokens deberán contar con un período de vigencia definido.
4. Un token expirado no deberá permitir el acceso.
5. La renovación solamente podrá realizarse mediante una credencial de renovación válida.
6. La renovación deberá generar un nuevo token de acceso y una nueva credencial de renovación cuando corresponda a la estrategia de rotación establecida.
7. La credencial de renovación anterior deberá dejar de ser válida después de una renovación exitosa.
8. La renovación deberá verificar que la sesión y las condiciones de acceso continúen siendo válidas.
9. La desactivación de un usuario, aplicación o asignación deberá invalidar los tokens asociados al contexto afectado.
10. La finalización o revocación de una sesión deberá invalidar todos sus tokens asociados.
11. Los tokens invalidados no deberán ser aceptados aunque su fecha de expiración todavía no haya sido alcanzada.
12. Los tokens deberán protegerse contra accesos y usos no autorizados.

**Restricciones:**

- No se permitirá renovar tokens asociados a sesiones finalizadas o revocadas.
- No se permitirá renovar tokens cuando el usuario, la aplicación o la asignación de acceso se encuentren deshabilitados.
- No se permitirá utilizar tokens expirados o invalidados.

**Criterios de aceptación:**

- El sistema emite tokens después de una autenticación exitosa.
- El sistema valida los tokens vigentes y rechaza los expirados.
- El sistema permite renovar credenciales cuando la sesión continúa siendo válida.
- El sistema invalida la credencial de renovación anterior después de una rotación exitosa.
- El sistema rechaza tokens asociados a sesiones finalizadas.
- El sistema rechaza tokens asociados a usuarios, aplicaciones o asignaciones desactivadas.
- La invalidación de un token debe ser efectiva independientemente de su fecha de expiración.

---

## RF-08. Información del perfil

**Descripción:**

El sistema proporcionará información centralizada del perfil de los usuarios autenticados a las aplicaciones clientes que cuenten con autorización para consultar dicha información.

**Funcionalidades:**

- Consultar información del perfil del usuario autenticado.
- Identificar al usuario asociado al contexto de autenticación.
- Proporcionar los atributos de perfil autorizados para su consumo.
- Validar la vigencia del contexto de acceso antes de proporcionar información.

**Reglas de negocio y consideraciones:**

1. La información proporcionada deberá corresponder a la identidad asociada al contexto de autenticación.
2. Solamente las aplicaciones autorizadas podrán consultar la información del perfil.
3. La información expuesta deberá limitarse a los atributos definidos para su consumo.
4. Las aplicaciones no deberán poder consultar arbitrariamente perfiles de otros usuarios mediante el servicio destinado al usuario autenticado.
5. Dike_Identity será responsable de mantener centralizada la información de identidad.
6. Las aplicaciones clientes podrán utilizar la información proporcionada para representar los datos del usuario en sus interfaces.
7. La información específica del negocio de cada aplicación no formará parte del perfil centralizado.

**Criterios de aceptación:**

- El sistema proporciona la información correspondiente al usuario autenticado.
- El sistema rechaza solicitudes con tokens expirados o invalidados.
- El sistema impide consultar perfiles sin autorización.
- El sistema proporciona únicamente los atributos permitidos para la aplicación solicitante.

---

## RF-09. Gestión de cuentas administrativas

**Descripción:**

El sistema permitirá administrar las cuentas administrativas utilizadas para acceder a las funcionalidades de gestión de Dike_Identity, manteniendo su independencia respecto de las identidades de usuario de las aplicaciones clientes.

**Funcionalidades:**

- Registrar cuentas administrativas.
- Consultar cuentas administrativas.
- Actualizar información administrativa.
- Desactivar cuentas administrativas.
- Autenticar administradores.
- Gestionar los mecanismos de autenticación administrativos.
- Consultar el estado de las cuentas administrativas.

**Reglas de negocio y consideraciones:**

1. Las cuentas administrativas serán independientes de las identidades de usuario de las aplicaciones.
2. Las cuentas administrativas dispondrán de mecanismos de autenticación propios.
3. La autenticación administrativa no deberá depender de las asignaciones de acceso a aplicaciones.
4. La desactivación de una cuenta administrativa no deberá modificar las identidades ni las asignaciones de acceso de los usuarios.
5. Las sesiones administrativas deberán gestionarse de manera independiente de las sesiones de usuario.
6. Las operaciones administrativas deberán estar restringidas a cuentas administrativas activas y autorizadas.
7. La desactivación de una cuenta administrativa deberá finalizar sus sesiones administrativas.

**Criterios de aceptación:**

- El sistema permite autenticar administradores mediante sus mecanismos propios.
- El sistema impide el acceso administrativo a cuentas desactivadas.
- El sistema mantiene la independencia entre las cuentas administrativas y las cuentas de usuario.
- La desactivación de una cuenta administrativa no modifica las identidades ni los accesos de los usuarios.

---

## RF-10. Registro y consulta de auditoría

**Descripción:**

El sistema registrará y permitirá consultar los acontecimientos relevantes relacionados con la administración de identidades, aplicaciones, asignaciones de acceso, autenticación, sesiones y tokens.

**Funcionalidades:**

- Registrar operaciones administrativas.
- Registrar intentos de autenticación exitosos y fallidos.
- Registrar renovaciones de tokens.
- Registrar cierres de sesión y revocaciones.
- Registrar cambios de estado de usuarios y aplicaciones.
- Registrar cambios en las asignaciones de acceso.
- Registrar acontecimientos relacionados con mecanismos de autenticación.
- Consultar los registros de auditoría.
- Consultar el historial de operaciones asociadas a usuarios y aplicaciones.

**Reglas de negocio y consideraciones:**

1. Los registros deberán permitir identificar el tipo de acontecimiento.
2. Cada registro deberá conservar la fecha y hora de ocurrencia.
3. Los acontecimientos administrativos deberán permitir identificar al administrador responsable.
4. Los acontecimientos generados por aplicaciones deberán permitir identificar la aplicación de origen.
5. Los acontecimientos generados automáticamente deberán permitir identificar el proceso o componente responsable.
6. Los registros deberán incluir el resultado de la operación cuando corresponda.
7. Los registros históricos deberán conservarse aunque se desactive un usuario, aplicación o asignación.
8. Los registros de auditoría no deberán poder modificarse ni eliminarse mediante las operaciones administrativas habituales.
9. No deberán almacenarse contraseñas, tokens completos ni credenciales de renovación en los registros.
10. El acceso a la consulta de auditoría deberá estar restringido a administradores autorizados.

**Criterios de aceptación:**

- El sistema registra los acontecimientos definidos como relevantes.
- Los registros permiten identificar el origen, tipo, fecha y resultado del acontecimiento.
- Los registros administrativos permiten identificar al responsable.
- El sistema permite consultar el historial de auditoría.
- El sistema impide la modificación o eliminación de registros mediante las operaciones habituales.
- Los registros no contienen credenciales ni tokens completos.

---

## RF-11. Notificaciones a aplicaciones clientes

**Descripción:**

El sistema permitirá notificar a las aplicaciones clientes sobre cambios relevantes en su estado mediante los endpoints de backend configurados para recibir dichas notificaciones.

**Funcionalidades:**

- Configurar el endpoint de notificaciones de una aplicación.
- Generar notificaciones cuando se produzcan cambios relevantes en el estado de una aplicación.
- Enviar notificaciones al endpoint configurado.
- Registrar los intentos de entrega.
- Mantener pendientes las notificaciones que no puedan entregarse.
- Reintentar la entrega de notificaciones pendientes.

**Reglas de negocio y consideraciones:**

1. Cada aplicación podrá tener cero o un endpoint de notificaciones configurado.
2. El endpoint deberá pertenecer a la aplicación correspondiente.
3. La modificación del estado de una aplicación deberá generar una notificación.
4. La falta de disponibilidad del endpoint no deberá impedir la ejecución de la operación administrativa.
5. Las notificaciones que no puedan entregarse deberán quedar pendientes de procesamiento.
6. Los intentos de entrega deberán permitir identificar su resultado.
7. Las notificaciones deberán incluir únicamente la información necesaria para comunicar el acontecimiento.
8. El sistema deberá contemplar mecanismos para verificar la autenticidad de las notificaciones recibidas por las aplicaciones.

**Criterios de aceptación:**

- El sistema permite configurar un endpoint para una aplicación.
- El sistema genera una notificación cuando cambia el estado de una aplicación.
- El sistema intenta entregar la notificación al endpoint configurado.
- La indisponibilidad del endpoint no revierte la operación administrativa.
- El sistema conserva las notificaciones pendientes de entrega.
- El sistema permite identificar los resultados de los intentos de entrega.

---

# 03. Requerimientos no funcionales

Los requerimientos no funcionales establecen las condiciones de calidad, seguridad, rendimiento y operación que deberá cumplir Dike_Identity.

## RNF-01. Seguridad de la información

**Descripción:**

El sistema deberá proteger la información sensible almacenada y procesada, evitando su exposición, modificación o acceso por parte de personas y aplicaciones no autorizadas.

**Criterios:**

- Las contraseñas de los usuarios deberán almacenarse mediante algoritmos de hashing diseñados para contraseñas, con mecanismos de protección adecuados.
- Las credenciales de integración de aplicaciones deberán almacenarse de forma segura.
- Los secretos y tokens no deberán exponerse en registros de auditoría ni mensajes de error.
- La comunicación entre Dike_Identity y las aplicaciones clientes deberá utilizar canales protegidos mediante HTTPS.
- El acceso a información sensible deberá estar restringido según las responsabilidades y autorizaciones correspondientes.

## RNF-02. Seguridad de autenticación y tokens

**Descripción:**

Los mecanismos de autenticación, emisión, validación, renovación e invalidación de tokens deberán implementar controles que protejan la identidad de los usuarios y eviten el uso no autorizado de las credenciales.

**Criterios:**

- Las credenciales deberán validarse mediante mecanismos seguros.
- Los tokens deberán contar con mecanismos de integridad y autenticidad.
- Los tokens deberán tener tiempos de expiración definidos.
- Las credenciales de renovación deberán estar protegidas contra su reutilización no autorizada.
- La invalidación de tokens deberá hacerse efectiva según las reglas de negocio establecidas.
- Las solicitudes de autenticación y renovación deberán validar el contexto de acceso correspondiente.
- Los mecanismos de autenticación deberán contemplar protección frente a ataques de fuerza bruta y abuso de solicitudes.

## RNF-03. Disponibilidad

**Descripción:**

Dike_Identity deberá proporcionar disponibilidad suficiente para permitir que las aplicaciones clientes consuman sus servicios de identidad y autenticación durante los períodos operativos establecidos.

**Criterios:**

- El sistema deberá contar con un objetivo de disponibilidad definido.
- Las interrupciones del servicio deberán poder identificarse y registrarse.
- Deberán establecerse procedimientos de recuperación ante fallos.
- Las operaciones de mantenimiento deberán planificarse para reducir su impacto sobre las aplicaciones clientes.

**Parámetros pendientes de definición:**

- Porcentaje de disponibilidad objetivo.
- Ventanas de mantenimiento.
- Tiempo objetivo de recuperación (RTO).
- Punto objetivo de recuperación de datos (RPO).

## RNF-04. Rendimiento

**Descripción:**

El sistema deberá responder oportunamente a las solicitudes de autenticación, validación de tokens, renovación y consulta de perfiles realizadas por las aplicaciones clientes.

**Criterios:**

- Los tiempos de respuesta deberán mantenerse dentro de los objetivos definidos para cada operación.
- El sistema deberá soportar la cantidad esperada de solicitudes concurrentes.
- Las operaciones de autenticación no deberán verse afectadas significativamente por consultas administrativas de baja prioridad.
- El rendimiento deberá evaluarse mediante pruebas de carga y concurrencia.

**Parámetros pendientes de definición:**

- Tiempo máximo de respuesta para autenticación.
- Tiempo máximo de respuesta para validación de tokens.
- Tiempo máximo de respuesta para renovación.
- Cantidad esperada de solicitudes concurrentes.
- Volumen esperado de usuarios y aplicaciones.

## RNF-05. Integridad de la información

**Descripción:**

El sistema deberá garantizar la consistencia e integridad de la información relacionada con usuarios, aplicaciones, asignaciones, sesiones, tokens y registros históricos.

**Criterios:**

- No deberán existir asignaciones duplicadas para una misma combinación de usuario y aplicación.
- Las relaciones entre usuarios, aplicaciones y asignaciones deberán mantener su integridad.
- Las operaciones que modifiquen estados relacionados deberán mantener la consistencia del sistema.
- Las operaciones de persistencia deberán garantizar las propiedades de integridad correspondientes.
- Los registros históricos deberán conservar su relación con las identidades y aplicaciones asociadas.

## RNF-06. Trazabilidad y protección de auditoría

**Descripción:**

El sistema deberá garantizar que los acontecimientos registrados en la auditoría puedan consultarse de manera confiable, conservando su integridad y disponibilidad durante el período de retención establecido.

**Criterios:**

- Los registros deberán conservar una referencia temporal consistente.
- Los registros deberán protegerse contra modificaciones no autorizadas.
- La consulta de auditoría deberá estar restringida a usuarios autorizados.
- Los acontecimientos deberán poder correlacionarse con las operaciones que los originaron.
- Los registros deberán contar con una política de retención definida.

**Parámetros pendientes de definición:**

- Período mínimo de conservación.
- Volumen esperado de registros.
- Política de respaldo y archivo.
- Requisitos legales o regulatorios aplicables, si corresponde.

## RNF-07. Interoperabilidad

**Descripción:**

Dike_Identity deberá permitir la integración con diferentes aplicaciones clientes mediante interfaces de comunicación estandarizadas y documentadas.

**Criterios:**

- Los servicios deberán exponer contratos de comunicación documentados.
- Las interfaces deberán utilizar formatos de intercambio compatibles con las aplicaciones consumidoras.
- Los errores deberán comunicarse mediante respuestas consistentes.
- Los cambios incompatibles en las interfaces deberán gestionarse mediante una estrategia de versionado.
- La integración deberá contemplar mecanismos seguros de identificación de aplicaciones.

## RNF-08. Mantenibilidad

**Descripción:**

El sistema deberá facilitar su mantenimiento y evolución, permitiendo incorporar funcionalidades y corregir defectos sin comprometer innecesariamente los componentes existentes.

**Criterios:**

- El código deberá mantener una organización coherente con las responsabilidades del sistema.
- Los componentes deberán contar con responsabilidades claramente delimitadas.
- Las funcionalidades principales deberán disponer de pruebas automatizadas.
- Las interfaces y decisiones técnicas relevantes deberán estar documentadas.
- Los cambios deberán poder verificarse mediante procesos de construcción y pruebas.

## RNF-09. Recuperabilidad

**Descripción:**

El sistema deberá disponer de mecanismos que permitan recuperar la información y restablecer la operación después de fallos o interrupciones.

**Criterios:**

- Deberán establecerse mecanismos de respaldo de información.
- Los respaldos deberán poder verificarse mediante pruebas de restauración.
- La recuperación deberá considerar la integridad de las relaciones entre usuarios, aplicaciones, sesiones y registros históricos.
- Los procedimientos de recuperación deberán ser compatibles con los objetivos de RTO y RPO definidos.

## RNF-10. Protección de datos personales

**Descripción:**

El sistema deberá proteger la información personal de los usuarios y limitar su tratamiento y exposición a los fines autorizados.

**Criterios:**

- Solamente deberán recopilarse los datos necesarios para las funcionalidades del sistema.
- La información del perfil deberá exponerse únicamente a aplicaciones autorizadas.
- Las operaciones administrativas sobre información personal deberán quedar registradas.
- La información sensible no deberá exponerse innecesariamente en mensajes de error ni registros técnicos.
- El tratamiento de datos deberá cumplir las disposiciones legales aplicables.

---

# 04. Restricciones generales del sistema

Las siguientes restricciones delimitan las responsabilidades y el comportamiento general de Dike_Identity.

1. Dike_Identity será responsable de la gestión centralizada de identidades y autenticación.
2. Las aplicaciones clientes conservarán la responsabilidad de administrar sus propias funcionalidades de negocio.
3. Los roles y permisos internos de las aplicaciones no serán administrados por Dike_Identity durante esta etapa.
4. Las identidades de los usuarios serán independientes de sus asignaciones de acceso.
5. Las cuentas administrativas serán independientes de las cuentas de usuario de las aplicaciones.
6. La desactivación de usuarios, aplicaciones o asignaciones no implicará la eliminación de sus registros históricos.
7. Las operaciones administrativas deberán requerir una cuenta autorizada.
8. Los servicios de integración deberán verificar la identidad de las aplicaciones consumidoras.
9. Las sesiones y los tokens deberán respetar las condiciones actuales de acceso.
10. Los registros de auditoría deberán conservar la trazabilidad de las operaciones relevantes.

---

# 05. Aspectos pendientes de definición

Los siguientes aspectos requieren decisiones adicionales antes de considerar completa la especificación funcional y no funcional.

| Código | Aspecto pendiente | Requerimiento relacionado | Prioridad |
|---|---|---|---|
| PD-01 | Definir los atributos obligatorios y únicos de un usuario. | RF-01 | Alta |
| PD-02 | Definir los atributos e identificadores de las aplicaciones. | RF-02 | Alta |
| PD-03 | Definir el mecanismo de autenticación de aplicaciones clientes. | RF-02, RF-05 | Crítica |
| PD-04 | Definir las políticas de contraseñas. | RF-04, RNF-01 | Alta |
| PD-05 | Definir el cambio y recuperación de contraseñas. | RF-04 | Alta |
| PD-06 | Definir los mecanismos de protección frente a intentos fallidos de autenticación. | RF-05, RNF-02 | Crítica |
| PD-07 | Definir el formato y los tiempos de vida de los tokens. | RF-07 | Alta |
| PD-08 | Definir la estrategia técnica para invalidación inmediata de tokens. | RF-06, RF-07 | Crítica |
| PD-09 | Definir el comportamiento ante renovación concurrente de tokens. | RF-07 | Alta |
| PD-10 | Definir los atributos del perfil expuestos a las aplicaciones. | RF-08 | Alta |
| PD-11 | Definir el proceso de creación y administración de cuentas administrativas. | RF-09 | Alta |
| PD-12 | Definir la política de retención de auditoría. | RF-10, RNF-06 | Alta |
| PD-13 | Definir los tiempos máximos de respuesta y capacidad esperada. | RNF-04 | Media |
| PD-14 | Definir los objetivos de disponibilidad, RTO y RPO. | RNF-03, RNF-09 | Media |
| PD-15 | Definir el mecanismo de autenticidad de las notificaciones. | RF-11, RNF-01 | Alta |
| PD-16 | Definir la política de reintentos y entrega de notificaciones. | RF-11 | Media |
| PD-17 | Definir los atributos y permisos de consulta de auditoría. | RF-10 | Alta |
| PD-18 | Definir el comportamiento cuando una persona tiene una cuenta administrativa y una cuenta de usuario asociadas a la misma identidad personal. | RF-01, RF-09 | Media |

---

# 06. Consideraciones para la siguiente etapa

La presente especificación establece las funcionalidades y condiciones generales que deberá cumplir Dike_Identity durante su primera etapa.

Antes de avanzar al diseño detallado, deberán resolverse los aspectos pendientes que afecten directamente a la seguridad, integridad y comportamiento funcional del sistema.

Una vez definidos estos aspectos, los requerimientos servirán como base para:

- Elaborar los diagramas de casos de uso.
- Modelar los flujos principales y alternativos.
- Diseñar los contratos de integración.
- Diseñar la arquitectura del sistema.
- Definir el modelo de persistencia.
- Elaborar los casos de prueba funcionales y no funcionales.

Los criterios de aceptación definidos en este documento podrán ampliarse posteriormente mediante escenarios de prueba específicos.

---

**Fin del documento - Especificación de Requerimientos de Dike_Identity**