namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "basis_universal/";

        // needed until https://github.com/BinomialLLC/basis_universal/pull/391 gets merged
        context.ReplaceTextInFiles("basis_universal/CMakeLists.txt", "project(basisu)", "project(basisu C CXX)\nset(CMAKE_CXX_STANDARD 17)");

        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "-DSAN=ON -DSTATIC=TRUE CMakeLists.txt" });
        context.ReplaceTextInFiles("basis_universal/basisu.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles("basis_universal/basisu_encoder.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.ReplaceTextInFiles("basis_universal/examples.vcxproj", "MultiThreadedDLL", "MultiThreaded");
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        var files = Directory.GetFiles(System.IO.Path.Combine(buildWorkingDir, "bin"), "basisu.exe", SearchOption.TopDirectoryOnly);
        context.CopyFile(files[0], $"{context.ArtifactsDir}/basisu.exe");
    }
}
