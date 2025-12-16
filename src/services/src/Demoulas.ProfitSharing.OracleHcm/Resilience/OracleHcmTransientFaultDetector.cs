using System.Net.Http;
using System.Net.Sockets;

namespace Demoulas.ProfitSharing.OracleHcm.Resilience;

internal static class OracleHcmTransientFaultDetector
{
    public static bool IsTransient(Exception ex)
    {
        for (Exception? current = ex; current is not null; current = current.InnerException)
        {
            if (current is OperationCanceledException)
            {
                continue;
            }

            if (current is TimeoutException)
            {
                return true;
            }

            if (current is HttpRequestException)
            {
                return true;
            }

            if (current is SocketException)
            {
                return true;
            }

            if (current is System.IO.IOException)
            {
                return true;
            }

            string? typeName = current.GetType().FullName;
            if (!string.IsNullOrWhiteSpace(typeName)
                && (typeName.Contains("OracleException", StringComparison.OrdinalIgnoreCase)
                    || typeName.Contains("DbException", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            string message = current.Message;
            if (!string.IsNullOrWhiteSpace(message)
                && (message.Contains("An error occurred using the connection", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("ORA-", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("TNS", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("connection", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("network", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }
}
