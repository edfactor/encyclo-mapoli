using System.Net;
using System.Net.NetworkInformation;

namespace Demoulas.ProfitSharing.AppHost.Helpers;

public static class PortHelper
{
    public static bool IsTcpPortInUse(int port)
    {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

        IPEndPoint[] tcpListeners = ipProperties.GetActiveTcpListeners();
        return tcpListeners.Any(listener => listener.Port == port);
    }
}
