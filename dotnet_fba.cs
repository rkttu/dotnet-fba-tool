#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk

#:property PublishAot=False
#:property OutputType=Exe
#:property TargetFramework=net10.0

#:property ToolCommandName=dotnet-fba
#:property PackAsTool=True
#:property PackageId=dotnet-fba
#:property PackageOutputPath=nupkg
#:property Version=1.0.0
#:property Authors=rkttu
#:property Description=File-based app source scaffolding tool
#:property PackageTags=dotnet;file based app;fba

#:package System.CommandLine.Hosting@0.4.0-alpha.25306.1

// NuGet tool packaging steps (summary)
// 1) Remove the existing 'projects' directory if present. (The convert command does not overwrite existing directories.)
// 2) Run: dotnet project convert .\dotnet_fba.cs -o .\projects
// 3) Run: dotnet pack .\projects
// 4) Inspect the generated package in .\projects\nupkg\*.nupkg

using System.CommandLine;
using System.Text;

var rootCommand = new RootCommand("File-based app source scaffolding tool");

var outputOption = new Option<FileInfo>("--output", "-o")
{
    Description = "Output file path. Use '-' to write to standard output.",
    DefaultValueFactory = e =>
    {
        var currentDirectory = Environment.CurrentDirectory;

        for (var i = 1; i < int.MaxValue; i++)
        {
            var outputPath = Path.Combine(currentDirectory, $"Program{i}.cs");
            if (!File.Exists(outputPath))
                return new FileInfo(outputPath);
        }

        return new FileInfo(Path.Combine(currentDirectory, $"Program_{Guid.NewGuid().ToString("n")}.cs"));
    },
};

var forceOption = new Option<bool>("--force")
{
    Description = "Overwrite the output file if it already exists.",
    DefaultValueFactory = e => false,
};

var emitShebangLineOption = new Option<bool>("--shebang", "-sh")
{
    Description = "Emit a Unix shebang line.",
    DefaultValueFactory = e => false,
};

var sdkTypeOption = new Option<string[]>("--sdk")
{
    Description = "SDK(s) to include.",
    DefaultValueFactory = e => ["Microsoft.NET.Sdk"],
};

var propertiesOption = new Option<string[]>("--property", "-p")
{
    Description = "Override project properties.",
    DefaultValueFactory = e => Array.Empty<string>(),
};

var disableAotOption = new Option<bool>("--disable-aot")
{
    Description = "Disable AOT (adds PublishAot=False).",
    DefaultValueFactory = e => false,
};

var packagesOption = new Option<string[]>("--nuget")
{
    Description = "NuGet packages to reference.",
    DefaultValueFactory = e => Array.Empty<string>(),
};

rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(forceOption);
rootCommand.Options.Add(emitShebangLineOption);
rootCommand.Options.Add(sdkTypeOption);
rootCommand.Options.Add(propertiesOption);
rootCommand.Options.Add(disableAotOption);
rootCommand.Options.Add(packagesOption);

rootCommand.SetAction(e =>
{
    var output = e.GetValue(outputOption);
    if (output == null)
    {
        Console.Error.WriteLine("Output file was not specified.");
        Environment.Exit(2);
    }

    if (output.Exists && !e.GetValue(forceOption))
    {
        Console.Error.WriteLine("Refusing to overwrite the output file. Use --force to override.");
        Environment.Exit(2);
    }

    using var outputStream = output.Name.Trim().Equals("-", StringComparison.OrdinalIgnoreCase) ?
        Console.OpenStandardOutput() : output.Open(FileMode.Create, FileAccess.Write);
    using var writer = new StreamWriter(outputStream, new UTF8Encoding(false));

    var sdkOptions = new List<string>();
    var sdkTypes = (e.GetValue(sdkTypeOption) ?? Array.Empty<string>())
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Where(x => !string.Equals("Microsoft.NET.Sdk", x, StringComparison.OrdinalIgnoreCase))
        .Distinct();
    foreach (var eachSdk in sdkTypes)
        sdkOptions.Add($"#:sdk {eachSdk}");

    var propertiesOptions = new List<string>();
    var properties = e.GetValue(propertiesOption) ?? Array.Empty<string>();
    var propertyDictionary = new Dictionary<string, string>();
    var disableAot = e.GetValue(disableAotOption);
    if (disableAot)
        propertyDictionary.Add("PublishAot", "False");

    foreach (var eachProperty in properties)
    {
        var firstSepIndex = eachProperty.IndexOf('=');
        if (firstSepIndex < 0)
            continue;
        var key = eachProperty.Substring(0, firstSepIndex).TrimEnd();
        var value = eachProperty.Substring(firstSepIndex + 1).TrimStart();
        if (propertyDictionary.ContainsKey(key))
            propertyDictionary[key] = value;
        else
            propertyDictionary.Add(key, value);
    }

    foreach (var eachProperty in propertyDictionary)
        propertiesOptions.Add($"#:property {eachProperty.Key}={eachProperty.Value}");

    var packagesOptions = new List<string>();
    var packages = e.GetValue(packagesOption) ?? Array.Empty<string>();
    foreach (var eachPackage in packages)
        packagesOptions.Add($"#:package {eachPackage}");

    var topLevelProgram = new List<string>
    {
        "Console.WriteLine(\"Hello, World!\");"
    };

    var emitShebangLineFlag = e.GetValue(emitShebangLineOption);
    if (emitShebangLineFlag)
    {
        writer.WriteLine("#!/usr/bin/env dotnet");
        writer.WriteLine();
    }

    writer.WriteLine(string.Join(Environment.NewLine + Environment.NewLine,
        string.Join(Environment.NewLine, sdkOptions),
        string.Join(Environment.NewLine, propertiesOptions),
        string.Join(Environment.NewLine, packagesOptions),
        string.Join(Environment.NewLine, topLevelProgram)).Trim());
});

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();
