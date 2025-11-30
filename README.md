# SocialMedia

## Overview
SocialMedia is a comprehensive .NET solution built using **Clean Architecture** principles. It serves as the backend for a social media platform, managing core entities such as **Users**, **Posts**, **Comments**, **Likes**, and **Polls**.

The service is designed to be scalable, maintainable, and testable, separating core business logic from external concerns.

## Architecture
The project is organized into the following layers:

```mermaid
graph TD
    subgraph Core
        Domain[SocialMedia.Domain]
        Application[SocialMedia.Application]
    end

    subgraph Infrastructure
        Infra[SocialMedia.Infrastructure]
        Persistence[Persistence / DB]
    end

    subgraph Presentation
        API[SocialMedia.API]
    end

    API --> Application
    Application --> Domain
    Infra --> Application
    Infra --> Domain
    Infra --> Persistence
```

- **Domain**: Contains enterprise logic and entities. No external dependencies.
- **Application**: Contains business logic and use cases (CQRS Commands and Queries). Depends only on Domain.
- **Infrastructure**: Implements interfaces defined in Application/Domain (e.g., Repositories, External Services).
- **API**: The entry point for the application (REST Controllers).

## Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB or Docker container)

### Build
To build the entire solution:
```bash
dotnet build
```

### Run
To run the API project:
```bash
dotnet run --project SocialMedia.API
```
The API will be available at `https://localhost:7057` (or similar, check console output).

## Documentation
Detailed documentation is available in the `docs/` folder:

- 📘 **[Domain Documentation](docs/DOMAIN.md)**: Class diagrams and entity relationships.
- 🚀 **[API Documentation](docs/API.md)**: Endpoints, authentication, and usage flows.
- 🏗️ **[Architecture Guide](docs/ARCHITECTURE.md)**: Deep dive into CQRS, patterns, and folder structure.
