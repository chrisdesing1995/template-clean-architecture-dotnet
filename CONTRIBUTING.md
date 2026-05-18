# Contributing

## Branching standard

Este repositorio usa un GitFlow moderno con:

- `main`
- `develop`
- `feature/*`
- `bugfix/*`
- `release/*`
- `hotfix/*`

### Naming

- `feature/{ticket}-{short-description}`
- `bugfix/{ticket}-{short-description}`
- `release/v{major.minor.patch}`
- `hotfix/{ticket}-{short-description}`

## Commit convention

Usar `Conventional Commits`.

Ejemplos:

- `feat: add person service`
- `fix: correct execution logger flow`
- `refactor: move orchestration to application service`
- `docs: update gitflow governance`
- `chore: update github workflows`

## Pull requests

- no hacer push directo a `main` ni `develop`
- abrir PR pequeño y enfocado
- esperar CI verde
- resolver comentarios antes de merge
- eliminar la rama temporal después del merge

## Quality gates

Antes de abrir PR:

```bash
dotnet restore
dotnet build
dotnet test
```
