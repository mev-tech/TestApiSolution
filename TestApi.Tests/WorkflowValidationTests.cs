namespace TestApi.Tests;

public class WorkflowValidationTests
{
    private const string RepoRoot = "..";
    private const string WorkflowsDir = ".github/workflows";

    [Fact]
    public void CiWorkflow_FileExists()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");

        // Act & Assert
        Assert.True(File.Exists(ciPath), $"CI workflow file should exist at {ciPath}");
    }

    [Fact]
    public void ReleaseWorkflow_FileExists()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");

        // Act & Assert
        Assert.True(File.Exists(releasePath), $"Release workflow file should exist at {releasePath}");
    }

    [Fact]
    public void CiWorkflow_ContainsRequiredJobs()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("jobs:", content);
        Assert.Contains("dotnet:", content);
        Assert.Contains("docker:", content);
    }

    [Fact]
    public void CiWorkflow_HasCorrectTriggers()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("on:", content);
        Assert.Contains("push:", content);
        Assert.Contains("pull_request:", content);
        Assert.Contains("branches: [\"master\"]", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ContainsFormatStep()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Format (verify no changes)", content);
        Assert.Contains("dotnet format ./TestApi.sln --verify-no-changes", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ContainsBuildStep()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Build", content);
        Assert.Contains("dotnet build ./TestApi.sln -c Release", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ContainsTestStep()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Test + Coverage", content);
        Assert.Contains("dotnet test ./TestApi.sln", content);
        Assert.Contains("XPlat Code Coverage", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_UsesDotNet8()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Setup .NET", content);
        Assert.Contains("dotnet-version: \"8.0.x\"", content);
    }

    [Fact]
    public void CiWorkflow_DockerJob_BuildsImage()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Docker build", content);
        Assert.Contains("docker build -t testapi:", content);
    }

    [Fact]
    public void CiWorkflow_HasConcurrencyControl()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("concurrency:", content);
        Assert.Contains("group:", content);
        Assert.Contains("cancel-in-progress: true", content);
    }

    [Fact]
    public void ReleaseWorkflow_TriggersOnVersionTags()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("on:", content);
        Assert.Contains("push:", content);
        Assert.Contains("tags:", content);
        Assert.Contains("\"v*.*.*\"", content);
    }

    [Fact]
    public void ReleaseWorkflow_HasRequiredPermissions()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("permissions:", content);
        Assert.Contains("contents: read", content);
        Assert.Contains("packages: write", content);
    }

    [Fact]
    public void ReleaseWorkflow_LogsInToGHCR()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("Log in to GHCR", content);
        Assert.Contains("docker/login-action@v3", content);
        Assert.Contains("registry: ghcr.io", content);
    }

    [Fact]
    public void ReleaseWorkflow_BuildsAndPushesDockerImage()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("Build and push", content);
        Assert.Contains("docker build", content);
        Assert.Contains("docker push", content);
        Assert.Contains("ghcr.io", content);
    }

    [Fact]
    public void ReleaseWorkflow_TagsBothVersionAndLatest()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains(":latest", content);
        Assert.Contains("github.ref_name", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_UploadsCoverageArtifact()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Upload coverage artifact", content);
        Assert.Contains("actions/upload-artifact@v6", content);
        Assert.Contains("name: coverage", content);
        Assert.Contains("TestResults", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_RunsOnUbuntuLatest()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("runs-on: ubuntu-latest", content);
    }

    [Theory]
    [InlineData("ci.yml")]
    [InlineData("release.yml")]
    public void WorkflowFiles_AreValidYamlFormat(string fileName)
    {
        // Arrange
        var workflowPath = Path.Combine(RepoRoot, WorkflowsDir, fileName);
        var content = File.ReadAllText(workflowPath);

        // Act & Assert
        // Basic YAML validation: should start with name or on, contain proper indentation
        Assert.NotEmpty(content);
        Assert.DoesNotContain("\t", content); // YAML should use spaces, not tabs

        // Check for required top-level keys
        var hasName = content.Contains("name:");
        var hasOn = content.Contains("on:");
        Assert.True(hasName || hasOn, "Workflow should have 'name:' or 'on:' at top level");
    }

    [Fact]
    public void CiWorkflow_DotnetJob_HasRestoreStep()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Restore", content);
        Assert.Contains("dotnet restore ./TestApi.sln", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_UsesCheckoutActionV6()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("Checkout", content);
        Assert.Contains("actions/checkout@v6", content);
    }

    [Fact]
    public void ReleaseWorkflow_UsesCheckoutActionV6()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("actions/checkout@v6", content);
    }

    [Fact]
    public void CiWorkflow_HasValidWorkflowName()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("name: CI", content);
    }

    [Fact]
    public void ReleaseWorkflow_HasValidWorkflowName()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("name: Release Docker Image", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_BuildsInReleaseConfiguration()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("-c Release", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_BuildsWithNoRestore()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        // Verify build uses --no-restore to ensure restore runs first
        Assert.Contains("--no-restore", content);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_TestsWithNoBuild()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        // Verify test uses --no-build to ensure build runs first
        Assert.Contains("--no-build", content);
    }

    [Fact]
    public void CiWorkflow_DoesNotContainSensitiveInformation()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath).ToLower();

        // Act & Assert
        // Negative test: ensure no hardcoded passwords or tokens
        Assert.DoesNotContain("password:", content);
        Assert.DoesNotContain("token:", content.Replace("github_token", "").Replace("github.token", ""));
        Assert.DoesNotContain("secret:", content.Replace("secrets.", ""));
    }

    [Fact]
    public void ReleaseWorkflow_UsesGitHubToken_NotHardcodedCredentials()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        Assert.Contains("secrets.GITHUB_TOKEN", content);
        Assert.DoesNotContain("password:", content.ToLower());
    }

    [Fact]
    public void CiWorkflow_CoverageArtifact_UploadsOnlyOnAllowedConditions()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        // Verify upload artifact has if condition
        Assert.Contains("if: always()", content);
    }

    [Fact]
    public void CiWorkflow_DockerJob_UsesCurrentCommitSha()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("github.sha", content);
    }

    [Fact]
    public void ReleaseWorkflow_OnlyTriggersOnTags_NotOnAllPushes()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act & Assert
        // Should NOT trigger on branch pushes
        Assert.DoesNotContain("branches:", content);
        // Should only trigger on tags
        Assert.Contains("tags:", content);
    }

    [Fact]
    public void WorkflowsDirectory_ContainsOnlyExpectedFiles()
    {
        // Arrange
        var workflowsPath = Path.Combine(RepoRoot, WorkflowsDir);
        var files = Directory.GetFiles(workflowsPath, "*.yml");

        // Act
        var fileNames = files.Select(f => Path.GetFileName(f)).ToList();

        // Assert
        Assert.Contains("ci.yml", fileNames);
        Assert.Contains("release.yml", fileNames);
        // Verify no unexpected workflow files
        Assert.Equal(2, fileNames.Count);
    }

    [Fact]
    public void CiWorkflow_UsesDotNetSetupAction()
    {
        // Arrange
        var ciPath = Path.Combine(RepoRoot, WorkflowsDir, "ci.yml");
        var content = File.ReadAllText(ciPath);

        // Act & Assert
        Assert.Contains("actions/setup-dotnet@v5", content);
    }

    [Fact]
    public void ReleaseWorkflow_BuildsAndPushes_InCorrectOrder()
    {
        // Arrange
        var releasePath = Path.Combine(RepoRoot, WorkflowsDir, "release.yml");
        var content = File.ReadAllText(releasePath);

        // Act
        var buildIndex = content.IndexOf("docker build", StringComparison.Ordinal);
        var pushIndex = content.IndexOf("docker push", StringComparison.Ordinal);

        // Assert
        Assert.True(buildIndex > 0, "Should contain docker build");
        Assert.True(pushIndex > 0, "Should contain docker push");
        Assert.True(buildIndex < pushIndex, "docker build should come before docker push");
    }

    [Theory]
    [InlineData("ci.yml", "ubuntu-latest")]
    [InlineData("release.yml", "ubuntu-latest")]
    public void Workflows_UseConsistentRunners(string fileName, string expectedRunner)
    {
        // Arrange
        var workflowPath = Path.Combine(RepoRoot, WorkflowsDir, fileName);
        var content = File.ReadAllText(workflowPath);

        // Act & Assert
        Assert.Contains($"runs-on: {expectedRunner}", content);
    }
}