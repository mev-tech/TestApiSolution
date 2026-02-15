namespace TestApi.Tests;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class WorkflowValidationTests
{
    private const string WorkflowPath = "../../../.github/workflows/ci.yml";

    private dynamic LoadWorkflow()
    {
        var yaml = File.ReadAllText(WorkflowPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<dynamic>(yaml);
    }

    [Fact]
    public void Workflow_File_Should_Exist()
    {
        Assert.True(File.Exists(WorkflowPath), $"Workflow file should exist at {WorkflowPath}");
    }

    [Fact]
    public void Workflow_Should_Have_Valid_YAML_Syntax()
    {
        var exception = Record.Exception(() => LoadWorkflow());
        Assert.Null(exception);
    }

    [Fact]
    public void Workflow_Should_Have_Name()
    {
        var workflow = LoadWorkflow();
        Assert.NotNull(workflow);
        Assert.True(workflow.ContainsKey("name"), "Workflow should have a name");
        Assert.Equal("CI", workflow["name"]);
    }

    [Fact]
    public void Workflow_Should_Trigger_On_Push_To_Master()
    {
        var workflow = LoadWorkflow();
        Assert.True(workflow.ContainsKey("on"), "Workflow should have trigger configuration");
        var triggers = workflow["on"];
        Assert.True(triggers.ContainsKey("push"), "Workflow should trigger on push");
        var push = triggers["push"];
        Assert.True(push.ContainsKey("branches"), "Push trigger should specify branches");
        var branches = push["branches"] as List<object>;
        Assert.NotNull(branches);
        Assert.Contains("master", branches.Select(b => b.ToString()));
    }

    [Fact]
    public void Workflow_Should_Trigger_On_Pull_Request_To_Master()
    {
        var workflow = LoadWorkflow();
        var triggers = workflow["on"];
        Assert.True(triggers.ContainsKey("pullRequest"), "Workflow should trigger on pull_request");
        var pullRequest = triggers["pullRequest"];
        Assert.True(pullRequest.ContainsKey("branches"), "Pull request trigger should specify branches");
        var branches = pullRequest["branches"] as List<object>;
        Assert.NotNull(branches);
        Assert.Contains("master", branches.Select(b => b.ToString()));
    }

    [Fact]
    public void Workflow_Should_Have_Concurrency_Configuration()
    {
        var workflow = LoadWorkflow();
        Assert.True(workflow.ContainsKey("concurrency"), "Workflow should have concurrency configuration");
        var concurrency = workflow["concurrency"];
        Assert.True(concurrency.ContainsKey("group"), "Concurrency should have a group");
        Assert.True(concurrency.ContainsKey("cancelInProgress"), "Concurrency should have cancel-in-progress setting");
        Assert.True((bool)concurrency["cancelInProgress"], "cancel-in-progress should be true");
    }

    [Fact]
    public void Workflow_Should_Have_Jobs()
    {
        var workflow = LoadWorkflow();
        Assert.True(workflow.ContainsKey("jobs"), "Workflow should have jobs");
        var jobs = workflow["jobs"];
        Assert.NotNull(jobs);
    }

    [Fact]
    public void Workflow_Should_Have_Dotnet_Job()
    {
        var workflow = LoadWorkflow();
        var jobs = workflow["jobs"];
        Assert.True(jobs.ContainsKey("dotnet"), "Workflow should have dotnet job");
        var dotnetJob = jobs["dotnet"];
        Assert.NotNull(dotnetJob);
        Assert.True(dotnetJob.ContainsKey("name"), "Dotnet job should have a name");
        Assert.Equal("dotnet", dotnetJob["name"]);
    }

    [Fact]
    public void Dotnet_Job_Should_Run_On_Ubuntu_Latest()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        Assert.True(dotnetJob.ContainsKey("runsOn"), "Dotnet job should specify runner");
        Assert.Equal("ubuntu-latest", dotnetJob["runsOn"]);
    }

    [Fact]
    public void Dotnet_Job_Should_Have_Steps()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        Assert.True(dotnetJob.ContainsKey("steps"), "Dotnet job should have steps");
        var steps = dotnetJob["steps"] as List<object>;
        Assert.NotNull(steps);
        Assert.NotEmpty(steps);
    }

    [Fact]
    public void Dotnet_Job_Should_Checkout_Code()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var checkoutStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Checkout");
        Assert.NotNull(checkoutStep);
        Assert.True(checkoutStep.ContainsKey("uses"), "Checkout step should use an action");
        Assert.Equal("actions/checkout@v6", checkoutStep["uses"]);
    }

    [Fact]
    public void Dotnet_Job_Should_Setup_Dotnet()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var setupStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Setup .NET");
        Assert.NotNull(setupStep);
        Assert.True(setupStep.ContainsKey("uses"), "Setup .NET step should use an action");
        Assert.Equal("actions/setup-dotnet@v5", setupStep["uses"]);
    }

    [Fact]
    public void Dotnet_Job_Should_Use_Dotnet_8_Version()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var setupStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Setup .NET");
        Assert.NotNull(setupStep);
        Assert.True(setupStep.ContainsKey("with"), "Setup .NET step should have configuration");
        var withConfig = setupStep["with"] as Dictionary<object, object>;
        Assert.NotNull(withConfig);
        Assert.True(withConfig.ContainsKey("dotnetVersion"), "Setup .NET should specify version");
        Assert.Equal("8.0.x", withConfig["dotnetVersion"]);
    }

    [Fact]
    public void Dotnet_Job_Should_Run_Format_Verification()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var formatStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Format (verify no changes)");
        Assert.NotNull(formatStep);
        Assert.True(formatStep.ContainsKey("run"), "Format step should have run command");
        Assert.Contains("dotnet format", formatStep["run"].ToString());
        Assert.Contains("--verify-no-changes", formatStep["run"].ToString());
    }

    [Fact]
    public void Dotnet_Job_Should_Restore_Dependencies()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var restoreStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Restore");
        Assert.NotNull(restoreStep);
        Assert.True(restoreStep.ContainsKey("run"), "Restore step should have run command");
        Assert.Contains("dotnet restore", restoreStep["run"].ToString());
    }

    [Fact]
    public void Dotnet_Job_Should_Build_In_Release_Configuration()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Build");
        Assert.NotNull(buildStep);
        Assert.True(buildStep.ContainsKey("run"), "Build step should have run command");
        var buildCommand = buildStep["run"].ToString();
        Assert.Contains("dotnet build", buildCommand);
        Assert.Contains("-c Release", buildCommand);
        Assert.Contains("--no-restore", buildCommand);
    }

    [Fact]
    public void Dotnet_Job_Should_Run_Tests_With_Coverage()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var testStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Test + Coverage");
        Assert.NotNull(testStep);
        Assert.True(testStep.ContainsKey("run"), "Test step should have run command");
        var testCommand = testStep["run"].ToString();
        Assert.Contains("dotnet test", testCommand);
        Assert.Contains("-c Release", testCommand);
        Assert.Contains("--no-build", testCommand);
        Assert.Contains("--collect:\"XPlat Code Coverage\"", testCommand);
    }

    [Fact]
    public void Dotnet_Job_Should_Upload_Coverage_Artifact()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var uploadStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Upload coverage artifact");
        Assert.NotNull(uploadStep);
        Assert.True(uploadStep.ContainsKey("uses"), "Upload step should use an action");
        Assert.Equal("actions/upload-artifact@v6", uploadStep["uses"]);
    }

    [Fact]
    public void Coverage_Upload_Should_Run_Always()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var uploadStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Upload coverage artifact");
        Assert.NotNull(uploadStep);
        Assert.True(uploadStep.ContainsKey("if"), "Upload step should have conditional execution");
        Assert.Equal("always()", uploadStep["if"]);
    }

    [Fact]
    public void Coverage_Upload_Should_Have_Correct_Configuration()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var uploadStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Upload coverage artifact");
        Assert.NotNull(uploadStep);
        Assert.True(uploadStep.ContainsKey("with"), "Upload step should have configuration");
        var withConfig = uploadStep["with"] as Dictionary<object, object>;
        Assert.NotNull(withConfig);
        Assert.True(withConfig.ContainsKey("name"), "Upload should specify artifact name");
        Assert.Equal("coverage", withConfig["name"]);
        Assert.True(withConfig.ContainsKey("path"), "Upload should specify path");
    }

    [Fact]
    public void Workflow_Should_Have_Docker_Job()
    {
        var workflow = LoadWorkflow();
        var jobs = workflow["jobs"];
        Assert.True(jobs.ContainsKey("docker"), "Workflow should have docker job");
        var dockerJob = jobs["docker"];
        Assert.NotNull(dockerJob);
        Assert.True(dockerJob.ContainsKey("name"), "Docker job should have a name");
        Assert.Equal("docker", dockerJob["name"]);
    }

    [Fact]
    public void Docker_Job_Should_Run_On_Ubuntu_Latest()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        Assert.True(dockerJob.ContainsKey("runsOn"), "Docker job should specify runner");
        Assert.Equal("ubuntu-latest", dockerJob["runsOn"]);
    }

    [Fact]
    public void Docker_Job_Should_Have_Steps()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        Assert.True(dockerJob.ContainsKey("steps"), "Docker job should have steps");
        var steps = dockerJob["steps"] as List<object>;
        Assert.NotNull(steps);
        Assert.NotEmpty(steps);
    }

    [Fact]
    public void Docker_Job_Should_Checkout_Code()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        var checkoutStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Checkout");
        Assert.NotNull(checkoutStep);
        Assert.True(checkoutStep.ContainsKey("uses"), "Checkout step should use an action");
        Assert.Equal("actions/checkout@v6", checkoutStep["uses"]);
    }

    [Fact]
    public void Docker_Job_Should_Build_Image()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Docker build");
        Assert.NotNull(buildStep);
        Assert.True(buildStep.ContainsKey("run"), "Docker build step should have run command");
        var buildCommand = buildStep["run"].ToString();
        Assert.Contains("docker build", buildCommand);
        Assert.Contains("-t testapi:", buildCommand);
    }

    [Fact]
    public void Docker_Build_Should_Use_Git_SHA_Tag()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Docker build");
        Assert.NotNull(buildStep);
        var buildCommand = buildStep["run"].ToString();
        Assert.Contains("${{ github.sha }}", buildCommand);
    }

    [Fact]
    public void Dotnet_Job_Should_Have_Exactly_Six_Steps()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        Assert.Equal(6, steps.Count);
    }

    [Fact]
    public void Docker_Job_Should_Have_Exactly_Two_Steps()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        Assert.Equal(2, steps.Count);
    }

    [Fact]
    public void Workflow_Should_Use_Latest_Action_Versions()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var dockerJob = workflow["jobs"]["docker"];

        var allSteps = new List<object>();
        allSteps.AddRange(dotnetJob["steps"] as List<object>);
        allSteps.AddRange(dockerJob["steps"] as List<object>);

        foreach (var step in allSteps.Cast<Dictionary<object, object>>())
        {
            if (step.ContainsKey("uses"))
            {
                var uses = step["uses"].ToString();
                if (uses.StartsWith("actions/checkout@"))
                {
                    Assert.Contains("@v6", uses);
                }
                else if (uses.StartsWith("actions/setup-dotnet@"))
                {
                    Assert.Contains("@v5", uses);
                }
                else if (uses.StartsWith("actions/upload-artifact@"))
                {
                    Assert.Contains("@v6", uses);
                }
            }
        }
    }

    [Fact]
    public void Workflow_File_Should_Not_Be_Empty()
    {
        var content = File.ReadAllText(WorkflowPath);
        Assert.False(string.IsNullOrWhiteSpace(content), "Workflow file should not be empty");
        Assert.True(content.Length > 100, "Workflow file should have substantial content");
    }

    [Fact]
    public void Workflow_Steps_Should_Have_Names()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var dockerJob = workflow["jobs"]["docker"];

        var allSteps = new List<object>();
        allSteps.AddRange(dotnetJob["steps"] as List<object>);
        allSteps.AddRange(dockerJob["steps"] as List<object>);

        foreach (var step in allSteps.Cast<Dictionary<object, object>>())
        {
            Assert.True(step.ContainsKey("name"), "Each step should have a name");
            Assert.False(string.IsNullOrWhiteSpace(step["name"].ToString()), "Step name should not be empty");
        }
    }

    [Fact]
    public void Concurrency_Group_Should_Use_PR_Number_Or_Ref()
    {
        var workflow = LoadWorkflow();
        var concurrency = workflow["concurrency"];
        var group = concurrency["group"].ToString();
        Assert.Contains("github.event.pull_request.number", group);
        Assert.Contains("github.ref", group);
        Assert.Contains("||", group);
    }

    [Fact]
    public void Workflow_Should_Reference_Correct_Solution_File()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;

        var solutionFile = "./TestApi.sln";
        var commandsRequiringSolution = new[] { "format", "restore", "build", "test" };

        foreach (var command in commandsRequiringSolution)
        {
            var step = steps.Cast<Dictionary<object, object>>()
                .FirstOrDefault(s => s.ContainsKey("run") && s["run"].ToString().Contains($"dotnet {command}"));
            if (step != null)
            {
                Assert.Contains(solutionFile, step["run"].ToString());
            }
        }
    }

    [Fact]
    public void Workflow_Should_Only_Trigger_On_Master_Branch()
    {
        var workflow = LoadWorkflow();
        var triggers = workflow["on"];

        var pushBranches = triggers["push"]["branches"] as List<object>;
        Assert.Single(pushBranches);
        Assert.Equal("master", pushBranches[0].ToString());

        var prBranches = triggers["pullRequest"]["branches"] as List<object>;
        Assert.Single(prBranches);
        Assert.Equal("master", prBranches[0].ToString());
    }

    [Fact]
    public void Format_Step_Should_Come_Before_Restore_Step()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;

        var formatIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString().Contains("Format"));

        var restoreIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString() == "Restore");

        Assert.True(formatIndex < restoreIndex, "Format step should come before Restore step");
    }

    [Fact]
    public void Restore_Step_Should_Come_Before_Build_Step()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;

        var restoreIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString() == "Restore");

        var buildIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString() == "Build");

        Assert.True(restoreIndex < buildIndex, "Restore step should come before Build step");
    }

    [Fact]
    public void Build_Step_Should_Come_Before_Test_Step()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;

        var buildIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString() == "Build");

        var testIndex = steps.Cast<Dictionary<object, object>>()
            .ToList()
            .FindIndex(s => s.ContainsKey("name") && s["name"].ToString() == "Test + Coverage");

        Assert.True(buildIndex < testIndex, "Build step should come before Test step");
    }

    [Fact]
    public void Jobs_Should_Not_Have_Dependencies()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var dockerJob = workflow["jobs"]["docker"];

        Assert.False(dotnetJob.ContainsKey("needs"), "Dotnet job should not depend on other jobs");
        Assert.False(dockerJob.ContainsKey("needs"), "Docker job should not depend on other jobs");
    }

    [Fact]
    public void Coverage_Path_Should_Include_TestResults()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var uploadStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Upload coverage artifact");

        var withConfig = uploadStep["with"] as Dictionary<object, object>;
        var path = withConfig["path"].ToString();
        Assert.Contains("TestResults", path);
    }

    [Fact]
    public void All_Dotnet_Commands_Should_Target_Solution_File()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;

        var dotnetCommands = steps.Cast<Dictionary<object, object>>()
            .Where(s => s.ContainsKey("run") && s["run"].ToString().StartsWith("dotnet"))
            .Select(s => s["run"].ToString())
            .ToList();

        foreach (var command in dotnetCommands)
        {
            if (command.Contains("format") || command.Contains("restore") ||
                command.Contains("build") || command.Contains("test"))
            {
                Assert.Contains("TestApi.sln", command);
            }
        }
    }

    [Fact]
    public void Docker_Build_Should_Use_Current_Directory()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Docker build");

        var buildCommand = buildStep["run"].ToString();
        Assert.EndsWith(" .", buildCommand.Trim());
    }

    [Fact]
    public void Workflow_Should_Have_Exactly_Two_Jobs()
    {
        var workflow = LoadWorkflow();
        var jobs = workflow["jobs"] as Dictionary<object, object>;
        Assert.Equal(2, jobs.Count);
        Assert.True(jobs.ContainsKey("dotnet"));
        Assert.True(jobs.ContainsKey("docker"));
    }

    [Fact]
    public void All_Steps_Should_Have_Either_Run_Or_Uses()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var dockerJob = workflow["jobs"]["docker"];

        var allSteps = new List<object>();
        allSteps.AddRange(dotnetJob["steps"] as List<object>);
        allSteps.AddRange(dockerJob["steps"] as List<object>);

        foreach (var step in allSteps.Cast<Dictionary<object, object>>())
        {
            var hasRun = step.ContainsKey("run");
            var hasUses = step.ContainsKey("uses");
            Assert.True(hasRun || hasUses, "Each step must have either 'run' or 'uses'");
        }
    }

    [Fact]
    public void Concurrency_Group_Should_Have_Prefix()
    {
        var workflow = LoadWorkflow();
        var concurrency = workflow["concurrency"];
        var group = concurrency["group"].ToString();
        Assert.StartsWith("ci-", group);
    }

    [Fact]
    public void Dotnet_Job_Should_Use_No_Build_Flag_For_Test()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var testStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Test + Coverage");

        var testCommand = testStep["run"].ToString();
        Assert.Contains("--no-build", testCommand);
    }

    [Fact]
    public void Dotnet_Job_Should_Use_No_Restore_Flag_For_Build()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var steps = dotnetJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Build");

        var buildCommand = buildStep["run"].ToString();
        Assert.Contains("--no-restore", buildCommand);
    }

    [Fact]
    public void Workflow_Should_Only_Use_Official_GitHub_Actions()
    {
        var workflow = LoadWorkflow();
        var dotnetJob = workflow["jobs"]["dotnet"];
        var dockerJob = workflow["jobs"]["docker"];

        var allSteps = new List<object>();
        allSteps.AddRange(dotnetJob["steps"] as List<object>);
        allSteps.AddRange(dockerJob["steps"] as List<object>);

        var usedActions = allSteps.Cast<Dictionary<object, object>>()
            .Where(s => s.ContainsKey("uses"))
            .Select(s => s["uses"].ToString())
            .ToList();

        foreach (var action in usedActions)
        {
            Assert.StartsWith("actions/", action);
        }
    }

    [Fact]
    public void Workflow_Should_Not_Have_Workflow_Dispatch_Trigger()
    {
        var workflow = LoadWorkflow();
        var triggers = workflow["on"];
        Assert.False(triggers.ContainsKey("workflowDispatch"),
            "Workflow should not have manual workflow_dispatch trigger");
    }

    [Fact]
    public void Docker_Image_Name_Should_Be_Lowercase()
    {
        var workflow = LoadWorkflow();
        var dockerJob = workflow["jobs"]["docker"];
        var steps = dockerJob["steps"] as List<object>;
        var buildStep = steps.Cast<Dictionary<object, object>>()
            .FirstOrDefault(s => s.ContainsKey("name") && s["name"].ToString() == "Docker build");

        var buildCommand = buildStep["run"].ToString();
        var imageNameMatch = System.Text.RegularExpressions.Regex.Match(buildCommand, @"-t\s+(\w+):");
        if (imageNameMatch.Success)
        {
            var imageName = imageNameMatch.Groups[1].Value;
            Assert.Equal(imageName.ToLower(), imageName);
        }
    }
}