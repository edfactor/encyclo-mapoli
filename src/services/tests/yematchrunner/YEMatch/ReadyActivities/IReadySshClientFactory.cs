using Renci.SshNet;
using YEMatch.Activities;

namespace YEMatch.ReadyActivities;

/// <summary>
///     Factory interface for creating READY SSH/SFTP clients
/// </summary>
public interface IReadySshClientFactory : IDisposable
{
    /// <summary>
    ///     Gets the configured READY schema name (e.g., "tbherrmann", "mtpr3")
    /// </summary>
    string SchemaName { get; }

    /// <summary>
    ///     Gets or creates an SSH client connection to READY
    /// </summary>
    SshClient GetSshClient();

    /// <summary>
    ///     Gets or creates an SFTP client connection to READY
    /// </summary>
    SftpClient GetSftpClient();

    /// <summary>
    ///     Creates READY activities
    /// </summary>
    List<IActivity> CreateActivities(string dataDirectory);
}
