using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.AppHost.Helpers;
public static class PortHelper
{
    public static bool IsTcpPortInUse(int port)
    {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

        IPEndPoint[] tcpListeners = ipProperties.GetActiveTcpListeners();
        foreach (var listener in tcpListeners)
        {
            if (listener.Port == port)
            {
                return true;
            }
        }

        return false;
    }
}
