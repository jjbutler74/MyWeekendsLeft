# MyWeekendsLeft — Agent Context

## Project Purpose
A .NET 10 / React 18 web service that calculates how many weekends a person has left based on
age, gender, and country-level life expectancy data. The goal is to inspire people to be intentional
with their time.

## Repository Structure
```
api/MWL/           # .NET 10 ASP.NET Core Web API (C#)
web/               # React 18, TypeScript, Vite, Tailwind CSS frontend
infrastructure/    # Terraform/HCL — Azure App Service. DO NOT MODIFY.
.github/workflows/ # CI/CD pipelines. DO NOT MODIFY.
```

## Life Expectancy Data Source
Data is sourced live from the [population.io](https://population.io/) API on every request.
- Do not hardcode life expectancy values
- Do not replace, mock, or cache this data source unless an issue explicitly requires it
- The `country` parameter uses population.io's country naming conventions — verify against their
  API docs if working on country support, as names do not always match ISO codes

## Public API Contract — DO NOT BREAK
```
GET /api/getweekends?age={int}&gender={male|female}&country={string}
```
- Parameters (`age`, `gender`, `country`) must remain unchanged
- Response shape must remain backward compatible with existing frontend consumers
- Caching and rate limiting are in place — do not remove them

## Branching Strategy
- `main` — production
- `develop` — integration branch; **all PRs must target `develop`**
- Feature branches: `agent/issue-{number}-{short-slug}`

## How to Build & Test

### API
```bash
cd api/MWL
dotnet restore
dotnet test
dotnet run --project MWL.API
```

### Frontend
```bash
cd web
npm install
npm test           # Vitest unit tests
npx playwright test  # E2E (skip in agent runs unless the issue is frontend-specific)
```

## Agent Rules
- Only modify files under `api/MWL/` or `web/` unless the issue explicitly targets something else
- Never touch `infrastructure/` or `.github/workflows/`
- Always run `dotnet test` (API changes) or `npm test` (frontend changes) before pushing
- If tests fail, do not push — comment on the issue explaining what failed
- Keep changes minimal and focused — do not refactor unrelated code
- Commit message format: `fix: <description> (closes #<issue-number>)`
- PR description must reference the issue with `Closes #<number>` and include a brief summary of what changed and why

## Environments
| Env | API | Frontend |
|-----|-----|----------|
| DEV | dev-myweekendsleft-api.azurewebsites.net | dev-myweekendsleft-web.azurewebsites.net |
| UAT | uat-myweekendsleft-api.azurewebsites.net | uat-myweekendsleft-web.azurewebsites.net |
