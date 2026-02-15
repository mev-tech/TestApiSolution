# TestApi

A minimal, production-ready ASP.NET Core 8.0 Web API built with .NET's minimal APIs pattern.

## Overview

TestApi is a lightweight REST API that demonstrates modern ASP.NET Core development practices. It includes:

- **Minimal APIs** — Lightweight endpoint definitions without traditional controllers
- **Swagger/OpenAPI** — Interactive API documentation with Swagger UI
- **Docker support** — Multi-stage Dockerfile and Docker Compose configuration
- **PostgreSQL integration** — Database infrastructure ready for use
- **Comprehensive CI/CD** — GitHub Actions for automated testing, building, and Docker image deployment
- **Automated code review** — CodeRabbit AI integration for PR reviews
- **Dependency management** — Dependabot for automated security and feature updates
- **Branch protection** — GitHub rulesets enforcing code quality on the master branch

## Quick Start

### Prerequisites

- **.NET 8.0** or later ([download](https://dotnet.microsoft.com/download))
- **Docker & Docker Compose** (optional, for containerized development)

### Running Locally

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project TestApi/TestApi.csproj

# API will be available at https://localhost:5001
# Swagger UI at https://localhost:5001/swagger
```

### Running with Docker Compose

```bash
# Start API + PostgreSQL
docker-compose up

# API will be available at http://localhost:8080
# PostgreSQL at localhost:5432
```

## API Endpoints

### Weather Forecast

```http
GET /weatherforecast
```

Returns a 5-day weather forecast with random temperatures.

**Response Example:**
```json
[
  {
    "date": "2026-02-15",
    "temperatureC": 12,
    "temperatureF": 53,
    "summary": "Mild"
  }
]
```

**Parameters:** None

**Status Codes:**
- `200 OK` — Successfully retrieved forecast

## Project Structure

```text
TestApiSolution/
├── TestApi/                    # Main API project
│   ├── Program.cs              # Application entry point & endpoint definitions
│   ├── appsettings.json        # Application configuration
│   └── TestApi.http            # HTTP request examples
├── TestApi.Tests/              # xUnit test project
├── .github/
│   ├── workflows/              # GitHub Actions CI/CD workflows
│   ├── scripts/                # GitHub automation scripts
│   └── dependabot.yml          # Dependency update automation
├── Dockerfile                  # Multi-stage Docker build
└── docker-compose.yml          # Local development stack
```

## Development

### Running Tests

```bash
# Run all tests with coverage
dotnet test TestApi.sln -c Release --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test TestApi.Tests/TestApi.Tests.csproj
```

### Code Formatting

The project enforces code style via `dotnet format`:

```bash
# Check formatting (fails if changes needed)
dotnet format ./TestApi.sln --verify-no-changes

# Auto-fix formatting
dotnet format ./TestApi.sln
```

All code formatting is verified in CI; PRs with formatting violations will fail.

### Making Changes

1. **Create a feature branch** from `master`
   ```bash
   git checkout -b feat/your-feature-name
   ```

2. **Make your changes** and test locally
   ```bash
   dotnet build
   dotnet test
   dotnet format ./TestApi.sln
   ```

3. **Push your branch** and open a pull request

4. **Wait for checks**
   - **CI (dotnet)** — Tests and code formatting verification
   - **CI (docker)** — Docker image builds successfully
   - **CodeRabbit AI** — Automated code review

5. **Address feedback** from CodeRabbit; once resolved, it auto-approves the PR

6. **Merge** when all checks pass

## CI/CD Pipeline

### GitHub Actions Workflows

#### CI Workflow (`.github/workflows/ci.yml`)

Runs on every push to `master` and pull requests targeting `master`:

1. **Format verification** — Ensures code matches style guidelines
2. **Build** — Compiles the solution in Release mode
3. **Test** — Runs xUnit tests and collects code coverage
4. **Docker build** — Verifies Dockerfile builds successfully

#### Release Workflow (`.github/workflows/release.yml`)

Automatically triggered when you push a Git tag matching `v*.*.*`:

- Builds Docker image
- Pushes to GitHub Container Registry (GHCR)
- Tags with both the version and `latest`

**To release a new version:**
```bash
git tag v1.0.0
git push origin v1.0.0
```

### Branch Protection Rules

The `master` branch is protected by GitHub rulesets requiring:

- ✅ All status checks must pass (CI dotnet & docker jobs)
- ✅ At least 1 approval (handled by CodeRabbit AI review)
- ❌ No force pushes
- ❌ No direct pushes (all changes via PR)

### Dependency Updates

Dependabot automatically creates PRs for:

- **NuGet packages** — Updated weekly (major versions excluded)
- **GitHub Actions** — Updated weekly

Review and merge these PRs to keep dependencies fresh and secure.

## Configuration

### Application Settings

Edit `TestApi/appsettings.json` for configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Database Connection

PostgreSQL connection is configured in `docker-compose.yml`:

- **Host:** `db`
- **Port:** `5432`
- **Database:** `appdb`
- **User:** `app`
- **Password:** `app`

Update `appsettings.json` to add a connection string when database integration is needed.

### Docker Environment

Customize `docker-compose.yml` to adjust:

- API port (default: 8080)
- Database credentials
- Database name
- Health check settings

## Code Quality

### CodeRabbit AI Review

Every PR gets an automated code review from CodeRabbit focusing on:

- C# conventions and .NET naming guidelines
- Async/await best practices
- Null safety (nullable reference types enabled)
- Input validation on API endpoints
- Error handling and HTTP status codes
- Test coverage and quality (Arrange-Act-Assert pattern)
- GitHub Actions workflow security

The review blocks the PR if issues are found; once you address the comments, CodeRabbit auto-approves.

### Code Coverage

Test coverage is collected and uploaded as artifacts in the CI pipeline. Check the coverage reports in the CI job artifacts to identify untested code.

## Deployment

### Docker Image

Build locally:

```bash
docker build -t testapi:latest .
```

Run:

```bash
docker run -p 8080:8080 testapi:latest
```

### Production Considerations

For production deployment:

1. Update `appsettings.json` with production settings
2. Use a production-grade database (managed PostgreSQL, Azure SQL, etc.)
3. Configure HTTPS with proper certificates
4. Set appropriate logging levels
5. Implement health check endpoints
6. Use container orchestration (Kubernetes, App Service, etc.)

## Troubleshooting

### "Port 8080 already in use"

Change the port in `docker-compose.yml` or stop the conflicting container:

```bash
docker-compose down
```

### "Connection refused" when connecting to database

Ensure PostgreSQL is healthy:

```bash
docker-compose logs db
```

Check that the container is running and health checks pass.

### CI tests fail locally but pass in pipeline

Ensure you're using the same .NET version:

```bash
dotnet --version
```

The project requires .NET 8.0.x.

## Contributing

1. Follow the branching strategy: `feat/`, `chore/`, `fix/` prefixes
2. Ensure all tests pass locally before pushing
3. Code must be properly formatted
4. Address CodeRabbit feedback on PRs
5. Keep commits focused and descriptive

## License

This project is unlicensed. See your repository settings for licensing details.

## Support

For issues or questions:

1. Check existing [GitHub Issues](https://github.com/mev-tech/TestApiSolution/issues)
2. Create a new issue with a clear description
3. Include error messages and steps to reproduce

## Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [Swagger/OpenAPI](https://swagger.io/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions](https://github.com/features/actions)
