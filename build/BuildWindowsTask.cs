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
        BuildForArchitecture(context, "ARM64", "windows-arm64", "-DSSE=OFF");
    }

    private void BuildForArchitecture(BuildContext context, string cmakeArch, string rid, string cmakeOptions = "")
    {
        var buildWorkingDir = $"basis_universal/{rid}";
        context.CreateDirectory(buildWorkingDir);
        
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = $"-A {cmakeArch} {cmakeOptions} -DSAN=ON -DSTATIC=TRUE ../" });
        context.ReplaceTextInFiles($"{buildWorkingDir}/basisu.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles($"{buildWorkingDir}/basisu_encoder.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles($"{buildWorkingDir}/examples.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        
        context.CreateDirectory($"{context.ArtifactsDir}/{rid}");
        foreach (var file in Directory.GetFiles(System.IO.Path.Combine("basis_universal"), "*.exe", SearchOption.AllDirectories))
        {
            context.Information($"Found {file}");
        }
        var files = Directory.GetFiles(System.IO.Path.Combine("basis_universal", "bin"), "basisu.exe", SearchOption.TopDirectoryOnly);
        context.Information($"Copying {files.Length} to {context.ArtifactsDir}/{rid}");
        foreach (var file in files)
        {
            context.Information($"Copying {file} to {context.ArtifactsDir}/{rid}/basisu.exe");
        }
        context.CopyFile(files[0], $"{context.ArtifactsDir}/{rid}/basisu.exe");
    }
}
