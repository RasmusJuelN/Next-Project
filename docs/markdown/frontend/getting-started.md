# Getting Started with Frontend Development

## Prerequisites

- **Node.js** (version 18 or later)
- **Angular CLI** (latest version)
- **npm** package manager

## Development Setup

### 1. Install Dependencies
```bash
npm install
```

### 2. Start Development Server
```bash
ng serve
```

Application available at `http://localhost:4200`

## Configuration

### Environment Files
- `src/environments/environment.ts` - Production settings
- `src/environments/environment.development.ts` - Development settings

### Mock Services
The application can use mock services for development by setting the `useMock` flag in environment files:

```typescript
// environment.development.ts
export const environment = {
    production: false,
    useMock: true,  // Use mock services instead of real API
    apiUrl: "http://localhost:4200/api"
};
```

When `useMock: true`, providers in `app.config.ts` automatically switch to mock implementations:
- AuthService → MockAuthService
- HomeService → MockHomeService
- TemplateService → MockTemplateService
- etc.

**NOTE**: Since it changes when running it, outdated mock files that is not updated can give unknown errors.

### API Proxy
For backend integration with real services and (`useMock: false`), the proxy automatically redirects `/api` calls to `https://127.0.0.1:7135`:

```bash
ng serve --configuration development
```

## Building

### Development Build
```bash
ng build
```

### Production Build
```bash
ng build --configuration production
```

**Production build features:**
- Optimized bundles with minification
- Dead code elimination
- Output files in `dist/frontend-proj/`
- Bundle size limits: 500KB warning, 1MB error for initial bundle


### Deployment
After successful production build, deploy the `dist/frontend-proj/` folder to your web server.

**For IIS Deployment:**
Angular requires a web server configuration file to handle client-side routing correctly. Place a `web.config` file in the `dist/frontend-proj/` folder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

This also requires the use of IIS modules with URL Rewrite

## Common Issues

- **Port 4200 in use**: Use `ng serve --port 4201`
- **Proxy errors**: Ensure backend is running on `https://127.0.0.1:7135`
- **Build failures**: Check TypeScript errors with `npx tsc --noEmit`
- **Bundle size warnings**: Review and optimize large dependencies