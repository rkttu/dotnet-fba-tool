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
#:property PackageReadmeFile=README.md

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
        var outputPath = Path.Combine(currentDirectory, $"Program.cs");

        if (!File.Exists(outputPath))
            return new FileInfo(outputPath);

        for (var i = 1; i < int.MaxValue; i++)
        {
            outputPath = Path.Combine(currentDirectory, $"Program{i}.cs");
            if (!File.Exists(outputPath))
                return new FileInfo(outputPath);
        }

        return new FileInfo(Path.Combine(currentDirectory, $"Program_{Guid.NewGuid():n)}.cs"));
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
    DefaultValueFactory = e => [],
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

var templateOption = new Option<FbaTemplateType>("--template", "-t")
{
    Description = "Specifies the application default template.",
    DefaultValueFactory = e => FbaTemplateType.Console,
};

rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(forceOption);
rootCommand.Options.Add(emitShebangLineOption);
rootCommand.Options.Add(sdkTypeOption);
rootCommand.Options.Add(propertiesOption);
rootCommand.Options.Add(disableAotOption);
rootCommand.Options.Add(packagesOption);
rootCommand.Options.Add(templateOption);

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

    var template = FbaRenderer.GetTemplateContents(e.GetValue(templateOption));
    template.Sdks = Enumerable.Concat(template.Sdks, (e.GetValue(sdkTypeOption) ?? Array.Empty<string>()))
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct()
        .ToList();

    var properties = e.GetValue(propertiesOption) ?? Array.Empty<string>();
    var propertyDictionary = template.Properties;

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

    var packages = e.GetValue(packagesOption) ?? Array.Empty<string>();
    var packageDictionary = template.Packages;

    foreach (var eachPackage in packages)
    {
        var firstSepIndex = eachPackage.IndexOf('@');
        if (firstSepIndex < 0)
            continue;
        var key = eachPackage.Substring(0, firstSepIndex).TrimEnd();
        var value = eachPackage.Substring(firstSepIndex + 1).TrimStart();
        if (packageDictionary.ContainsKey(key))
            packageDictionary[key] = value;
        else
            packageDictionary.Add(key, value);
    }

    var disableAot = e.GetValue(disableAotOption);
    if (disableAot)
        propertyDictionary["PublishAot"] = "False";

    var emitShebangLineFlag = e.GetValue(emitShebangLineOption);
    if (emitShebangLineFlag)
    {
        writer.WriteLine("#!/usr/bin/env dotnet");
        writer.WriteLine();
    }

    var parts = new List<string>();
    var sdkContent = string.Join(Environment.NewLine, template.Sdks.Select(x => $"#:sdk {x}")).Trim();
    if (!string.IsNullOrWhiteSpace(sdkContent))
        parts.Add(sdkContent);

    var propertiesContent = string.Join(Environment.NewLine, template.Properties.Select(x => $"#:property {x.Key}={x.Value}")).Trim();
    if (!string.IsNullOrWhiteSpace(propertiesContent))
        parts.Add(propertiesContent);

    var packagesContent = string.Join(Environment.NewLine, string.Join(Environment.NewLine, template.Packages.Select(x => $"#:package {x.Key}@{x.Value}")).Trim());
    if (!string.IsNullOrWhiteSpace(packagesContent))
        parts.Add(packagesContent);

    var contents = template.Contents;
    if (string.IsNullOrWhiteSpace(contents))
        contents = @"Console.WriteLine(""Hello, World!"");";

    parts.Add(contents);
    writer.WriteLine(string.Join(Environment.NewLine + Environment.NewLine, parts).Trim());

    if (outputStream is FileStream)
        Console.WriteLine($"// A new source code file named '{output.FullName}' has been created. (Template type: {template.Kind})");
});

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();

public enum FbaTemplateType : int
{
    Console = 0,
    GenericHost,
    WebHost,
    AspireAppHost,
}

public static class FbaRenderer
{
    public static FbaOptions GetTemplateContents(FbaTemplateType type) => type switch
    {
        FbaTemplateType.WebHost => new()
        {
            Kind = type,
            Sdks = ["Microsoft.NET.Sdk.Web"],
            Contents = """
                var builder = WebApplication.CreateBuilder(args);

                using var app = builder.Build();
                app.MapGet("/", () => "Hello, World!");
                app.Run();
                """,
        },

        FbaTemplateType.GenericHost => new()
        {
            Kind = type,
            Packages = {
                { "Microsoft.Extensions.Hosting", "10.0.0-rc.*" },
            },
            Contents = """
                using Microsoft.Extensions.Hosting;
                using Microsoft.Extensions.Logging;
                using Microsoft.Extensions.Configuration;
                using Microsoft.Extensions.DependencyInjection;

                var builder = Host.CreateApplicationBuilder(args);
                builder.Configuration.AddEnvironmentVariables();
                builder.Logging.AddConsole();
                
                using var app = builder.Build();
                app.Start();

                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Hello, World!");

                app.StopAsync().GetAwaiter().GetResult();
                """,
        },

        FbaTemplateType.AspireAppHost => new()
        {
            Kind = type,
            Sdks = ["Microsoft.NET.Sdk", "Aspire.AppHost.Sdk@9.4.2"],
            Packages = {
                { "Aspire.Hosting.AppHost", "9.4.2" },
            },
            Properties = {
                { "UserSecretsId", Guid.NewGuid().ToString() },
                { "PublishAot", "False" },
            },
            Contents = """
                using Microsoft.Extensions.Configuration;
                using Microsoft.Extensions.Hosting;

                var builder = DistributedApplication.CreateBuilder(args);
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "DOTNET_ENVIRONMENT", "Development" },
                    { "MYSQL_ROOT_PASSWORD", "aew!uwe5UTG7tzg0gqy" },
                    { "MYSQL_DATABASE", "wordpress" },
                    { "MYSQL_USER", "wordpress" },
                    { "MYSQL_PASSWORD", "vvuu42XUUGgN7thvodQZ" },
                    { "ASPNETCORE_URLS", "http://localhost:18888" },
                    { "ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:18889" },
                    { "ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", "http://localhost:18890" },
                    { "ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true" },
                });

                var mysqlRootPassword = builder.AddParameterFromConfiguration("MySqlRootPassword", "MYSQL_ROOT_PASSWORD", true);
                var mysqlDatabase = builder.AddParameterFromConfiguration("MySqlDatabase", "MYSQL_DATABASE");
                var mysqlUser = builder.AddParameterFromConfiguration("MySqlUser", "MYSQL_USER");
                var mysqlPassword = builder.AddParameterFromConfiguration("MySqlPassword", "MYSQL_PASSWORD", true);

                // MYSQL_USER, MYSQL_PASSWORD가 변경되면 볼륨을 삭제하고 다시 만들어야 함
                var mysqlContainer = builder.AddContainer("mysql", "docker.io/mysql", "latest")
                    .WithEndpoint(port: 3306, targetPort: 3306, name: "mysql-tcp")
                    .WithEnvironment("MYSQL_ROOT_PASSWORD", mysqlRootPassword)
                    .WithEnvironment("MYSQL_DATABASE", mysqlDatabase)
                    .WithEnvironment("MYSQL_USER", mysqlUser)
                    .WithEnvironment("MYSQL_PASSWORD", mysqlPassword)
                    .WithVolume("mysql-data", "/var/lib/mysql") // 볼륨은 AppHost 종료 뒤에도 그대로 유지됨
                    ;

                var mysqlEndpoint = mysqlContainer.GetEndpoint("mysql-tcp");

                _ = builder.AddContainer("phpmyadmin", "docker.io/phpmyadmin", "latest")
                    .WithHttpEndpoint(port: 8080, targetPort: 80)
                    .WithEnvironment("PMA_HOST", string.Join(':', mysqlContainer.Resource.Name, mysqlEndpoint?.TargetPort))
                    .WithEnvironment("PMA_USER", "root")
                    .WithEnvironment("PMA_PASSWORD", mysqlRootPassword)
                    .WaitFor(mysqlContainer)
                    ;

                var wordpressContainer = builder.AddContainer("wordpress", "docker.io/wordpress", "latest")
                    .WithHttpEndpoint(port: 8081, targetPort: 80, name: "wp-http")
                    .WithEnvironment("WORDPRESS_DB_HOST", string.Join(':', mysqlContainer.Resource.Name, mysqlEndpoint?.TargetPort))
                    .WithEnvironment("WORDPRESS_DB_NAME", mysqlDatabase)
                    .WithEnvironment("WORDPRESS_DB_USER", mysqlUser)
                    .WithEnvironment("WORDPRESS_DB_PASSWORD", mysqlPassword)
                    .WithVolume("wp-data", "/var/www/html") // 볼륨은 AppHost 종료 뒤에도 그대로 유지됨
                    .WaitFor(mysqlContainer)
                    ;

                var wordpressHttp = wordpressContainer.GetEndpoint("wp-http");

                wordpressContainer.WithEnvironment("WORDPRESS_CONFIG_EXTRA", @"
                if (isset($_SERVER['HTTP_X_FORWARDED_PROTO']) && $_SERVER['HTTP_X_FORWARDED_PROTO'] === 'https') {
                    $_SERVER['HTTPS'] = 'on';
                }
                ");

                builder.Build().Run();
                """,
        },

        _ => new()
        {
            Kind = FbaTemplateType.Console,
            Contents = """
                Console.WriteLine("Hello, World!"),
                """,
        },
    };
}

public sealed class FbaOptions
{
    public required FbaTemplateType Kind { get; init; }
    public List<string> Sdks { get; set; } = [];
    public Dictionary<string, string> Packages { get; set; } = [];
    public Dictionary<string, string> Properties { get; set; } = [];
    public required string Contents { get; set; } = string.Empty;
}
