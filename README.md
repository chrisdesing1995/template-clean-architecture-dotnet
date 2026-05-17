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

Por ahora, la solución incluye únicamente la estructura base y las referencias correctas entre capas.

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
