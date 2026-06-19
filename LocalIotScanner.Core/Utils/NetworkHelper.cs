using System.Net;
using System.Net.Sockets;

namespace LocalIotScanner.Core.Utils
{
    public static class NetworkHelper
    {
        public static string GetLocalIPv4Subnet()
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);

                if (socket.LocalEndPoint is IPEndPoint endPoint)
                {
                    var localIp = endPoint.Address.ToString();

                    var lastDot = localIp.LastIndexOf('.');
                    return localIp.Substring(0, lastDot) + ".0/24";
                }
            }
            catch
            {

            }

            return "192.168.1.0/24"; 
        }
    }
}