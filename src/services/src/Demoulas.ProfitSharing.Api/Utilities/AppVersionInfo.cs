using System.Reflection;
using System.Text.Json;

namespace Demoulas.ProfitSharing.Api.Utilities;

public class AppVersionInfo
{
    private const string BuildFileName = ".buildinfo.json";
    private readonly string _buildFilePath;
    private string? _buildNumber;
    private short? _buildId;
    private string? _gitHash;
    private string? _gitShortHash;

    public AppVersionInfo(IHostEnvironment hostEnvironment)
    {
        _buildFilePath = Path.Combine(hostEnvironment.ContentRootPath, BuildFileName);
    }

    public string BuildNumber
    {
        get
        {
            // Build number format should be yyyyMMdd.# (e.g. 20200308.1)
            if (string.IsNullOrWhiteSpace(_buildNumber))
            {
                if (File.Exists(_buildFilePath))
                {
                    string fileContents = File.ReadAllText(_buildFilePath);
                    if (!string.IsNullOrWhiteSpace(fileContents))
                    {
                        var buildInfo = JsonSerializer.Deserialize<BuildInfo>(fileContents);
                        _buildNumber = buildInfo?.BuildNumber;
                        _buildId = buildInfo?.BuildId;
                        _gitHash = buildInfo?.RevisionNumber;
                    }
                }

                _buildNumber = $"{DateTime.UtcNow:yyyyMMdd}.{_buildId}";
            }

            return _buildNumber;
        }
    }

    public string GitHash
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_gitHash))
            {
                string? version = "1.0.0+LOCALBUILD"; // Dummy version for local dev
                var appAssembly = typeof(AppVersionInfo).Assembly;
                var infoVerAttr = (AssemblyInformationalVersionAttribute)appAssembly
                    .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute)).FirstOrDefault()!;

                if (infoVerAttr is { InformationalVersion.Length: > 6 })
                {
                    // Hash is embedded in the version after a '+' symbol, e.g. 1.0.0+a34a913742f8845d3da5309b7b17242222d41a21
                    version = infoVerAttr.InformationalVersion;
                }
                _gitHash = version[(version.IndexOf('+') + 1)..];

            }

            return _gitHash;
        }
    }

    public string ShortGitHash
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_gitShortHash))
            {
                _gitShortHash = GitHash.Substring(GitHash.Length - 6, 6);
            }
            return _gitShortHash;
        }
    }
}
