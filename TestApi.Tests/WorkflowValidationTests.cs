using YamlDotNet.RepresentationModel;

namespace TestApi.Tests;

/// <summary>
/// Tests to validate GitHub Actions workflow configurations.
/// Verifies CI and Release workflows have correct structure, triggers, and required jobs.
/// </summary>
public class WorkflowValidationTests
{
    private static string GetSolutionRoot()
    {
        var assemblyLocation = typeof(WorkflowValidationTests).Assembly.Location;
        var assemblyDir = Path.GetDirectoryName(assemblyLocation)!;
        // Navigate from TestApi.Tests/bin/Release/net8.0 to solution root (4 levels up)
        return Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", ".."));
    }

    private static string CiWorkflowPath => Path.Combine(GetSolutionRoot(), ".github", "workflows", "ci.yml");
    private static string ReleaseWorkflowPath => Path.Combine(GetSolutionRoot(), ".github", "workflows", "release.yml");

    [Fact]
    public void CiWorkflow_ShouldExist()
    {
        // Arrange & Act & Assert
        Assert.True(File.Exists(CiWorkflowPath), "CI workflow file should exist");
    }

    [Fact]
    public void ReleaseWorkflow_ShouldExist()
    {
        // Arrange & Act & Assert
        Assert.True(File.Exists(ReleaseWorkflowPath), "Release workflow file should exist");
    }

    [Fact]
    public void CiWorkflow_ShouldHaveValidYamlSyntax()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();

        // Act & Assert
        var exception = Record.Exception(() => yamlStream.Load(stringReader));
        Assert.Null(exception);
    }

    [Fact]
    public void ReleaseWorkflow_ShouldHaveValidYamlSyntax()
    {
        // Arrange
        var yaml = File.ReadAllText(ReleaseWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();

        // Act & Assert
        var exception = Record.Exception(() => yamlStream.Load(stringReader));
        Assert.Null(exception);
    }

    [Fact]
    public void CiWorkflow_ShouldHaveRequiredName()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var nameNode = mapping.Children[new YamlScalarNode("name")];
        var name = ((YamlScalarNode)nameNode).Value;

        // Assert
        Assert.NotNull(name);
        Assert.NotEmpty(name);
        Assert.Equal("CI", name);
    }

    [Fact]
    public void ReleaseWorkflow_ShouldHaveRequiredName()
    {
        // Arrange
        var yaml = File.ReadAllText(ReleaseWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var nameNode = mapping.Children[new YamlScalarNode("name")];
        var name = ((YamlScalarNode)nameNode).Value;

        // Assert
        Assert.NotNull(name);
        Assert.NotEmpty(name);
    }

    [Fact]
    public void CiWorkflow_ShouldHaveDotnetJob()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var hasDotnetJob = jobsNode.Children.ContainsKey(new YamlScalarNode("dotnet"));

        // Assert
        Assert.True(hasDotnetJob, "CI workflow should have a 'dotnet' job");
    }

    [Fact]
    public void CiWorkflow_ShouldHaveDockerJob()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var hasDockerJob = jobsNode.Children.ContainsKey(new YamlScalarNode("docker"));

        // Assert
        Assert.True(hasDockerJob, "CI workflow should have a 'docker' job");
    }

    [Fact]
    public void CiWorkflow_ShouldTriggerOnPushToMaster()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var onNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("on")];
        var pushNode = (YamlMappingNode)onNode.Children[new YamlScalarNode("push")];
        var branchesNode = (YamlSequenceNode)pushNode.Children[new YamlScalarNode("branches")];
        var branches = branchesNode.Children.Select(n => ((YamlScalarNode)n).Value).ToList();

        // Assert
        Assert.Contains("master", branches);
    }

    [Fact]
    public void CiWorkflow_ShouldTriggerOnPullRequestToMaster()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var onNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("on")];
        var prNode = (YamlMappingNode)onNode.Children[new YamlScalarNode("pull_request")];
        var branchesNode = (YamlSequenceNode)prNode.Children[new YamlScalarNode("branches")];
        var branches = branchesNode.Children.Select(n => ((YamlScalarNode)n).Value).ToList();

        // Assert
        Assert.Contains("master", branches);
    }

    [Fact]
    public void ReleaseWorkflow_ShouldTriggerOnVersionTags()
    {
        // Arrange
        var yaml = File.ReadAllText(ReleaseWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var onNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("on")];
        var pushNode = (YamlMappingNode)onNode.Children[new YamlScalarNode("push")];
        var tagsNode = (YamlSequenceNode)pushNode.Children[new YamlScalarNode("tags")];
        var tags = tagsNode.Children.Select(n => ((YamlScalarNode)n).Value).ToList();

        // Assert
        Assert.Contains("v*.*.*", tags);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ShouldRunOnUbuntuLatest()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var dotnetJob = (YamlMappingNode)jobsNode.Children[new YamlScalarNode("dotnet")];
        var runsOn = ((YamlScalarNode)dotnetJob.Children[new YamlScalarNode("runs-on")]).Value;

        // Assert
        Assert.Equal("ubuntu-latest", runsOn);
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ShouldHaveFormatVerificationStep()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var dotnetJob = (YamlMappingNode)jobsNode.Children[new YamlScalarNode("dotnet")];
        var stepsNode = (YamlSequenceNode)dotnetJob.Children[new YamlScalarNode("steps")];

        var hasFormatStep = stepsNode.Children
            .OfType<YamlMappingNode>()
            .Any(step =>
            {
                if (step.Children.TryGetValue(new YamlScalarNode("run"), out var runNode))
                {
                    var runCommand = ((YamlScalarNode)runNode).Value;
                    return runCommand != null && runCommand.Contains("dotnet format") && runCommand.Contains("--verify-no-changes");
                }
                return false;
            });

        // Assert
        Assert.True(hasFormatStep, "Dotnet job should have a format verification step");
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ShouldHaveBuildStep()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var dotnetJob = (YamlMappingNode)jobsNode.Children[new YamlScalarNode("dotnet")];
        var stepsNode = (YamlSequenceNode)dotnetJob.Children[new YamlScalarNode("steps")];

        var hasBuildStep = stepsNode.Children
            .OfType<YamlMappingNode>()
            .Any(step =>
            {
                if (step.Children.TryGetValue(new YamlScalarNode("run"), out var runNode))
                {
                    var runCommand = ((YamlScalarNode)runNode).Value;
                    return runCommand != null && runCommand.Contains("dotnet build");
                }
                return false;
            });

        // Assert
        Assert.True(hasBuildStep, "Dotnet job should have a build step");
    }

    [Fact]
    public void CiWorkflow_DotnetJob_ShouldHaveTestStep()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var dotnetJob = (YamlMappingNode)jobsNode.Children[new YamlScalarNode("dotnet")];
        var stepsNode = (YamlSequenceNode)dotnetJob.Children[new YamlScalarNode("steps")];

        var hasTestStep = stepsNode.Children
            .OfType<YamlMappingNode>()
            .Any(step =>
            {
                if (step.Children.TryGetValue(new YamlScalarNode("run"), out var runNode))
                {
                    var runCommand = ((YamlScalarNode)runNode).Value;
                    return runCommand != null && runCommand.Contains("dotnet test");
                }
                return false;
            });

        // Assert
        Assert.True(hasTestStep, "Dotnet job should have a test step");
    }

    [Fact]
    public void CiWorkflow_DockerJob_ShouldHaveDockerBuildStep()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var jobsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("jobs")];
        var dockerJob = (YamlMappingNode)jobsNode.Children[new YamlScalarNode("docker")];
        var stepsNode = (YamlSequenceNode)dockerJob.Children[new YamlScalarNode("steps")];

        var hasDockerBuildStep = stepsNode.Children
            .OfType<YamlMappingNode>()
            .Any(step =>
            {
                if (step.Children.TryGetValue(new YamlScalarNode("run"), out var runNode))
                {
                    var runCommand = ((YamlScalarNode)runNode).Value;
                    return runCommand != null && runCommand.Contains("docker build");
                }
                return false;
            });

        // Assert
        Assert.True(hasDockerBuildStep, "Docker job should have a docker build step");
    }

    [Fact]
    public void ReleaseWorkflow_ShouldHavePackagesWritePermission()
    {
        // Arrange
        var yaml = File.ReadAllText(ReleaseWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var permissionsNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("permissions")];
        var packagesPermission = ((YamlScalarNode)permissionsNode.Children[new YamlScalarNode("packages")]).Value;

        // Assert
        Assert.Equal("write", packagesPermission);
    }

    [Fact]
    public void CiWorkflow_ShouldHaveConcurrencyControl()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var hasConcurrency = mapping.Children.ContainsKey(new YamlScalarNode("concurrency"));

        // Assert
        Assert.True(hasConcurrency, "CI workflow should have concurrency control configured");
    }

    [Fact]
    public void CiWorkflow_ShouldCancelInProgressRuns()
    {
        // Arrange
        var yaml = File.ReadAllText(CiWorkflowPath);
        var stringReader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(stringReader);
        var mapping = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        // Act
        var concurrencyNode = (YamlMappingNode)mapping.Children[new YamlScalarNode("concurrency")];
        var cancelInProgress = ((YamlScalarNode)concurrencyNode.Children[new YamlScalarNode("cancel-in-progress")]).Value;

        // Assert
        Assert.Equal("true", cancelInProgress);
    }
}