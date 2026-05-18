# GitFlow Enterprise para GitHub

## 1. Resumen ejecutivo del modelo

Este repositorio adopta un GitFlow moderno y liviano para GitHub, orientado a proyectos `.NET` empresariales con foco en:

- trazabilidad
- seguridad
- calidad automatizada
- releases predecibles
- operación simple para equipos medianos

El estándar usa solo dos ramas permanentes:

- `main`
- `develop`

Y cuatro tipos de ramas temporales:

- `feature/*`
- `bugfix/*`
- `release/*`
- `hotfix/*`

Los ambientes son operativos, no ramas:

- `DEV`
- `CERT`
- `PROD`

## 2. Estrategia de ramas

### Ramas permanentes

#### `main`

- contiene únicamente código productivo
- solo acepta merges desde `release/*` o `hotfix/*`
- no permite push directo
- debe estar protegida con ruleset de GitHub
- cada merge a `main` debe asociarse a una versión `SemVer`

#### `develop`

- es la rama principal de integración
- desde aquí nacen `feature/*`, `bugfix/*` y `release/*`
- no permite push directo
- debe estar protegida con ruleset de GitHub
- representa el estado desplegable hacia `DEV`

### Ramas temporales

#### `feature/*`

- propósito: nuevas funcionalidades
- origen: `develop`
- destino: `develop`
- formato: `feature/{ticket}-{descripcion-corta}`

Ejemplos:

- `feature/1234-standard-response`
- `feature/1450-person-service`

#### `bugfix/*`

- propósito: correcciones no críticas
- origen: `develop`
- destino: `develop`
- formato: `bugfix/{ticket}-{descripcion-corta}`

#### `release/*`

- propósito: estabilización breve antes de producción
- origen: `develop`
- destino: `main`
- retorno obligatorio: `develop`
- formato: `release/v{major.minor.patch}`

Cambios permitidos:

- fixes menores
- ajustes de configuración
- validación QA
- endurecimiento operativo

Cambios prohibidos:

- funcionalidades nuevas
- refactors grandes
- cambios arquitectónicos tardíos

#### `hotfix/*`

- propósito: incidentes críticos en producción
- origen: `main`
- destino: `main`
- retorno obligatorio: `develop`
- formato: `hotfix/{ticket}-{descripcion-corta}`

## 3. Reglas por rama

### `main`

- push directo bloqueado
- PR obligatorio
- mínimo `2` aprobaciones
- checks obligatorios de CI
- CodeQL obligatorio cuando aplique
- comentarios resueltos antes de merge
- historial lineal o merge strategy homogénea
- solo merge desde `release/*` o `hotfix/*`

### `develop`

- push directo bloqueado
- PR obligatorio
- mínimo `1` aprobación
- checks obligatorios de CI
- comentarios resueltos antes de merge

### `release/*`

- vida corta
- solo fixes menores
- PR obligatorio a `main`
- PR obligatorio de retorno a `develop`

### `hotfix/*`

- vida muy corta
- prioridad operativa alta
- merge a `main`
- retorno obligatorio a `develop`

## 4. Ambientes y estrategia de despliegue

### Mapeo recomendado

- `develop` -> `DEV`
- `release/*` -> `CERT`
- `main` -> `PROD`
- `hotfix/*` -> `CERT` y luego `PROD`

### Recomendación GitHub

Usar `GitHub Environments`:

- `DEV`
- `CERT`
- `PROD`

Y configurar:

- secretos por ambiente
- aprobaciones manuales para `PROD`
- restricciones de despliegue por branch

## 5. Versionamiento y tags

Usar `Semantic Versioning`.

Formato:

- `vMAJOR.MINOR.PATCH`

Ejemplos:

- `v1.0.0`
- `v1.1.0`
- `v1.1.1`

Reglas:

- `MAJOR`: breaking changes
- `MINOR`: nuevas capacidades compatibles
- `PATCH`: fixes

## 6. Políticas de protección y calidad

### Estándar propuesto

- bloquear push directo en `main` y `develop`
- exigir PR obligatorio
- exigir checks exitosos
- exigir reviewers
- exigir comentarios resueltos
- eliminar ramas temporales tras merge
- habilitar Dependabot
- habilitar Code Scanning con CodeQL
- proteger secrets fuera del repositorio

### Configuración concreta en GitHub Rulesets

#### Ruleset para `main`

- target branches: `main`
- restrict creations: `false`
- restrict updates: `true`
- restrict deletions: `true`
- require pull request: `true`
- required approvals: `2`
- require code owner review: `true`
- dismiss stale approvals: `true`
- require conversation resolution: `true`
- require status checks: `true`
- required checks:
  - `build-and-test`
  - `codeql`
- block force pushes: `true`

#### Ruleset para `develop`

- target branches: `develop`
- restrict updates: `true`
- require pull request: `true`
- required approvals: `1`
- require conversation resolution: `true`
- require status checks: `true`
- required checks:
  - `build-and-test`

## 7. Estrategia de Pull Requests

Todo PR debe incluir:

- objetivo del cambio
- ticket relacionado
- impacto técnico
- validaciones realizadas
- riesgos conocidos
- evidencia si aplica

Estrategia recomendada:

- PRs pequeños
- ramas cortas
- `squash merge` para `feature/*` y `bugfix/*`
- consistencia de estrategia de merge en todo el repositorio

## 8. Implementación recomendada en GitHub

### Configuraciones del repositorio

- activar `Automatically delete head branches`
- activar `Vulnerability alerts`
- activar `Dependabot security updates`
- activar `Code scanning`
- usar `CODEOWNERS`
- usar `pull_request_template.md`

### GitHub Actions recomendadas

- CI de build y test para `.NET`
- CodeQL para análisis estático de seguridad
- workflow de release o tagging cuando el proceso de versionado esté formalizado

## 9. Ejemplos prácticos

### Convención de commits

- `feat: add person service`
- `fix: correct person validation`
- `refactor: move execution logger to application service`
- `docs: add github gitflow standard`
- `chore: update github workflows`

### Ejemplo de nombres de rama

- `feature/2451-swagger-enterprise`
- `bugfix/2479-serilog-config`
- `release/v1.0.0`
- `hotfix/2510-prod-null-reference`

### Flujo real nueva funcionalidad

1. partir desde `develop`
2. crear `feature/*`
3. abrir PR a `develop`
4. pasar CI
5. aprobar
6. mergear
7. eliminar rama

## 10. Escenarios operativos end-to-end

### Feature normal

1. `develop` -> `feature/*`
2. desarrollo local
3. PR a `develop`
4. CI y revisión
5. merge y despliegue a `DEV`

### Release

1. `develop` -> `release/vX.Y.Z`
2. validación en `CERT`
3. PR a `main`
4. aprobación
5. despliegue a `PROD`
6. merge de retorno a `develop`

### Hotfix

1. `main` -> `hotfix/*`
2. corrección urgente
3. PR a `main`
4. despliegue controlado
5. retorno a `develop`

## 11. Checklist de implementación

- crear ramas `main` y `develop`
- configurar `GitHub Rulesets` para `main` y `develop`
- activar `Automatically delete head branches`
- activar `Dependabot`
- activar `CodeQL`
- usar `CODEOWNERS`
- usar `pull_request_template.md`
- exigir CI verde antes de merge
- usar `DEV`, `CERT` y `PROD` como GitHub Environments
- documentar versión objetivo antes de crear `release/*`
