namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "basis_universal/";
        context.StartProcessWithDocker("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "CMakeLists.txt" });
        context.StartProcessWithDocker("make", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "" });
        var files = Directory.GetFiles(System.IO.Path.Combine(buildWorkingDir, "bin"), "basisu", SearchOption.TopDirectoryOnly);
        var artifact = $"{context.ArtifactsDir}/basisu";
        context.CopyFile(files[0], artifact);
        
        var stripArguments = new ProcessArgumentBuilder();
        stripArguments.Append("--strip-unneeded");
        stripArguments.AppendQuoted(artifact);
        context.StartProcessWithDocker("strip", new ProcessSettings { WorkingDirectory = "", Arguments = stripArguments });
    }
}
