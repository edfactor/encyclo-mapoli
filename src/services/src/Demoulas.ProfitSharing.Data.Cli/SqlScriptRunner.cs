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

        // Read bytes and detect BOM/encoding to provide consistent decoding across machines/editors
        byte[] bytes = await File.ReadAllBytesAsync(sqlFile, ct);
        string sql = DecodeToUtf8String(bytes);
        sql = Normalize(sql);

        foreach (var (key, value) in tokens)
        {
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

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
        if (string.IsNullOrEmpty(sql))
        {
            return sql;
        }

        // Normalize CRLF -> LF
        sql = sql.Replace("\r\n", "\n").Replace('\r', '\n');

        // Remove SQL*Plus standalone slash lines
        var lines = sql.Split('\n');
        var filtered = lines.Where(l => l.Trim() != "/");
        sql = string.Join('\n', filtered);

        // Remove common COMMIT variants
        sql = sql.Replace("COMMIT ;", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("COMMIT;", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("COMMIT\n", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("COMMIT ", string.Empty, StringComparison.OrdinalIgnoreCase);

        return sql.Trim();
    }

    private static string DecodeToUtf8String(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) {return string.Empty;}

        // Detect BOMs for UTF-8, UTF-16 (LE/BE) and UTF-32
        // UTF-8 BOM: EF BB BF
        if (bytes is [0xEF, 0xBB, 0xBF, ..])
        {
            // UTF8 with BOM -> decode as UTF8 skipping BOM
            return new UTF8Encoding(false).GetString(bytes, 3, bytes.Length - 3);
        }

        // UTF-16 LE BOM: FF FE
        if (bytes is [0xFF, 0xFE, ..])
        {
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        }

        // UTF-16 BE BOM: FE FF
        if (bytes is [0xFE, 0xFF, ..])
        {
            return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
        }

        // UTF-32 LE BOM: FF FE 00 00
        if (bytes is [0xFF, 0xFE, 0x00, 0x00, ..])
        {
            return Encoding.UTF32.GetString(bytes, 4, bytes.Length - 4);
        }

        // Try to decode as UTF8 first; fall back to ANSI (Windows-1252) if invalid
        try
        {
            var utf8 = new UTF8Encoding(false, true);
            return utf8.GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            // fall back to Windows-1252 (common on Windows) - best-effort
            return Encoding.GetEncoding(1252).GetString(bytes);
        }
    }
}
