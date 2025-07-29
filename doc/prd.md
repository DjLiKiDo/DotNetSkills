---

## Product Requirement Document (PRD) - API de Gestión de Proyectos (MVP)

### 1. Visión y Alcance del Producto

* **Problema a Resolver:** La dificultad de encontrar y utilizar una herramienta de gestión de proyectos de **alta calidad y usabilidad** que sea moderna y eficiente, especialmente para equipos pequeños de desarrollo.
* **Público Objetivo del MVP:** **Equipos de desarrollo** de startups o pequeñas empresas que buscan una solución ágil para organizar sus proyectos, tareas y la colaboración interna.
* **Core del MVP:** Proporcionar las funcionalidades esenciales para la **gestión de usuarios, equipos, proyectos, tareas y subtareas (un nivel de anidamiento)**, incluyendo la asignación y el seguimiento del estado.

### 2. Funcionalidades del MVP

#### 2.1. Gestión de Usuarios y Autenticación

* **Autenticación:**
    * Soporte para **JWT (JSON Web Tokens)** para la autenticación sin estado.
    * La API actuará como un **Identity Provider** para la emisión y validación de tokens.
* **Gestión de Usuarios:**
    * Inicialmente, los usuarios serán **creados por un administrador** a través de la API.
    * No se implementará el registro de usuarios de auto-servicio ni la recuperación de contraseña en el MVP.
* **Roles:**
    * Se definirán **roles fijos** (ej., Administrador, Gestor de Proyecto, Desarrollador) que se asignarán a los usuarios. Estos roles controlarán el acceso a distintas funcionalidades de la API.

#### 2.2. Gestión de Equipos

* **CRUD Completo:** Capacidad para **Crear, Leer, Actualizar y Eliminar** equipos a través de la API.
* **Asociación Usuario-Equipo:**
    * Es **imprescindible** poder asociar usuarios a equipos.
    * *Nota para el desarrollo:* Se utilizará una **relación muchos a muchos** entre `User` y `Team` (probablemente con una tabla intermedia `TeamMember` que contenga el `UserId` y el `TeamId`). Esto evita duplicar usuarios y permite que un usuario pertenezca a múltiples equipos.

#### 2.3. Gestión de Proyectos

* **Atributos Mínimos:**
    * **Nombre:** (String, obligatorio)
    * **Descripción:** (String, opcional)
    * **Equipo Asociado:** (Referencia a `Team`, obligatorio)
* **Líder de Proyecto:** El líder de un proyecto se inferirá a partir del **Product Owner o Manager del Equipo asignado**. No se necesita un campo explícito `LiderProyectoId` en el MVP, ya que se gestionaría a través de la relación con el equipo y los roles.
* **Estado del Proyecto:**
    * Se utilizará un **enumerador fijo** para el estado (ej., `Activo`, `Finalizado`, `Archivado`).

#### 2.4. Gestión de Tareas y Subtareas

* **Atributos de Tarea (Mínimos):**
    * **Título:** (String, obligatorio)
    * **Descripción:** (String, opcional)
    * **Proyecto Asociado:** (Referencia a `Project`, obligatorio)
    * **Estado:** (Enumerador fijo, ej., `Pendiente`, `En Progreso`, `Revisión`, `Completada`)
    * **Prioridad:** (Enumerador fijo, ej., `Baja`, `Media`, `Alta`, `Crítica`)
    * **Usuario Asignado:** (Referencia a `User`, obligatorio, **una tarea solo se asigna a un usuario**).
    * **Fecha de Vencimiento:** (DateTime, opcional)
    * **Fecha de Creación:** (DateTime, generada automáticamente)
    * **Tipo de Tarea:** (Enumerador fijo, ej., `Historia de Usuario`, `Bug`, `Tarea Técnica`, `Mejora`).
* **Subtareas:**
    * **Esencial para el MVP.**
    * Se soportará **un único nivel de anidamiento** (una tarea puede tener subtareas, pero una subtarea no puede tener más subtareas). Esto se logrará con una relación recursiva `ParentTaskId` en la entidad `Task`.

### 3. Requisitos No Funcionales (MVP)

* **Seguridad:**
    * **Validación de JWT:** Verificación de la autenticidad y validez de los tokens en cada solicitud.
    * **Validación de Entradas:** Estricta validación de los datos de entrada en todos los endpoints de la API para prevenir datos mal formados o maliciosos.
    * **Protección contra Ataques Comunes:** Implementación de medidas básicas contra inyección SQL (gracias a EF Core), XSS, etc.
    * **CORS (Cross-Origin Resource Sharing):** Configuración explícita para controlar qué orígenes web pueden acceder a la API.
    * **Rate Limiting:** Implementación básica para prevenir abusos o ataques de denegación de servicio.
    * **Gestión de Secretos:** Se explorará la gestión de secretos (conexión a DB, claves JWT) utilizando variables de entorno y/o Azure Key Vault en el momento oportuno.
* **Rendimiento y Escalabilidad:**
    * **Carga Inicial:** Se asume una carga baja. El rendimiento se monitoreará pero no será una prioridad de optimización exhaustiva en el MVP. Las decisiones de diseño se tomarán pensando en la escalabilidad futura.
* **Observabilidad:**
    * **Logging:** Implementación de un sistema de logging exhaustivo para errores, advertencias e información de solicitudes, especialmente en las fases iniciales de desarrollo y pruebas.
    * **Métricas y Monitoreo:** No se implementarán herramientas o dashboards de métricas y monitoreo avanzados en el MVP, pero se tendrá en cuenta para futuras fases.
* **Documentación:**
    * **Swagger/OpenAPI:** Será la herramienta principal para documentar la API, permitiendo la exploración interactiva de los endpoints.
* **Versión de API:**
    * La API será **versionada desde el MVP** (ej., `/v1/`). Esto es crucial para la evolución futura sin romper compatibilidad.

### 4. Entorno y Herramientas

* **Framework:** .NET 9 (última versión y características).
* **Lenguaje:** C#.
* **API Style:** Minimal APIs.
* **Base de Datos:** SQL Server.
* **Contenedorización:**
    * Docker para la API.
    * Docker para SQL Server.
    * Uso de `docker-compose.yml` para el entorno de desarrollo local.
* **Control de Versiones:** GitHub.
* **CI/CD:** GitHub Actions (discusión más detallada en la Fase 4 del Plan de Acción).
* **Despliegue:** Azure (para Staging y Production).

### 5. Pruebas

* **Pruebas Unitarias:** Imprescindible para el MVP, con un objetivo de **al menos 80% de cobertura de código**.

---

## Plan de Acción para el MVP

Este plan se desglosa en fases para asegurar un progreso estructurado y la entrega incremental de valor.

---

### Fase 1: Configuración Inicial y Cimientos (Semana 1-2, estimado)

1.  **Configuración del Proyecto Base:**
    * Crear la solución y el proyecto **ASP.NET Core Web API (.NET 9)**.
    * Configurar `Program.cs` para el uso de **Minimal APIs**.
2.  **Modelado de Entidades con EF Core:**
    * Definir las clases de entidad en la carpeta `Models`: `User`, `Role`, `Team`, `Project`, `Task`.
    * Prestar especial atención a las relaciones: muchos a muchos (`User`-`Team`), uno a muchos (`Project`-`Team`), uno a muchos (`Task`-`Project`), y recursiva (`Task`-`Task` para subtareas).
3.  **Configuración de EF Core y Base de Datos:**
    * Instalar paquetes NuGet: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`.
    * Crear `ApplicationDbContext` y configurarlo en `Program.cs`.
    * Configurar la cadena de conexión de SQL Server en `appsettings.json` (para desarrollo).
    * Crear y aplicar la primera migración (`dotnet ef migrations add InitialCreate`).
4.  **Dockerización del Entorno de Desarrollo Local:**
    * Crear `Dockerfile` para la API.
    * Crear `Dockerfile` para SQL Server (o usar la imagen oficial `mcr.microsoft.com/mssql/server`).
    * Crear `docker-compose.yml` para orquestar la API y la DB para desarrollo local.
    * Probar el levantamiento del stack localmente (`docker-compose up`).

---

### Fase 2: Seguridad y Autenticación (Semana 2-3, estimado)

1.  **Implementación de Hashing de Contraseñas:**
    * Crear un servicio para hashear y verificar contraseñas (ej., usando `BCrypt.Net` o `Rfc2898DeriveBytes`).
2.  **Configuración de JWT:**
    * Instalar paquetes NuGet para JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`).
    * Configurar el middleware de autenticación JWT en `Program.cs`.
    * Definir la clave secreta JWT (inicialmente en `appsettings.Development.json`).
3.  **Endpoints de Autenticación (Identity Provider Básico):**
    * **`POST /api/v1/auth/register` (admin-only):** Para que un administrador inicial cree usuarios.
    * **`POST /api/v1/auth/login`:** Para que los usuarios obtengan un JWT.
4.  **Roles y Autorización:**
    * Crear los roles fijos en la base de datos (se puede hacer con un "seeder" en una migración o al inicio de la aplicación).
    * Asignar roles a los usuarios.
    * Aplicar atributos `[Authorize(Roles = "Admin")]` o políticas de autorización en los endpoints que lo requieran.
    * Implementar middleware para manejo de CORS.
    * Configurar Rate Limiting básico.

---

### Fase 3: Core del MVP - CRUD y Lógica de Negocio (Semana 3-5, estimado)

1.  **Estructura de Carpetas y DTOs:**
    * Crear carpetas para `DTOs`, `Services`, `Repositories` (si se usa el patrón), y `Mappers`.
    * Definir los DTOs para las operaciones de entrada y salida (`CreateXDto`, `UpdateXDto`, `XDto`).
    * Configurar AutoMapper o un mapeo manual simple.
2.  **Implementación de Repositorios (Opcional, pero buena práctica):**
    * Interfaces y clases para abstraer el acceso a datos para cada entidad principal.
3.  **Servicios de Lógica de Negocio:**
    * **`UserService`:** CRUD básico (obtener usuarios, actualizar roles si es necesario).
    * **`TeamService`:** CRUD para equipos, incluyendo la asociación de usuarios.
    * **`ProjectService`:** CRUD para proyectos.
    * **`TaskService`:** CRUD para tareas y subtareas (manejo del `ParentTaskId` y nivel de anidamiento).
4.  **Endpoints de Minimal APIs (Controladores):**
    * **`GET /api/v1/users` (Admin/Project Manager)**
    * **`GET /api/v1/teams`, `POST /api/v1/teams`, `PUT /api/v1/teams/{id}`, `DELETE /api/v1/teams/{id}`**
    * **`GET /api/v1/projects`, `POST /api/v1/projects`, `PUT /api/v1/projects/{id}`, `DELETE /api/v1/projects/{id}`**
    * **`GET /api/v1/projects/{projectId}/tasks`, `POST /api/v1/projects/{projectId}/tasks`, `PUT /api/v1/tasks/{id}`, `DELETE /api/v1/tasks/{id}`**
    * Considerar endpoints específicos para la asignación de tareas o el cambio de estado.
5.  **Validación de Entradas:**
    * Implementar validaciones en los DTOs (Data Annotations o FluentValidation).
6.  **Manejo de Errores Global:**
    * Crear un middleware de manejo de excepciones personalizado para devolver respuestas JSON estandarizadas (ej., con `ProblemDetails`).

---

### Fase 4: Optimización, Pruebas y CI/CD (Semana 5-6, estimado)

1.  **Paginación, Filtrado y Búsqueda (Básicos):**
    * Implementar query parameters para listar colecciones (ej., `/api/v1/tasks?projectId=X&status=Y&assignedTo=Z&page=1&pageSize=10`).
2.  **Documentación de la API:**
    * Configurar Swashbuckle para generar la documentación OpenAPI.
    * Añadir comentarios XML a los DTOs y endpoints para una mejor documentación en Swagger UI.
3.  **Pruebas Unitarias:**
    * Crear un proyecto de pruebas unitarias (`YourProject.Tests`).
    * Escribir pruebas para los servicios y la lógica de negocio crítica.
    * Asegurar que se cumple el objetivo del **80% de cobertura de código**.
4.  **Logging:**
    * Integrar un logger (ej., Serilog) para registrar eventos, errores y solicitudes.
    * Configurar diferentes niveles de logging para desarrollo y producción.
5.  **CI/CD con GitHub Actions (Estructura Inicial):**
    * **Workflow de CI:** Configurar un workflow que se ejecute en cada `push` o `pull request` a `main` o `develop`. Este workflow debe:
        * Restaurar paquetes NuGet.
        * Construir el proyecto (`dotnet build`).
        * Ejecutar las pruebas unitarias (`dotnet test`).
        * Construir la imagen Docker de la API.
    * **Workflow de CD (Staging):** Configurar un workflow que se active tras un `push` exitoso a `main` (o una rama de despliegue) para desplegar la API en el entorno de Staging de Azure.
        * Esto incluirá la configuración inicial de Azure App Service (o Container Apps) y Azure SQL Database.
