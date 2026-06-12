using LocalIotScanner.Core.Interfaces;
using LocalIotScanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace LocalIotScanner.Core.Services
{
    public class HostDiscovery : IHostDiscovery
    {
        private const int TimeoutMs = 1000;

        public async Task<IEnumerable<Device>> DiscoverActiveHostsAsync(string subnet, CancellationToken token)
        {
            var activeDevices = new List<Device>();
            var pingTasks = new List<Task<Device?>>();

            // Extract the base IP (e.g., "192.168.1" from "192.168.1.0/24")
            // Note: For a production app, consider using a library like IPNetwork2 for robust CIDR parsing.
            string baseIp = subnet.Contains('/') ? subnet.Substring(0, subnet.LastIndexOf('.')) : subnet;

            for (int i = 1; i < 255; i++)
            {
                if (token.IsCancellationRequested) break;

                string targetIp = $"{baseIp}.{i}";
                pingTasks.Add(PingHostAsync(targetIp, token));
            }

            var results = await Task.WhenAll(pingTasks);

            // Filter out nulls and return valid devices
            return results.Where(d => d != null).Select(d => d!);
        }

        private async Task<Device?> PingHostAsync(string ipAddress, CancellationToken token)
        {
            try
            {
                using var ping = new Ping();

                // SendPingAsync doesn't accept a CancellationToken directly in all overloads,
                // but we can pass the timeout.
                var reply = await ping.SendPingAsync(ipAddress, TimeoutMs);

                if (reply.Status == IPStatus.Success)
                {
                    return new Device
                    {
                        IpAddress = ipAddress,
                        DeviceType = "Unknown", // Can be refined later via MAC address lookup (OUI)
                        // MAC and Hostname require ARP/DNS lookups, which can be added in a separate enrichment step.
                    };
                }
            }
            catch (PingException)
            {
                // Host unreachable or other network error; simply ignore for the scan list.
            }

            return null;
        }
    }
}
