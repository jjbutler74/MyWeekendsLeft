# MyWeekendsLeft Web Frontend

A React-based frontend for the MyWeekendsLeft API.

## Tech Stack

- **React 18** with TypeScript
- **Vite** for fast development and building
- **Tailwind CSS** for styling
- **Inter** font for beautiful typography

## Getting Started

### Prerequisites

- Node.js 18+ installed
- The API running (either locally or using the deployed version)

### Installation

```bash
cd web
npm install
```

### Development

```bash
npm run dev
```

This starts the development server at http://localhost:3000

### Build

```bash
npm run build
```

The build output will be in the `dist` folder.

### Environment Variables

Copy `.env.example` to `.env.local` and adjust as needed:

```bash
cp .env.example .env.local
```

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_URL` | API base URL | `https://dev-myweekendsleft-api.azurewebsites.net` |

## Project Structure

```
web/
├── public/           # Static assets
├── src/
│   ├── components/   # React components
│   │   ├── Calculator.tsx
│   │   └── Results.tsx
│   ├── services/     # API service layer
│   │   └── api.ts
│   ├── types/        # TypeScript types
│   │   └── api.ts
│   ├── App.tsx       # Main app component
│   ├── main.tsx      # Entry point
│   └── index.css     # Global styles
├── index.html
├── tailwind.config.js
├── vite.config.ts
└── package.json
```

## Features

- Clean, modern UI with sunset/ocean color palette
- Animated count-up for results
- Visual progress bar showing weekends lived vs remaining
- Share functionality (Web Share API with clipboard fallback)
- Responsive design for mobile and desktop
- Error handling with friendly messages
