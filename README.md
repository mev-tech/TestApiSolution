# TestApi

[![codecov](https://codecov.io/gh/mev-tech/TestApiSolution/branch/master/graph/badge.svg)](https://codecov.io/gh/mev-tech/TestApiSolution)

A minimal, production-ready ASP.NET Core 8.0 Web API built with .NET's minimal APIs pattern.

## Overview

TestApi is a lightweight REST API that demonstrates modern ASP.NET Core development practices. It includes:

- **Minimal APIs** — Lightweight endpoint definitions without traditional controllers
- **Swagger/OpenAPI** — Interactive API documentation with Swagger UI
- **Structured logging** — Serilog with switchable sinks (Seq, ELK+APM, CloudWatch)
- **Docker support** — Multi-stage Dockerfile and Docker Compose configuration with profiles
- **PostgreSQL integration** — Database infrastructure ready for use
- **AWS Lambda** — Three environments (dev/staging/prod) deployed via GitHub Actions
- **Comprehensive CI/CD** — GitHub Actions for automated testing, building, and deployment
- **Automated code review** — CodeRabbit AI integration for PR reviews
- **Dependency management** — Dependabot for automated security and feature updates
- **Branch protection** — GitHub rulesets enforcing code quality on the master branch

## Quick Start

### Prerequisites

- **.NET 8.0** or later ([download](https://dotnet.microsoft.com/download))
- **Docker & Docker Compose** (optional, for containerized development)

### Running Locally (without Docker)

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project TestApi/TestApi.csproj

# API will be available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

### Running with Docker Compose

Use the `start-local.sh` script to choose your logging stack:

```bash
# Default: API + PostgreSQL + Seq (lightweight structured log viewer)
./start-local.sh

# Same as above, explicit
./start-local.sh seq

# ELK stack: API + PostgreSQL + Elasticsearch + Kibana
./start-local.sh elk

# ELK + APM: API + PostgreSQL + Elasticsearch + Kibana + APM Server
./start-local.sh apm

# Everything: all services (Seq + ELK + APM)
./start-local.sh all

# Stop all containers
./start-local.sh down

# Follow logs
./start-local.sh logs

# Restart with a specific stack (down + up)
./restart-local.sh [seq|elk|apm|all]
```

**Service URLs when running locally:**

| Service   | URL                        | Profile    |
|-----------|----------------------------|------------|
| API       | http://localhost:8000      | always     |
| Swagger   | http://localhost:8000/swagger | always  |
| Seq       | http://localhost:8081      | `seq`      |
| Kibana    | http://localhost:5601      | `elk`/`apm`|
| APM Server| http://localhost:8200      | `apm`      |
| Portainer | http://localhost:9000      | always     |

> **Seq login:** admin / admin

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

**Status Codes:**
- `200 OK` — Successfully retrieved forecast

### Health Check

```http
GET /healthz
```

Returns `200 OK` with `{ "status": "healthy" }`. Used for liveness probes.

## Project Structure

```text
TestApiSolution/
├── TestApi/                        # Main API project
│   ├── Program.cs                  # Entry point, endpoints, Serilog setup
│   ├── appsettings.json            # Base config (Console JSON sink)
│   ├── appsettings.Development.json# Local dev (Seq/ELK URLs, Debug level)
│   ├── appsettings.Staging.json    # Staging (CloudWatch via Console)
│   ├── appsettings.Production.json # Production (CloudWatch via Console)
│   ├── aws-lambda-tools-defaults.json # Lambda deploy defaults
│   └── TestApi.http                # HTTP request examples
├── TestApi.Tests/                  # xUnit test project
├── .github/
│   ├── workflows/
│   │   ├── ci.yml                  # Tests + coverage on every PR
│   │   ├── deploy.yml              # Multi-env Lambda deploy
│   │   ├── deploy-lambda.yml       # Reusable deploy workflow
│   │   └── release.yml             # Docker image release on tag
│   └── dependabot.yml              # Dependency update automation
├── start-local.sh                  # Start Docker Compose with chosen logging stack
├── restart-local.sh                # Restart local environment
├── Dockerfile                      # Multi-stage Docker build
└── docker-compose.yml              # Local development stack with profiles
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

### Code Coverage

This project uses **Coverlet** for code coverage collection. Coverage reports are generated during test execution and uploaded as CI artifacts.

**Local Coverage Reports:**
```bash
# Run tests with coverage
dotnet test TestApi.sln -c Release --collect:"XPlat Code Coverage"

# View coverage report
# Coverage reports are located in TestApi.Tests/TestResults/
# Open the .cobertura.xml file with a compatible viewer (e.g., ReportGenerator)
```

**CI Coverage:**
- Coverage reports are collected on every push to `master` and pull requests
- Reports are available as CI artifacts in the Actions tab
- 107 tests passing with comprehensive edge case coverage

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

## Structured Logging

Logging is powered by **Serilog** and the active sink is selected via the `LoggingSink` configuration key.

| `LoggingSink` value | Sink | Use case |
|---------------------|------|----------|
| *(unset)*           | Console JSON | Lambda → CloudWatch Logs |
| `seq`               | Seq at configured URL | Local dev (lightweight) |
| `elk`               | Elasticsearch at configured URL | Local dev (full ELK) |

The `start-local.sh` script sets `LoggingSink` automatically based on the profile you choose. Docker Compose reads it via `${LOGGING_SINK:-seq}`.

**Manual override:**
```bash
# Force ELK sink without using the script
LOGGING_SINK=elk docker compose --profile elk up
```

## Configuration

### Application Settings

| File | Environment | Purpose |
|------|-------------|---------|
| `appsettings.json` | All | Base config, Console JSON sink |
| `appsettings.Development.json` | Local | Debug level, Seq/ELK URLs |
| `appsettings.Staging.json` | Lambda Staging | Information level, Console only |
| `appsettings.Production.json` | Lambda Production | Warning level, Console only |

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

- API port (default: 8000)
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
docker run -p 8000:8000 testapi:latest
```

### AWS Lambda (Multi-environment ✅)

The API is deployed as three **AWS Lambda Functions** — one per environment — each with a public Function URL.

| Environment | Lambda function   | Trigger |
|-------------|-------------------|---------|
| Development | `TestApi-dev`     | Manual (`workflow_dispatch`) |
| Staging     | `TestApi-staging` | Push to `master` |
| Production  | `TestApi-prod`    | Push tag `v*.*.*` |

**Details:**
- Lambda runtime: .NET 8.0
- Region: `eu-north-1`
- Logging: structured JSON to stdout → CloudWatch Logs (free tier)
- Reusable deploy workflow in `.github/workflows/deploy-lambda.yml`

**Trigger a manual dev deploy:**

Go to Actions → "Deploy" → "Run workflow" → select environment `Development`.

**Trigger a production release:**
```bash
git tag v1.0.0
git push origin v1.0.0
```

**Required IAM Permissions for Deployment:**

- `lambda:CreateFunction`
- `lambda:UpdateFunctionCode`
- `lambda:UpdateFunctionConfiguration`
- `lambda:CreateFunctionUrlConfig`
- `lambda:UpdateFunctionUrlConfig`
- `iam:PassRole`

**Note:** On AWS accounts created after October 2025, public Function URLs require both `lambda:InvokeFunctionUrl` AND `lambda:InvokeFunction` permissions on the resource policy.

### Production Considerations

For production deployment:

1. Update `appsettings.json` with production settings
2. Use a production-grade database (managed PostgreSQL, Azure SQL, etc.)
3. Configure HTTPS with proper certificates
4. Set appropriate logging levels
5. Implement health check endpoints
6. Use container orchestration (Kubernetes, App Service, etc.)

## Troubleshooting

### "Port already in use"

Stop all local containers:

```bash
./start-local.sh down
# or
docker compose --profile seq --profile elk --profile apm down
```

### "Connection refused" when connecting to database

Ensure PostgreSQL is healthy:

```bash
./start-local.sh logs
# or
docker compose logs db
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
