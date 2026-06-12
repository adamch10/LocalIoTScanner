using LocalIotScanner.Core.Interfaces;
using LocalIotScanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace LocalIotScanner.Core.Services
{
    public class PortScanner : IPortScanner
    {
        private const int ConnectTimeoutMs = 500;
        private const int MaxConcurrentScans = 50;

        public async Task<IEnumerable<ScannedPort>> ScanPortsAsync(Device device, IEnumerable<int> targetPorts, CancellationToken token)
        {
            var scannedPorts = new List<ScannedPort>();
            using var semaphore = new SemaphoreSlim(MaxConcurrentScans);
            var tasks = new List<Task<ScannedPort>>();

            foreach (var port in targetPorts)
            {
                tasks.Add(CheckPortAsync(device.IpAddress, port, semaphore, token));
            }

            var results = await Task.WhenAll(tasks);
            return results.Where(p => p.State == "Open");
        }

        private async Task<ScannedPort> CheckPortAsync(string ipAddress, int portNumber, SemaphoreSlim semaphore, CancellationToken token)
        {
            await semaphore.WaitAsync(token);
            try
            {
                using var client = new TcpClient();

                // Using a cancellation token source to enforce our strict timeout
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                timeoutCts.CancelAfter(ConnectTimeoutMs);

                try
                {
                    await client.ConnectAsync(ipAddress, portNumber, timeoutCts.Token);

                    return new ScannedPort
                    {
                        PortNumber = portNumber,
                        Protocol = "TCP",
                        State = "Open",
                        DetectedService = GuessService(portNumber)
                    };
                }
                catch (OperationCanceledException)
                {
                    // Timeout occurred
                    return new ScannedPort { PortNumber = portNumber, Protocol = "TCP", State = "Closed" };
                }
                catch (SocketException)
                {
                    // Connection refused or host unreachable
                    return new ScannedPort { PortNumber = portNumber, Protocol = "TCP", State = "Closed" };
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        // Basic banner guessing. A more advanced implementation would grab the actual TCP banner.
        private string GuessService(int port) => port switch
        {
            21 => "FTP",
            22 => "SSH",
            23 => "Telnet",
            80 => "HTTP",
            443 => "HTTPS",
            1883 => "MQTT",
            8080 => "HTTP-Alt",
            _ => "Unknown"
        };
    }
}
