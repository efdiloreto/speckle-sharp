using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Build;
using GlobExpressions;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string CLEAN = "clean";
const string RESTORE = "restore";
const string BUILD = "build";
const string TEST = "test";
const string FORMAT = "format";
const string ZIP = "zip";
const string BUILD_INSTALLERS = "build-installers";
const string VERSION = "version";

var arguments = new List<string>();
if (args.Length > 1)
{
  arguments = args.ToList();
  args = new[] { arguments.First() };
  arguments = arguments.Skip(1).ToList();
}

Target(
  CLEAN,
  ForEach("**/output"),
  dir =>
  {
    IEnumerable<string> GetDirectories(string d)
    {
      return Glob.Directories(".", d);
    }

    void RemoveDirectory(string d)
    {
      if (Directory.Exists(d))
      {
        Console.WriteLine(d);
        Directory.Delete(d, true);
      }
    }

    foreach (var d in GetDirectories(dir))
    {
      RemoveDirectory(d);
    }
  }
);
Target(
  VERSION,
  async () =>
  {
    var (output, _) = await ReadAsync("dotnet", "minver -v w").ConfigureAwait(false);
    output = output.Trim();
    Console.WriteLine($"Version: {output}");
    Run("echo", $"\"version={output}\" >> $GITHUB_OUTPUT");
  }
);
Target(
  FORMAT,
  () =>
  {
    Run("dotnet", "tool restore");
    Run("dotnet", "csharpier --check .");
  }
);
Target(
  RESTORE,
  Consts.Solutions,
  s =>
  {
    Run("dotnet", $"dotnet restore --locked-mode {s}");
  }
);

Target(
  BUILD,
  Consts.Solutions,
  s =>
  {
    var version = Environment.GetEnvironmentVariable("VERSION");
    var fileVersion = Environment.GetEnvironmentVariable("FILE_VERSION");
    Console.WriteLine($"Version: {version} & {fileVersion}");
    Run(
      "msbuild",
      $"{s} /p:Configuration=Release /p:RestoreLockedMode=true /p:IsDesktopBuild=false /p:NuGetRestorePackages=false /p:Version={version} /p:FileVersion={fileVersion} -v:m"
    );
  }
);

Target(
  TEST,
  DependsOn(BUILD),
  () =>
  {
    IEnumerable<string> GetFiles(string d)
    {
      return Glob.Files(".", d);
    }

    foreach (var file in GetFiles("**/*.Tests.csproj"))
    {
       Run(
      "msbuild",
      $"{file} /p:Configuration=Release /p:RestoreLockedMode=true /p:IsDesktopBuild=false /p:NuGetRestorePackages=false -v:m"
    );
    }
  }
);

Target(
  ZIP,
  Consts.Projects,
  x =>
  {
    var (path, framework) = x;

    var fullPath = Path.Combine(".", path, "bin", "Release", framework);
    if (Directory.Exists(fullPath))
    {
      foreach (var file in Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories))
      {
        Console.WriteLine(file);
      }
    }
    else
    {
      throw new InvalidOperationException("Could not find: " + fullPath);
    }
    var outputDir = Path.Combine(".", "output");
    Directory.CreateDirectory(outputDir);

    var outputPath = Path.Combine(outputDir, $"{Path.GetFileName(path)}.zip");

    Console.WriteLine($"Zipping: '{fullPath}' to '{outputPath}'");
    ZipFile.CreateFromDirectory(fullPath, outputPath);
  }
);

Target(
  BUILD_INSTALLERS,
  async () =>
  {
    var token = arguments.First();
    var runId = arguments.Skip(1).First();
    var version = arguments.Skip(2).First();
    await Github.BuildInstallers(token, runId, version).ConfigureAwait(false);
  }
);

Target("default", DependsOn(ZIP), () => Console.WriteLine("Done!"));

await RunTargetsAndExitAsync(args).ConfigureAwait(true);