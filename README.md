# Clean Architecture .NET Template

Template base reusable para `.NET 8` orientada a microservicios y BFFs enterprise con enfoque `Clean Architecture + DDD Lite`.

El repositorio usa el nombre temporal `TemplateProject` para que luego pueda ser reemplazado dinámicamente por el motor de plantillas de `.NET` mediante `sourceName`.

## Propósito

Esta template deja preparada una base limpia, escalable y compilable para evolucionar posteriormente con capacidades como:

- CQRS
- MediatR
- FluentValidation
- Response estándar
- Middleware global
- Serilog
- Docker
- Swagger
- SQL Server
- EF Core
- Dapper
- Unit Testing expandido
- OpenTelemetry

La solución incluye estructura base enterprise, Swagger, response estándar, middleware global, CRUD de ejemplo y logging estructurado reusable con Serilog.

## Preparación para `dotnet new`

La template quedó preparada para instalación local con:

- `sourceName` configurado como `TemplateProject`
- exclusión de artefactos locales como `bin`, `obj`, `.git` y `.idea`
- solución y proyectos alineados a `.NET 8`
- namespaces base consistentes para reemplazo posterior por el motor de templates

## Estructura

```text
template-clean-architecture-dotnet
│
├── .template.config
│   └── template.json
│
├── src
│   ├── TemplateProject.Api
│   ├── TemplateProject.Application
│   ├── TemplateProject.Domain
│   └── TemplateProject.Infrastructure
│
├── tests
│   ├── TemplateProject.Api.Tests
│   ├── TemplateProject.Application.Tests
│   ├── TemplateProject.Domain.Tests
│   └── TemplateProject.Infrastructure.Tests
│
├── docker
│
├── README.md
│
└── TemplateProject.sln
```

## Capas de arquitectura

- `TemplateProject.Api`: punto de entrada HTTP preparado para Controllers tradicionales.
- `TemplateProject.Application`: capa de aplicación para casos de uso, contratos y orquestación.
- `TemplateProject.Domain`: núcleo del dominio para reglas de negocio y abstracciones del modelo.
- `TemplateProject.Infrastructure`: implementaciones técnicas e integraciones externas.
- `tests/*`: proyectos de pruebas separados por capa para crecimiento enterprise ordenado.

## GitHub Governance

La template incluye una base de gobernanza para GitHub con enfoque enterprise:

- estándar GitFlow documentado en [github-gitflow-standard.md](/Users/macuser/Repos/template-clean-architecture-dotnet/docs/governance/github-gitflow-standard.md)
- `pull_request_template.md`
- `CODEOWNERS`
- `Dependabot`
- `CodeQL`
- pipeline CI para `dotnet restore`, `dotnet build` y `dotnet test`

Esto deja el repositorio preparado para trabajar con `main`, `develop`, `feature/*`, `bugfix/*`, `release/*` y `hotfix/*`, con branch protection y quality gates administrados desde GitHub.

## Logging

La template usa `Serilog` como base de logging estructurado enterprise.

- escribe logs en consola
- escribe logs en archivos dentro de `logs/`
- genera un archivo por día con rolling logging
- incluye `TraceId` por request
- registra requests HTTP y excepciones
- queda preparada para evolucionar luego a `Seq`, `Elasticsearch`, `Kibana`, `Loki`, `Grafana` y `OpenTelemetry`

### Archivos generados

Al ejecutar la API con `dotnet run`, Serilog crea automáticamente archivos como:

```text
logs/
├── log-20260517.txt
├── log-20260518.txt
└── log-20260519.txt
```

### Configuración

La configuración de logging vive en [appsettings.json](/Users/macuser/Repos/template-clean-architecture-dotnet/src/TemplateProject.Api/appsettings.json).

Desde ahí puedes cambiar:

- nivel mínimo global
- overrides para `Microsoft`
- sinks habilitados
- formato de salida
- política de rolling diario

### Cambio de niveles

Para subir o bajar el nivel de logs, ajusta:

```json
"MinimumLevel": {
  "Default": "Information",
  "Override": {
    "Microsoft": "Warning"
  }
}
```

### Rolling logging

La escritura en archivo usa:

- `path: logs/log-.txt`
- `rollingInterval: Day`
- `retainedFileCountLimit: 31`

Esto permite mantener historial diario sin hardcodear rutas por ambiente y deja una base simple para observabilidad enterprise futura.

## ExecutionLogger

La template incluye además un componente reusable llamado `ExecutionLogger` en `TemplateProject.Application/Common/Logging`.

Su propósito es centralizar:

- logs de inicio
- logs de fin
- logs de error
- serialización de request
- serialización de response
- medición de tiempo
- manejo de excepciones
- `TraceId`

Con esto evitamos repetir `try/catch`, `LogInformation` y `LogError` en controllers, servicios y futuras piezas de `Application` o `Infrastructure`.
El patrón recomendado en esta template es usarlo principalmente en servicios de `Application` y, cuando tenga sentido, en implementaciones de `Infrastructure`, manteniendo los controllers delgados.

### Métodos disponibles

`ExecuteAsync<T>`

- úsalo cuando la operación retorna datos
- ejemplo: `GET`, `POST`, `PUT`

`ExecuteAsync`

- úsalo cuando la operación no retorna datos
- ejemplo: `DELETE`, procesos internos o comandos sin response

### Ejemplo de uso

```csharp
var response = await _executionLogger.ExecuteAsync(
    title: "CreatePerson",
    startMessage: "Starting person creation process.",
    endMessage: "Person created successfully.",
    errorMessage: "Error creating person.",
    request: request,
    action: async () =>
    {
        return await _repository.CreateAsync(person);
    });
```

Y para operaciones sin retorno:

```csharp
await _executionLogger.ExecuteAsync(
    title: "DeletePerson",
    startMessage: "Starting person deletion process.",
    endMessage: "Person deleted successfully.",
    errorMessage: "Error deleting person.",
    request: new { PersonId = id },
    action: async () =>
    {
        await _repository.DeleteAsync(id);
    });
```

El componente usa `System.Text.Json`, `Stopwatch`, `ILogger<ExecutionLogger>` y relanza excepciones con `throw;`, dejándolo listo para evolucionar luego a observabilidad enterprise más avanzada sin meter AOP ni comportamientos mágicos.

## Instalar el template

Desde la raíz del repositorio:

```bash
dotnet new install .
```

Luego podrás crear una nueva solución con:

```bash
dotnet new clean-arch-template -n admin-micro-be
```

## Compilación

```bash
dotnet build
```
template-clean-architecture-dotnet
