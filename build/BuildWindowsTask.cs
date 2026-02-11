namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        BuildForArchitecture(context, "x64", "windows-x64");
        BuildForArchitecture(context, "ARM64", "windows-arm64");
    }

    private void BuildForArchitecture(BuildContext context, string arch, string rid)
    {
        var buildWorkingDir = $"basis_universal/{rid}";
        context.CreateDirectory(buildWorkingDir);
        context.ReplaceTextInFiles("basis_universal/CMakeLists.txt", "project(basisu)", "project(basisu C CXX)\nset(CMAKE_CXX_STANDARD 17)");

        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = $"-DSAN=ON -A {arch} -DSTATIC=TRUE -S .." });

        context.ReplaceTextInFiles($"{buildWorkingDir}/basisu.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles($"{buildWorkingDir}/basisu_encoder.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles($"{buildWorkingDir}/examples.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        foreach (var file in Directory.GetFiles(buildWorkingDir, "*", SearchOption.AllDirectories))
        {
            context.Information($"{file}");
        }
        var files = Directory.GetFiles(System.IO.Path.Combine(buildWorkingDir, "bin"), "basisu.exe", SearchOption.TopDirectoryOnly);
        context.CreateDirectory($"{context.ArtifactsDir}/{rid}");
        context.CopyFile(files[0], $"{context.ArtifactsDir}/{rid}/basisu.exe");
    }
}
