namespace TestApi.Tests;

/// <summary>
/// Tests to validate the project structure and required configuration files exist.
/// These tests ensure the project maintains its expected structure.
/// </summary>
public class ProjectStructureTests
{
    private static string GetSolutionRoot()
    {
        var assemblyLocation = typeof(ProjectStructureTests).Assembly.Location;
        var assemblyDir = Path.GetDirectoryName(assemblyLocation)!;
        // Navigate from TestApi.Tests/bin/Release/net8.0 to solution root (4 levels up)
        return Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", ".."));
    }

    [Fact]
    public void Dockerfile_ShouldExist()
    {
        // Arrange
        var dockerfilePath = Path.Combine(GetSolutionRoot(), "Dockerfile");

        // Act & Assert
        Assert.True(File.Exists(dockerfilePath), "Dockerfile should exist in the project root");
    }

    [Fact]
    public void DockerCompose_ShouldExist()
    {
        // Arrange
        var dockerComposePath = Path.Combine(GetSolutionRoot(), "docker-compose.yml");

        // Act & Assert
        Assert.True(File.Exists(dockerComposePath), "docker-compose.yml should exist in the project root");
    }

    [Fact]
    public void ReadMe_ShouldExist()
    {
        // Arrange
        var readmePath = Path.Combine(GetSolutionRoot(), "README.md");

        // Act & Assert
        Assert.True(File.Exists(readmePath), "README.md should exist in the project root");
    }

    [Fact]
    public void CiWorkflowDirectory_ShouldExist()
    {
        // Arrange
        var workflowDir = Path.Combine(GetSolutionRoot(), ".github", "workflows");

        // Act & Assert
        Assert.True(Directory.Exists(workflowDir), ".github/workflows directory should exist");
    }

    [Fact]
    public void CiWorkflow_ShouldExist()
    {
        // Arrange
        var ciPath = Path.Combine(GetSolutionRoot(), ".github", "workflows", "ci.yml");

        // Act & Assert
        Assert.True(File.Exists(ciPath), "CI workflow should exist");
    }

    [Fact]
    public void ReleaseWorkflow_ShouldExist()
    {
        // Arrange
        var releasePath = Path.Combine(GetSolutionRoot(), ".github", "workflows", "release.yml");

        // Act & Assert
        Assert.True(File.Exists(releasePath), "Release workflow should exist");
    }

    [Fact]
    public void MainApiProject_ShouldExist()
    {
        // Arrange
        var apiProjectPath = Path.Combine(GetSolutionRoot(), "TestApi", "TestApi.csproj");

        // Act & Assert
        Assert.True(File.Exists(apiProjectPath), "Main API project file should exist");
    }

    [Fact]
    public void ProgramCs_ShouldExist()
    {
        // Arrange
        var programPath = Path.Combine(GetSolutionRoot(), "TestApi", "Program.cs");

        // Act & Assert
        Assert.True(File.Exists(programPath), "Program.cs should exist in the API project");
    }

    [Fact]
    public void AppSettings_ShouldExist()
    {
        // Arrange
        var appSettingsPath = Path.Combine(GetSolutionRoot(), "TestApi", "appsettings.json");

        // Act & Assert
        Assert.True(File.Exists(appSettingsPath), "appsettings.json should exist");
    }

    [Fact]
    public void DependabotConfig_ShouldExist()
    {
        // Arrange
        var dependabotPath = Path.Combine(GetSolutionRoot(), ".github", "dependabot.yml");

        // Act & Assert
        Assert.True(File.Exists(dependabotPath), "dependabot.yml should exist for automated updates");
    }

    [Fact]
    public void ScriptsDirectory_ShouldExist()
    {
        // Arrange
        var scriptsDir = Path.Combine(GetSolutionRoot(), ".github", "scripts");

        // Act & Assert
        Assert.True(Directory.Exists(scriptsDir), ".github/scripts directory should exist");
    }

    [Fact]
    public void CreateRulesetsScript_ShouldExist()
    {
        // Arrange
        var scriptPath = Path.Combine(GetSolutionRoot(), ".github", "scripts", "create-rulesets.sh");

        // Act & Assert
        Assert.True(File.Exists(scriptPath), "create-rulesets.sh script should exist");
    }

    [Fact]
    public void ReadMe_ShouldNotBeEmpty()
    {
        // Arrange
        var readmePath = Path.Combine(GetSolutionRoot(), "README.md");

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
        var dockerComposePath = Path.Combine(GetSolutionRoot(), "docker-compose.yml");

        // Act
        var content = File.ReadAllText(dockerComposePath);

        // Assert
        Assert.Contains("api:", content);
    }

    [Fact]
    public void DockerCompose_ShouldContainDatabaseService()
    {
        // Arrange
        var dockerComposePath = Path.Combine(GetSolutionRoot(), "docker-compose.yml");

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
        var programPath = Path.Combine(GetSolutionRoot(), "TestApi", "Program.cs");

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
        var programPath = Path.Combine(GetSolutionRoot(), "TestApi", "Program.cs");

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
        var testProjectPath = Path.Combine(GetSolutionRoot(), "TestApi.Tests", "TestApi.Tests.csproj");

        // Act
        var content = File.ReadAllText(testProjectPath);

        // Assert
        Assert.Contains("<TargetFramework>net8.0</TargetFramework>", content);
    }

    [Fact]
    public void TestProject_ShouldHaveNullableEnabled()
    {
        // Arrange
        var testProjectPath = Path.Combine(GetSolutionRoot(), "TestApi.Tests", "TestApi.Tests.csproj");

        // Act
        var content = File.ReadAllText(testProjectPath);

        // Assert
        Assert.Contains("<Nullable>enable</Nullable>", content);
    }
}