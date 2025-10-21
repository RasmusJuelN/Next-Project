# Structural overview
This document provides a high-level overview of the architecture and key components of the NextQuestionnaire Backend. It outlines the main projects, their responsibilities, and how they interact to form a cohesive backend solution.

> [!NOTE]
> The API project has project references to all other projects. To lessen code duplication, try placing project-specific code in their respective projects, allowing the API project to utilize it.
>
> For example, if you need a custom exception, place it in the project where it is most relevant (e.g., Database project for database-related exceptions) and then if needed, you can use it in the API project as well.

## API
- **`Backend/API/Attributes/`** - Contains custom attributes.
- **`Backend/API/Controllers/`** - Contains the API controllers that handle incoming HTTP requests and route them to the appropriate services.
- **`Backend/API/DTO/`** - Houses Data Transfer Objects used for communication between the API and other layers.
- **`Backend/API/Enums`** - Contains enums used within the API layer.
- **`Backend/API/Exceptions/`** - Contains custom exceptions for error handling.
- **`Backend/API/Extensions/`** - Contains extension methods to enhance functionality.
- **`Backend/API/Interfaces/`** - Contains interfaces defining contracts for services or other components.
- **`Backend/API/Services/`** - Contains service classes that implement business logic and interact with the data layer.
- **`Backend/API/Utils/`** - Contains utility classes and helper functions.

## Database
- **`Backend/Database/Attributes/`** - Contains custom attributes.
- **`Backend/Database/Defaults/`** - Contains JSON formatted seed data.
- **`Backend/Database/DTO/`** - Houses Data Transfer Objects used for safely exposing data from the database layer.
- **`Backend/Database/Enums/`** - Contains enums used within the database.
- **`Backend/Database/Extensions/`** - Contains extension methods to enhance functionality.
- **`Backend/Database/Interfaces/`** - Contains interfaces defining contracts for repositories or other components.
- **`Backend/Database/Migrations/`** - Contains Entity Framework Core migration files for managing database schema changes.
- **`Backend/Database/Models/`** - Contains entity models representing database tables.
- **`Backend/Database/Repositories/`** - Contains repository classes that handle data access and manipulation.
- **`Backend/Database/Utils/`** - Contains utility classes and helper functions.

## Logging
- **`Backend/Logging/DBLogger/`** - Contains the database logger implementation.
- **`Backend/Logging/Extensions/`** - Contains extension methods to enhance functionality.
- **`Backend/Logging/FileLogger/`** - Contains the file logger implementation.
- **`Backend/Logging/LogEvents/`** - Contains definitions for log events.

## Settings
- **`Backend/Settings/Defaults/`** - Contains classes for default settings values.
- **`Backend/Settings/Interfaces/`** - Contains interfaces defining contracts for settings layout.
- **`Backend/Settings/Models/`** - Contains models representing settings structure.

## UnitTests
- **`Backend/API/`** - Contains unit tests for the API project.