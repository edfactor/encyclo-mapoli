using Microsoft.Extensions.Configuration;

namespace YEMatch.YEMatch;

internal static class Config
{
    public static string CreateDataDirectory()
    {
        // used to get BaseDataDirectory for writing log files
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();

        string baseDir = config["BaseDataDirectory"] ?? Path.Combine("/tmp", "ye");
        Directory.CreateDirectory(baseDir);

        string dataDirectory = Path.Combine(baseDir, $"{DateTime.Now:dd-MMM-HH-mm}");
        Directory.CreateDirectory(dataDirectory);
        Console.WriteLine($"Directory created: file:///{dataDirectory}");
        return dataDirectory;
    }
}
