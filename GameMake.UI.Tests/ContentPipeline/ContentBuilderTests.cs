using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameMake.UI.Tests.ContentPipeline;

public class ContentBuilderTests
{
    static readonly string s_projectDir = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "../../../../GameSample/GameSample.ContentBuilder"));

    static readonly string s_solutionDir = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "../../../../"));

    [Fact]
    public void Builder_从Spritefont生成有效的Xnb()
    {
        // Arrange
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var workingDir = s_projectDir;

        // Act
        var exitCode = RunDotNet(workingDir,
            $"run --project \"{s_projectDir}\" -- build " +
            $"--workingDir \"{workingDir}\" " +
            $"-s Assets/Fonts -o \"{outputDir}\" -i \"{outputDir}/obj\"");

        // Assert
        Assert.Equal(0, exitCode);
        var xnbFiles = Directory.GetFiles(outputDir, "*.xnb", SearchOption.AllDirectories);
        Assert.Equal(7, xnbFiles.Length);
        foreach (var f in xnbFiles)
            Assert.True(new FileInfo(f).Length > 1000);

        // Cleanup
        Directory.Delete(outputDir, true);
    }

    static int RunDotNet(string workingDir, string args)
    {
        var psi = new ProcessStartInfo("dotnet", args)
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using var proc = Process.Start(psi)!;
        proc.WaitForExit(120_000);
        return proc.ExitCode;
    }
}
