namespace TestApi.Tests;

/// <summary>
/// Tests to validate the project structure and required configuration files exist.
/// These tests ensure the project maintains its expected structure.
/// </summary>
public class ProjectStructureTests
{
    [Fact]
    public void Dockerfile_ShouldExist()
    {
        // Arrange
        var dockerfilePath = "../../../Dockerfile";

        // Act & Assert
        Assert.True(File.Exists(dockerfilePath), "Dockerfile should exist in the project root");
    }

    [Fact]
    public void DockerCompose_ShouldExist()
    {
        // Arrange
        var dockerComposePath = "../../../docker-compose.yml";

        // Act & Assert
        Assert.True(File.Exists(dockerComposePath), "docker-compose.yml should exist in the project root");
    }

    [Fact]
    public void ReadMe_ShouldExist()
    {
        // Arrange
        var readmePath = "../../../README.md";

        // Act & Assert
        Assert.True(File.Exists(readmePath), "README.md should exist in the project root");
    }

    [Fact]
    public void CiWorkflowDirectory_ShouldExist()
    {
        // Arrange
        var workflowDir = "../../../.github/workflows";

        // Act & Assert
        Assert.True(Directory.Exists(workflowDir), ".github/workflows directory should exist");
    }

    [Fact]
    public void CiWorkflow_ShouldExist()
    {
        // Arrange
        var ciPath = "../../../.github/workflows/ci.yml";

        // Act & Assert
        Assert.True(File.Exists(ciPath), "CI workflow should exist");
    }

    [Fact]
    public void ReleaseWorkflow_ShouldExist()
    {
        // Arrange
        var releasePath = "../../../.github/workflows/release.yml";

        // Act & Assert
        Assert.True(File.Exists(releasePath), "Release workflow should exist");
    }

    [Fact]
    public void MainApiProject_ShouldExist()
    {
        // Arrange
        var apiProjectPath = "../../../TestApi/TestApi.csproj";

        // Act & Assert
        Assert.True(File.Exists(apiProjectPath), "Main API project file should exist");
    }

    [Fact]
    public void ProgramCs_ShouldExist()
    {
        // Arrange
        var programPath = "../../../TestApi/Program.cs";

        // Act & Assert
        Assert.True(File.Exists(programPath), "Program.cs should exist in the API project");
    }

    [Fact]
    public void AppSettings_ShouldExist()
    {
        // Arrange
        var appSettingsPath = "../../../TestApi/appsettings.json";

        // Act & Assert
        Assert.True(File.Exists(appSettingsPath), "appsettings.json should exist");
    }

    [Fact]
    public void DependabotConfig_ShouldExist()
    {
        // Arrange
        var dependabotPath = "../../../.github/dependabot.yml";

        // Act & Assert
        Assert.True(File.Exists(dependabotPath), "dependabot.yml should exist for automated updates");
    }

    [Fact]
    public void ScriptsDirectory_ShouldExist()
    {
        // Arrange
        var scriptsDir = "../../../.github/scripts";

        // Act & Assert
        Assert.True(Directory.Exists(scriptsDir), ".github/scripts directory should exist");
    }

    [Fact]
    public void CreateRulesetsScript_ShouldExist()
    {
        // Arrange
        var scriptPath = "../../../.github/scripts/create-rulesets.sh";

        // Act & Assert
        Assert.True(File.Exists(scriptPath), "create-rulesets.sh script should exist");
    }

    [Fact]
    public void ReadMe_ShouldNotBeEmpty()
    {
        // Arrange
        var readmePath = "../../../README.md";

        // Act
        var content = File.ReadAllText(readmePath);

        // Assert
        Assert.NotEmpty(content);
        Assert.Contains("TestApi", content);
    }

    [Fact]
    public void Dockerfile_ShouldNotBeEmpty()
    {
        // Arrange
        var dockerfilePath = "../../../Dockerfile";

        // Act
        var content = File.ReadAllText(dockerfilePath);

        // Assert
        Assert.NotEmpty(content);
        Assert.Contains("FROM", content); // Docker files should have FROM instruction
    }

    [Fact]
    public void DockerCompose_ShouldContainApiService()
    {
        // Arrange
        var dockerComposePath = "../../../docker-compose.yml";

        // Act
        var content = File.ReadAllText(dockerComposePath);

        // Assert
        Assert.Contains("api:", content);
    }

    [Fact]
    public void DockerCompose_ShouldContainDatabaseService()
    {
        // Arrange
        var dockerComposePath = "../../../docker-compose.yml";

        // Act
        var content = File.ReadAllText(dockerComposePath);

        // Assert
        Assert.Contains("db:", content);
        Assert.Contains("postgres", content);
    }

    [Fact]
    public void ProgramCs_ShouldContainWeatherForecastEndpoint()
    {
        // Arrange
        var programPath = "../../../TestApi/Program.cs";

        // Act
        var content = File.ReadAllText(programPath);

        // Assert
        Assert.Contains("weatherforecast", content.ToLowerInvariant());
        Assert.Contains("MapGet", content);
    }

    [Fact]
    public void ProgramCs_ShouldContainSwaggerConfiguration()
    {
        // Arrange
        var programPath = "../../../TestApi/Program.cs";

        // Act
        var content = File.ReadAllText(programPath);

        // Assert
        Assert.Contains("AddSwaggerGen", content);
        Assert.Contains("UseSwagger", content);
    }

    [Fact]
    public void TestProject_ShouldTargetNet80()
    {
        // Arrange
        var testProjectPath = "../TestApi.Tests.csproj";

        // Act
        var content = File.ReadAllText(testProjectPath);

        // Assert
        Assert.Contains("<TargetFramework>net8.0</TargetFramework>", content);
    }

    [Fact]
    public void TestProject_ShouldHaveNullableEnabled()
    {
        // Arrange
        var testProjectPath = "../TestApi.Tests.csproj";

        // Act
        var content = File.ReadAllText(testProjectPath);

        // Assert
        Assert.Contains("<Nullable>enable</Nullable>", content);
    }
}