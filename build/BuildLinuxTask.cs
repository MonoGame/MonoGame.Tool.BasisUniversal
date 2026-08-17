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
        context.StartProcessWithDocker("cmake", workingDirectory: buildWorkingDir, args: "CMakeLists.txt");
        context.StartProcessWithDocker("make", workingDirectory: buildWorkingDir, args: "");
        var files = Directory.GetFiles(System.IO.Path.Combine(buildWorkingDir, "bin"), "basisu", SearchOption.TopDirectoryOnly);
        context.CopyFile(files[0], $"{context.ArtifactsDir}/basisu");
    }
}
