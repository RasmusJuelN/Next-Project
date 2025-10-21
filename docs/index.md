# NextQuestionnaire Project Documentation

## Introduction

Welcome to the NextQuestionnaire project documentation. This is a full-stack application for managing questionnaires, featuring a modern Angular frontend and a robust ASP.NET Core backend. The application provides a comprehensive solution for creating, managing, and deploying questionnaires with user authentication and data persistence.

## Project Overview

The NextQuestionnaire project consists of two main components:

### 🎨 Frontend (Angular)
A modern, responsive web application built with Angular that provides:
- Interactive questionnaire interfaces
- User-friendly administration panels
- Real-time data visualization
- Mobile-responsive design

### ⚙️ Backend (ASP.NET Core)
A robust API backend built with .NET 8 that handles:
- **Questionnaire Management**: Create, manage, and deploy questionnaires with flexible templates
- **User Authentication & Authorization**: LDAP-based authentication with JWT token management
- **Comprehensive Logging**: Multi-target logging system (file, database, console)
- **Configuration Management**: Flexible settings system with default configurations
- **Data Persistence**: Entity Framework Core with SQL Server integration

## Technology Stack

### Frontend
- **Angular**: Modern TypeScript-based web framework
- **Tailwind CSS**: Utility-first CSS framework for styling
- **TypeScript**: Type-safe JavaScript development

### Backend
- **.NET 8**: Latest .NET framework with enhanced performance
- **ASP.NET Core 8**: Modern web framework for building APIs
- **Entity Framework Core 9**: Object-relational mapping (ORM)
- **SQL Server**: Primary database engine
- **JWT Authentication**: JSON Web Token implementation
- **LDAP**: Lightweight Directory Access Protocol for authentication

## Documentation Sections

### [Frontend Documentation](./markdown/frontend/index.md)
Explore the Angular frontend application:
- Component architecture
- Development setup
- Build and deployment
- User interface guidelines

### [Backend Documentation](./markdown/backend/index.md)
Dive into the ASP.NET Core backend:
- API reference and endpoints
- Database schema and models
- Authentication and authorization
- Configuration and deployment

### [API Reference]
Comprehensive API documentation with detailed endpoint specifications, request/response examples, and integration guides.

## Getting Started

Whether you're a developer looking to contribute, an integrator working with the API, or an administrator deploying the application, our documentation provides comprehensive guidance for working with the NextQuestionnaire project.

Choose the relevant section above to get started with your specific needs.

## Project Structure

```
NextQuestionnaire/
├── angular-src/          # Angular frontend application
├── Backend/              # ASP.NET Core backend
│   ├── API/             # Web API project
│   ├── Database/        # Data access layer
│   ├── Logging/         # Logging infrastructure
│   ├── Settings/        # Configuration management
│   └── UnitTests/       # Test suite
└── docs/                # Documentation (this site)
```

This documentation is designed to help you understand, develop, deploy, and integrate with the NextQuestionnaire platform effectively.