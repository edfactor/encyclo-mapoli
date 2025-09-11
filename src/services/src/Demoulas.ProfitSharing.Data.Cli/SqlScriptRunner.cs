using System.Text;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Cli;

/// <summary>
/// Encapsulates reading, tokenizing, and executing SQL files against the <see cref="ProfitSharingDbContext"/>.
/// Keeps Program.cs handlers lean (DRY) and respects SRP.
/// </summary>
internal sealed class SqlScriptRunner
{
    private readonly ProfitSharingDbContext _db;

    public SqlScriptRunner(ProfitSharingDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Executes a SQL file after performing token substitution.
    /// Optionally records a <see cref="DataImportRecord"/> when <paramref name="sourceSchema"/> is provided.
    /// </summary>
    /// <param name="sqlFile">Path to the SQL file to execute.</param>
    /// <param name="tokens">Key/value tokens to substitute in the file.</param>
    /// <param name="sourceSchema">When provided, an import record will be created.</param>
    public async Task ExecuteFileAsync(string sqlFile, IDictionary<string, string?> tokens, string? sourceSchema = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sqlFile))
        {
            throw new ArgumentNullException(nameof(sqlFile));
        }

        if (!File.Exists(sqlFile))
        {
            throw new FileNotFoundException("SQL file not found", sqlFile);
        }

        string sql = await File.ReadAllTextAsync(sqlFile, ct);
        sql = Normalize(sql);

        foreach (var (key, value) in tokens)
        {
            if (string.IsNullOrEmpty(key)) {continue;}
            sql = sql.Replace(key, value ?? string.Empty, StringComparison.Ordinal);
        }

        await _db.Database.ExecuteSqlRawAsync(sql, ct);

        if (!string.IsNullOrWhiteSpace(sourceSchema))
        {
            _db.DataImportRecords.Add(new DataImportRecord { SourceSchema = sourceSchema });
            await _db.SaveChangesAsync(ct);
        }
    }

    private static string Normalize(string sql)
    {
        // common cleanup for provided scripts
        return sql.Replace("COMMIT ;", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
    }
}
