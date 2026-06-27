# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

MyWeekendsLeft calculates how many weekends a person has left to live, based on age, gender, and
country, using external life-expectancy data. The goal is to inspire people to be intentional with
their time. It's a two-part app: a .NET 10 Web API and a React 18/TypeScript frontend, deployed
independently to Azure App Service.

## Repository Structure
```
api/MWL/            # .NET 10 ASP.NET Core Web API (C#)
web/                # React 18, TypeScript, Vite, Tailwind CSS frontend
infrastructure/     # Terraform/HCL — Azure App Service.
.github/workflows/  # CI/CD pipelines.
```
The autonomous issue-triage workflow (see "Autonomous Agent Rules" below) must not touch
`infrastructure/` or `.github/workflows/`. An interactive session directed by a human may modify
either when explicitly asked (e.g. the CI/security hardening work already done in this repo).

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
- `main` — production (auto-deploys to UAT)
- `develop` — integration branch; **all PRs must target `develop`** (auto-deploys to DEV)
- Feature branches: `agent/issue-{number}-{short-slug}`

## Commands

### API (`api/MWL`)
```bash
cd api/MWL
dotnet restore
dotnet run --project MWL.API          # run locally
dotnet build --configuration Release

dotnet test                                          # all tests
dotnet test --filter "Category=Unit"                 # unit tests only
dotnet test --filter "Category=Integration"          # service integration tests
dotnet test --filter "Category=API-Integration"      # full HTTP pipeline tests
dotnet test --filter "FullyQualifiedName~WeekendsLeftShould"  # single test class
```
Tests are categorized via `[Trait("Category", "...")]` and live in `api/MWL/Tests/` as three
separate projects (`MWL.Services.UnitTests`, `MWL.Services.IntegrationTests`,
`MWL.API.IntegrationTests`).

### Frontend (`web`)
```bash
cd web
npm install
npm run dev               # Vite dev server, http://localhost:3000
npm run build              # tsc -b && vite build
npm run lint

npm test                   # vitest run (single run)
npm run test:watch         # vitest watch mode
npm run test:coverage      # vitest with coverage
npx vitest run path/to/Component.test.tsx   # single test file

npm run test:e2e           # playwright test (skip in agent runs unless the issue is frontend-specific)
npm run test:e2e:ui        # playwright with UI
```
Copy `web/.env.example` to `.env.local` to set `VITE_API_URL` for local development against a
non-default API. The frontend is served in production by a small Express server
(`web/server.cjs`) wrapping the Vite `dist/` build — `npm start` runs it.

## Architecture

### API layering
Three projects under `api/MWL`, in dependency order:
- **MWL.Models** — request/response DTOs (`WeekendsLeftRequest`, `WeekendsLeftResponse`), entities
  (`Gender`, `VersionInfo`, `LifeExpectancyApiResponse`, `CountryCsvFormat`), and FluentValidation
  validators (`WeekendsLeftRequestValidator`).
- **MWL.Services** — business logic behind interfaces in `Interface/`, implementations in
  `Implementation/`:
  - `CountriesService` loads `countries.csv` (an embedded resource, sourced from population.io)
    into an `IMemoryCache` entry keyed `"CountryData"`, mapping ISO alpha-3 codes to population.io
    country names.
  - `LifeExpectancyService` calls the external population.io life-expectancy API (URL from
    `MwlConfiguration:LifeExpectancyApiUri`), caches per-request results for 24h keyed by
    age/gender/country, and does the weekends/age-of-death math.
  - `WeekendsLeftService` orchestrates: validates the request, validates the country code via
    `CountriesService`, then delegates to `LifeExpectancyService` for the external lookup and
    calculation.
- **MWL.API** — ASP.NET Core host: `WeekendsLeftController` exposes `GET /api/getweekends/`
  (v1.0; v2.0 routes exist as 501-returning stubs) and `GET /api/version/`. `Startup.cs` wires up
  response caching, IP rate limiting (AspNetCoreRateLimit), CORS (origins from
  `Cors:AllowedOrigins` config), gzip/brotli compression, API versioning, Swagger (served at the
  app root), a Polly retry+circuit-breaker policy around the life-expectancy `HttpClient`, and
  `SecurityHeadersMiddleware`. `/health` is mapped via `MapHealthChecks`.

Startup throws on boot if `MwlConfiguration:LifeExpectancyApiUri` is missing or invalid — there's
no silent fallback.

### Frontend
Flat structure under `web/src`: `components/` (Calculator, Results, ResultsSkeleton, WeekendDots,
DarkModeToggle, ErrorBoundary, each with a co-located test in `__tests__/`), `services/api.ts`
(the API client), `types/api.ts` (response shapes mirroring the API's DTOs),
`hooks/useDarkMode.ts`. `App.tsx` composes Calculator → Results. E2E smoke tests live in
`web/e2e/`.

### CI/CD
Single workflow (`.github/workflows/build-and-deploy.yml`) builds/tests both API and frontend on
every push/PR to `main`/`develop`, then conditionally deploys:
- Push to `develop` → deploys API to DEV (`dev-myweekendsleft-api`) and frontend to DEV
  (`dev-myweekendsleft-web`), then runs Playwright E2E against the live DEV frontend.
- Push to `main` → same flow against UAT.
- `workflow_dispatch` allows manual deploys to either environment.

Build number is injected as `1.0.{github.run_number}` into `appsettings.json` post-publish and
surfaced via `/api/version`. Infra (App Service, App Service Plan, Application Insights per
environment) is defined in `infrastructure/tf/{dev,uat}` with Terraform (AzureRM ~>3.0); see
`infrastructure/README.md` and `DEPLOYMENT.md` for the full setup/rotation process.

## Autonomous Agent Rules
These apply to the unattended issue-triage/implement workflows
(`.github/workflows/issue-agent.yml`, `issue-triage-and-implement.yml`, `issue-retriage.yml`) that
act on GitHub issues without a human in the loop:
- Only modify files under `api/MWL/` or `web/` unless the issue explicitly targets something else
- Never touch `infrastructure/` or `.github/workflows/`
- Always run `dotnet test` (API changes) or `npm test` (frontend changes) before pushing
- If tests fail, do not push — comment on the issue explaining what failed
- Keep changes minimal and focused — do not refactor unrelated code
- Commit message format: `fix: <description> (closes #<issue-number>)`
- PR description must reference the issue with `Closes #<number>` and include a brief summary of
  what changed and why

## Environments
| Env | API | Frontend |
|-----|-----|----------|
| DEV | dev-myweekendsleft-api.azurewebsites.net | dev-myweekendsleft-web.azurewebsites.net |
| UAT | uat-myweekendsleft-api.azurewebsites.net | uat-myweekendsleft-web.azurewebsites.net |
