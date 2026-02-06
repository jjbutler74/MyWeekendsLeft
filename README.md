# MyWeekendsLeft

[![Build and Deploy](https://github.com/jjbutler74/MyWeekendsLeft/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/jjbutler74/MyWeekendsLeft/actions/workflows/build-and-deploy.yml)

A service to inspire people to get busy living! Calculate how many weekends you have left and make every one count.

## Tech Stack

- **API**: .NET 10, ASP.NET Core Web API
- **Frontend**: React 18, TypeScript, Vite, Tailwind CSS
- **Infrastructure**: Azure App Service, GitHub Actions CI/CD
- **Testing**: xUnit, Vitest, Playwright (E2E)

## Environments

| Environment | API | Frontend |
|-------------|-----|----------|
| DEV | [dev-myweekendsleft-api.azurewebsites.net](https://dev-myweekendsleft-api.azurewebsites.net) | [dev-myweekendsleft-web.azurewebsites.net](https://dev-myweekendsleft-web.azurewebsites.net) |
| UAT | [uat-myweekendsleft-api.azurewebsites.net](https://uat-myweekendsleft-api.azurewebsites.net) | [uat-myweekendsleft-web.azurewebsites.net](https://uat-myweekendsleft-web.azurewebsites.net) |

## Sample API Call

```
GET https://dev-myweekendsleft-api.azurewebsites.net/api/getweekends?age=45&gender=male&country=USA
```

## Local Development

### API

```bash
cd api/MWL
dotnet restore
dotnet run --project MWL.API
```

### Frontend

```bash
cd web
npm install
npm run dev
```

## Testing

### Run API Tests

```bash
cd api/MWL
dotnet test
```

### Run Frontend Tests

```bash
cd web
npm test              # Run once
npm run test:watch    # Watch mode
npm run test:coverage # With coverage
```

### Run E2E Tests

```bash
cd web
npx playwright test
```

## Features

- Calculate remaining weekends based on life expectancy data
- Visual dot grid showing your life in weekends
- Dark mode support
- Mobile responsive design
- 50 randomized inspirational tips
- API response caching for performance
- Rate limiting for API protection
