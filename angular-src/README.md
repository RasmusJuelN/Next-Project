# Frontend Project Overview

The **src** directory is the source folder for the application, containing all the code and assets required to build the project.

## src/app
The **src/app** directory is the heart of the application. It includes core components, services, models, and configuration files.

- **app.config.ts**: Contains application-wide settings and constants, and is imported in `main.ts` during initialization. This file also includes logic for toggling between mock data and real services.

  > **Note:** When using mock services, ensure that the names and return types match those of the real services to avoid runtime errors. The switch between mock and real services is controlled via a `useClass` setting in this file, based on the flags set in either `environment.development.ts` or `environment.ts`.

- **app.routes.ts**: Defines the routing configuration for the application by mapping URLs to their corresponding components for navigation.

### src/app/Core
The **src/app/Core** directory holds the core functionality of the application, including authentication and header configuration.

#### Guards and Interceptors
- **Interceptors:** Attach the JWT token from local storage to outgoing HTTP requests so that the backend can authenticate them.
- **Guards:** Protect routes from unauthorized access. For example, if a student attempts to access the dashboard, a guard verifies the connection and user role. If validation fails, the user is redirected to an error component.

## src/app/Features
The **src/app/Features** directory contains logic specific to individual pages. This includes:
- Login page components
- Home page components
- Template pages
- Other feature-specific pages

## src/app/Shared
The **src/app/Shared** directory includes reusable components & models that can be utilized across multiple parts of the application. This includes:
- User model
- Template model
- Loading component
- Pagination component

# Other stuff

## JWT Token
The JWT token is stored in **local storage** and is typically retrieved using a dedicated service.

## API and Proxy Configuration
During development, the API URL is sourced from `environment.ts` or `environment.development.ts`. When using a proxy, the URL in API calls is replaced by the proxy's URL. For example:

- Original URL: `http://localhost:4200/api`
- Proxy URL: `http://127.0.0.1:8000/api/`

## i18n Translation
The project also uses **i18n** for translations, with string resources located inside the **assets** directory.
- **da.json**: Dansk/Danish
- **en.json**: Engelsk/English
