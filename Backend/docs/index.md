# NextQuestionnaire Backend Documentation

## Introduction

Welcome to the NextQuestionnaire Backend documentation. This project provides a comprehensive ASP.NET Core Web API backend solution for managing questionnaires, user authentication, and data persistence. Built with .NET 8, the backend follows modern architectural patterns and provides a robust foundation for questionnaire management applications.

## Project Overview

The NextQuestionnaire Backend is a multi-layered ASP.NET Core application designed to handle:

- **Questionnaire Management**: Create, manage, and deploy questionnaires with flexible templates
- **User Authentication & Authorization**: LDAP-based authentication with JWT token management
- **Comprehensive Logging**: Multi-target logging system (file, database, console)
- **Configuration Management**: Flexible settings system with default configurations
- **Data Persistence**: Entity Framework Core with SQL Server integration

## Architecture

The backend follows a clean architecture pattern with clear separation of concerns across multiple projects:

### Core Components

#### API Layer (`API/`)
The main Web API project that serves as the entry point for all HTTP requests. Built with ASP.NET Core 8.0, it includes:

- **Controllers**: RESTful API endpoints for all major functionality
  - `ActiveQuestionnaireController` - Manage active questionnaire sessions
  - `AuthController` - Handle user authentication and authorization
  - `QuestionnaireTemplateController` - Manage questionnaire templates
  - `UserController` - User management operations
  - `SystemController` - System health and configuration
  - `LogsController` - Application logging endpoints
  - `SocketController` - WebSocket connection management

- **Services**: Business logic layer containing core application services
- **Authentication**: JWT-based authentication with LDAP integration
- **Middleware**: Custom middleware for request processing and error handling

#### Database Layer (`Database/`)
Entity Framework Core-based data access layer providing:

- **Models**: Entity definitions for all database tables
- **Repository Pattern**: Data access abstraction with interfaces
- **Migrations**: Database schema versioning and updates
- **Context**: DbContext configuration and database connection management
- **Extensions**: Custom query methods and data mapping utilities

#### Logging System (`Logging/`)
Comprehensive logging infrastructure supporting multiple targets:

- **File Logging**: Structured file-based logging with rotation
- **Database Logging**: Application events stored in database
- **Console Logging**: Development and debugging output
- **Custom Log Events**: Typed logging events for different scenarios

#### Settings Management (`Settings/`)
Centralized configuration management system:

- **Configuration Models**: Strongly-typed settings classes
- **Default Configurations**: Automatic generation of default settings
- **Runtime Configuration**: Dynamic configuration updates
- **Validation**: Configuration validation and error handling

#### Testing (`UnitTests/`)
Comprehensive test suite covering:

- **Unit Tests**: Individual component testing
- **Integration Tests**: End-to-end API testing
- **Mock Services**: Test doubles for external dependencies

## Key Features

### Authentication & Security
- **LDAP Integration**: Enterprise directory service authentication
- **JWT Tokens**: Secure, stateless authentication with refresh token support
- **Role-Based Authorization**: Granular permission system
- **Security Headers**: CORS, HTTPS redirection, and security policies

### Questionnaire Management
- **Template System**: Flexible questionnaire template creation and management
- **Response Tracking**: Comprehensive response collection and analysis
- **Status Management**: Complete questionnaire lifecycle management

### Real-time Features
- **WebSocket Support**: Live communication for real-time updates
- **Session Management**: Active connection tracking and management
- **Event Broadcasting**: Real-time notifications and updates

### Data Management
- **Entity Framework Core**: Modern ORM with LINQ support
- **Repository Pattern**: Clean data access abstraction
- **Migrations**: Automated database schema management
- **Seed Data**: Default questionnaire templates and configurations

### Monitoring & Diagnostics
- **Structured Logging**: Comprehensive application event tracking
- **Health Checks**: System health monitoring endpoints
- **Error Handling**: Global exception handling with detailed error responses

## Technology Stack

- **.NET 8**: Latest .NET framework with enhanced performance
- **ASP.NET Core 8**: Modern web framework for building APIs
- **Entity Framework Core 9**: Object-relational mapping (ORM)
- **SQL Server**: Primary database engine
- **JWT Authentication**: JSON Web Token implementation
- **LDAP**: Lightweight Directory Access Protocol for authentication
- **Swagger/OpenAPI**: API documentation and testing
- **Newtonsoft.Json**: JSON serialization and deserialization

## Getting Started

The backend is designed to be easily deployable and configurable. On first run, it automatically:

1. Creates default configuration files if they don't exist
2. Sets up the database schema through Entity Framework migrations
3. Loads default questionnaire templates
4. Configures logging targets based on settings

## Documentation Structure

This documentation covers:

- **API Reference**: Detailed endpoint documentation with examples
- **Database Schema**: Entity relationships and data models
- **Configuration Guide**: Settings and deployment options
- **Development Guide**: Setup instructions for contributors
- **Architecture Details**: In-depth technical implementation details

Whether you're integrating with the API, contributing to the codebase, or deploying the application, this documentation provides comprehensive guidance for working with the NextQuestionnaire Backend.