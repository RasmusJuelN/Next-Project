# Getting Started

This guide will help you set up the NextQuestionnaire Backend project for development, testing, or production deployment. Whether you're a developer, contributor, or end-user, this comprehensive guide covers all the necessary steps.

## Prerequisites

### Required Software

Before you begin, ensure you have the following installed on your system:

#### For Development and Deployment
- **.NET 8 SDK or later** - [Download from Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express, or Full version)
  - **Windows**: SQL Server Express or SQL Server Developer Edition
  - **Linux/macOS**: SQL Server in Docker or SQL Server Express
- **Git** - [Download from git-scm.com](https://git-scm.com/)

#### For Documentation (Optional)
- **DocFX** - For generating documentation
  ```bash
  dotnet tool install -g docfx
  ```

#### Development Tools (Recommended)
- **Visual Studio 2022** (Windows) or **Visual Studio Code** (Cross-platform)
- **SQL Server Management Studio (SSMS)** or **Azure Data Studio**
- **Postman** or similar API testing tool

### System Requirements

- **OS**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+, CentOS 7+)
- **RAM**: Minimum 4GB, Recommended 8GB+
- **Storage**: At least 2GB free space
- **Network**: Internet connection for package downloads

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/RasmusJuelN/Next-Project.git
cd Next-Project/Backend
```

### 2. Database Setup

#### Option A: SQL Server LocalDB (Windows)
LocalDB is automatically installed with Visual Studio and .NET SDK on Windows.

#### Option B: SQL Server in Docker (Cross-platform)
```bash
# Pull and run SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrongPassword123" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Option C: SQL Server Express (Windows)
Download and install SQL Server Express from Microsoft's website.

### 3. Configure Connection String

The application will automatically create a `config.json` file on first run with default settings. You can also create it manually:

```json
{
  "Database": {
    "ConnectionString": "Server=localhost,1433;Database=QuestionnaireDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  },
  "JWT": {
    "AccessTokenSecret": "your-secure-access-token-secret-key-here-minimum-32-characters",
    "RefreshTokenSecret": "your-secure-refresh-token-secret-key-here-minimum-32-characters",
    "TokenTTLMinutes": 30,
    "RenewTokenTTLDays": 30,
    "Issuer": "next.dev",
    "Audience": "next.dev"
  },
  "LDAP": {
    "Host": "your-ldap-server",
    "Port": 389,
    "FQDN": "your.domain.com",
    "BaseDN": "OU=Users,DC=your,DC=domain,DC=com",
    "SA": "admin-user",
    "SAPassword": "admin-password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "error",
      "Microsoft": "warning"
    },
    "Console": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "Microsoft.Hosting.Lifetime": "information"
      }
    },
    "FileLogger": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "API": "information",
        "Database": "information",
        "Microsoft.EntityFrameworkCore.Migrations": "information"
      },
      "Path": "./app.log"
    },
    "DBLogger": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "API": "information",
        "Database": "information",
        "Microsoft.EntityFrameworkCore.Migrations": "information"
      }
    }
  }
}
```

### 4. Build and Run

```bash
# Navigate to the API project
cd API

# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will be available at:
- **HTTPS**: https://localhost:7135
- **HTTP**: http://localhost:5284
- **Swagger UI**: https://localhost:7135 (in Development mode)

## Detailed Setup Instructions

### Development Environment Setup

#### 1. Project Structure Overview
```
Backend/
├── API/                   # Main Web API project
├── Database/              # Entity Framework Core data layer
├── Logging/               # Custom logging infrastructure
├── Settings/              # Configuration management
├── UnitTests/             # Test projects
└── docs/                  # Documentation (DocFX)
```

#### 2. Building the Solution

Using Visual Studio:
1. Open `Backend.sln` in Visual Studio
2. Set `API` as the startup project
3. Build → Build Solution (Ctrl+Shift+B)
4. Debug → Start Debugging (F5)

Using Command Line:
```bash
# From the Backend directory
dotnet build Backend.sln

# Run the API project
cd API
dotnet run
```

#### 3. Database Initialization

The application uses Entity Framework Core with automatic migrations. On first run:

1. The application will automatically create the database if it doesn't exist
2. All pending migrations will be applied
3. Default data will be seeded

To manually run migrations:
> [!NOTE]
> Ensure the desired database destination is configured in the `config.json` file beforehand.
```bash
cd Database
dotnet ef database update --startup-project ../API
```

### Configuration Guide

#### Application Settings

The application uses `config.json` for all runtime configuration. This file is automatically generated with default values on first run if it doesn't exist. The application reads all settings exclusively from this file, making configuration management simple and centralized.

#### Key Configuration Sections

##### Database Configuration
```json
{
  "Database": {
    "ConnectionString": "Your SQL Server connection string"
  }
}
```

##### Authentication Configuration
> [!NOTE]
> `TokenTTLMinutes` controls how long the access token is valid for. This is typically a short duration, such as 30 minutes. This does not reflect how long the user stays logged on for.
>
> `RenewTokenTTLDays` controls how long the refresh token is valid for. This is typically a longer duration, such as 30 days. The refresh token ultimately acts as the maximum amount of time a user can be logged in for while being inactive.
> <details>
> <summary>Why?</Summary>
> Access tokens are short-lived to minimize the risk if they are compromised. If an access token is stolen, the attacker can only use it for a limited time (e.g., 30 minutes). After that, the token expires, and the attacker would need to obtain a new one.<br /><br />
>Refresh tokens, on the other hand, are long-lived and can be used to obtain new access tokens without requiring the user to log in again. This allows users to stay logged in for longer periods (e.g., 30 days) without frequent interruptions, while still maintaining security by limiting the lifespan of access tokens. A new refresh token is also provided on each refresh, giving the user a longer grace period (e.g., 30 days) of inactivity, without getting logged out.
> </details>
```json
{
  "JWT": {
    "AccessTokenSecret": "32+ character secret key",
    "RefreshTokenSecret": "32+ character secret key",
    "TokenTTLMinutes": 30,
    "RenewTokenTTLDays": 30,
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

##### LDAP Configuration (Optional)
> [!NOTE]
> `Host` can be a domain name or IP.
>
> `FQDN` is the fully qualified domain name of the machine. If the server is an Active Directory server, this is most likely the name of the machine that hosts the AD + forest domain (e.g., `adserver.domain.com`).
>
> `BaseDN` is the base distinguished name for LDAP searches. This is typically the organizational unit (OU) where user accounts are located, along with the domain components (DC). For example, if your users are in an OU called "Users" within the domain "domain.com", the BaseDN would be `OU=Users,DC=domain,DC=com`.
>
> `SA`/`SAPassword` are the service account username and password used to bind to the LDAP server for searches. This account should have read access to the user objects in the specified BaseDN.
```json
{
  "LDAP": {
    "Host": "ldap-server-address",
    "Port": 389,
    "FQDN": "domain.com",
    "BaseDN": "OU=Users,DC=domain,DC=com",
    "SA": "service-account",
    "SAPassword": "service-account-password"
  }
}
```

##### Logging Configuration
> [!NOTE]
> The logging configuration allows you to enable or disable different logging targets (Console, File, Database) and set log levels for various components. Adjust these settings based on your environment (Development or Production) to control the verbosity and destination of log messages.
>
> `LogLevel` can be used to set the loglevel of a specific component (namespace). For example, setting `API` to `Information` will log all information level logs and above (Warning, Error, Critical) from the API project. Setting `Default` to `Error` means that only error and critical logs from all other components will be logged.
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "error",
      "Microsoft": "warning"
    },
    "Console": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "Microsoft.Hosting.Lifetime": "information"
      }
    },
    "FileLogger": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "API": "information",
        "Database": "information",
        "Microsoft.EntityFrameworkCore.Migrations": "information"
      },
      "Path": "./app.log"
    },
    "DBLogger": {
      "IsEnabled": true,
      "LogLevel": {
        "Default": "warning",
        "API": "information",
        "Database": "information",
        "Microsoft.EntityFrameworkCore.Migrations": "information"
      }
    }
  }
}
```

### Production Deployment

#### IIS Deployment (Windows)

1. **Publish the application:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure IIS:**
   - Install IIS with ASP.NET Core Module
   - Create a new website pointing to the publish folder
   - Set the application pool to "No Managed Code"

3. **Configure SSL certificate** for HTTPS

#### Docker Deployment

Create a `Dockerfile` in the API directory:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Database/Database.csproj", "Database/"]
COPY ["Logging/Logging.csproj", "Logging/"]
COPY ["Settings/Settings.csproj", "Settings/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

Build and run:
```bash
docker build -t nextquestionnaire-api .
docker run -p 8080:80 nextquestionnaire-api
```

#### Linux Service Deployment

1. **Publish the application:**
   ```bash
   dotnet publish -c Release -o /var/www/nextquestionnaire
   ```

2. **Create systemd service file** (`/etc/systemd/system/nextquestionnaire.service`):
   ```ini
   [Unit]
   Description=NextQuestionnaire API
   After=network.target

   [Service]
   Type=notify
   ExecStart=/usr/bin/dotnet /var/www/nextquestionnaire/API.dll
   Restart=always
   RestartSec=10
   KillSignal=SIGINT
   SyslogIdentifier=nextquestionnaire
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production

   [Install]
   WantedBy=multi-user.target
   ```

3. **Enable and start the service:**
   ```bash
   sudo systemctl enable nextquestionnaire
   sudo systemctl start nextquestionnaire
   ```

### Testing

#### Running Unit Tests
> [!IMPORTANT]
> 🚧 WIP 🚧

#### API Testing

The application includes Swagger UI for interactive API testing:
- **Development**: Available at the root URL (https://localhost:7135)
- **Production**: Disabled by default for security

Example API endpoints:
- `GET /api/system/health` - Health check
- `POST /api/auth/login` - User authentication
- `GET /api/questionnaire-template` - List questionnaire templates

### Troubleshooting

#### Common Issues

**Database Connection Failures:**
- Verify SQL Server is running
- Check connection string in `config.json`
- Ensure database user has appropriate permissions

**Authentication Issues:**
- Verify JWT secrets are properly configured
- Check LDAP settings if using LDAP authentication
- Ensure token expiration settings are appropriate

**Port Conflicts:**
- Default ports: 7135 (HTTPS), 5284 (HTTP)
- Modify ports in `launchSettings.json` if needed

**Missing Dependencies:**
```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

#### Logging and Diagnostics

The application provides comprehensive logging:
- **Console logs** during development
- **File logs** in the `logs` directory
- **Database logs** in the ApplicationLogs table
- **Health check endpoint** at `/api/system/health`

#### Getting Help

- **Issues**: Create an issue on the GitHub repository
- **Documentation**: Refer to the API documentation generated by DocFX
- **Logs**: Check application logs for detailed error information

### Next Steps

After successful setup:

1. **Explore the API** using Swagger UI
2. **Review the documentation** for detailed API reference
3. **Set up the frontend** application (if applicable)
4. **Configure monitoring** and alerting for production deployments
5. **Set up automated backups** for the database

### Development Workflow

For contributors and developers:

1. **Create a feature branch** from the main branch
2. **Make your changes** following the coding standards
3. **Run tests** to ensure nothing is broken
4. **Update documentation** if needed
5. **Submit a pull request** for review

The project follows standard .NET development practices and includes comprehensive unit tests to ensure code quality.