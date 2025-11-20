# Frontend Documentation

## Project Overview

The NextQuestionnaire frontend is a modern Angular application that provides an intuitive interface for managing questionnaires. Built with Angular and styled with Tailwind CSS, it offers a responsive and user-friendly experience for both administrators and end users.

## Project Structure

The **angular-src** directory contains the complete Angular application source code and configuration files.

### src/app Directory Structure

The **src/app** directory is the heart of the application, containing all core components, services, models, and configuration files.

#### Core Configuration Files

- **app.config.ts**: Contains application-wide settings and constants, imported in `main.ts` during initialization. This file includes logic for toggling between mock data and real services.

  > **Note:** When using mock services, ensure that the names and return types match those of the real services to avoid runtime errors. The switch between mock and real services is controlled via a `useClass` setting in this file, based on the flags set in either `environment.development.ts` or `environment.ts`.

- **app.routes.ts**: Defines the routing configuration for the application by mapping URLs to their corresponding components for navigation.

#### src/app/Core
The **src/app/Core** directory holds the core functionality of the application, including authentication and header configuration.

##### Guards and Interceptors
- **Interceptors**: Automatically attach the JWT token from local storage to outgoing HTTP requests so that the backend can authenticate them.
- **Guards**: Protect routes from unauthorized access. For example, if a student attempts to access the dashboard, a guard verifies the connection and user role. If validation fails, the user is redirected to an error component.


#### src/app/Features
The **src/app/Features** directory contains logic specific to individual pages and features:
- Home/Login page component 
- Template pages
- Questionnaire management pages
- Other feature-specific components

#### src/app/Shared
The **src/app/Shared** directory includes reusable components and models that can be utilized across multiple parts of the application:
- User model
- Template model
- Loading component
- Pagination component
- Common UI components

## Key Features

### Authentication & Security
- **JWT Token Management**: Tokens are stored in local storage and automatically attached to API requests
- **Route Protection**: Guards ensure only authorized users can access specific pages
- **Role-Based Access**: Different user roles (admin, student, etc.) have different access levels

### API Integration
- **Environment-Based Configuration**: API URLs are sourced from environment files
- **Proxy Support**: Development proxy configuration for seamless backend integration
- **Mock Services**: Ability to switch between real and mock services for development/testing

### Internationalization (i18n)
The application supports multiple languages with translation files located in the **assets** directory:
- **da.json**: Danish translations
- **en.json**: English translations

### Development Configuration

#### API and Proxy Setup
During development, the API URL is configured through environment files. When using a proxy, the URL in API calls is replaced by the proxy's URL:

- Original URL: `http://localhost:4200/api`
- Proxy URL: `http://127.0.0.1:8000/api/`

## Technology Stack

- **Angular**: Modern TypeScript-based web framework
- **Tailwind CSS**: Utility-first CSS framework for responsive design
- **TypeScript**: Type-safe JavaScript development
- **Angular i18n**: Built-in internationalization support
- **JWT**: JSON Web Token for authentication

## Getting Started

### Prerequisites
- Node.js (latest LTS version)
- Angular CLI
- npm package manager

### Development Setup
0. Running Backend
1. Navigate to the `angular-src` directory
2. Install dependencies: `npm install`
3. Start the development server: `ng serve`
4. Open your browser to `http://localhost:4200`

### Building for Production
```bash
ng build --configuration production
```

## Architecture Overview

The frontend follows Angular best practices with a clear separation of concerns:

- **Core**: Authentication, guards, interceptors, and core services
- **Features**: Page-specific components and logic organized by functionality
- **Shared**: Reusable components, models, and utilities

This modular architecture ensures maintainability, testability, and scalability as the application grows.