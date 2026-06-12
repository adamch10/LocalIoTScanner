using LocalIotScanner.Core.Interfaces;
using LocalIotScanner.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Services
{
    public class AuditOrchestratorService : IAuditOrchestratorService
    {
        private readonly IHostDiscovery _discoveryStrategy;
        private readonly IPortScanner _portScanningStrategy;
        private readonly IVulnerabilityChecker _vulnerabilityChecker;
        private readonly ScannerDbContext _dbContext;
        private readonly ILogger<AuditOrchestratorService> _logger;

        public AuditOrchestratorService(
            IHostDiscovery discovery,
            IPortScanner PortScanner,
            IVulnerabilityChecker vulnerabilityChecker,
            ScannerDbContext dbContext,
            ILogger<AuditOrchestratorService> logger)
        {
            _discoveryStrategy = discovery;
            _portScanningStrategy = PortScanner;
            _vulnerabilityChecker = vulnerabilityChecker;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AuditSession> RunFullAuditAsync(string subnet, IEnumerable<int> targetPorts, CancellationToken token = default, IProgress<ScanProgress> progress = null)
        {
            _logger.LogInformation("Starting audit on subnet: {Subnet}", subnet);

            // --- RAPORT: Inicjalizacja (5%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 5,
                StatusMessage = "Initializing scan session...",
                DevicesDiscovered = 0
            });

            // 1. Initialize the session
            var session = new AuditSession
            {
                Subnet = subnet,
                Timestamp = DateTime.UtcNow,
                OverallRiskScore = "Low" // Default, will be escalated if needed
            };

            // --- RAPORT: Rozpoczęcie mapowania sieci (10%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 10,
                StatusMessage = $"Running ICMP discovery on {subnet}...",
                DevicesDiscovered = 0
            });

            // 2. Discover Hosts
            var activeDevices = await _discoveryStrategy.DiscoverActiveHostsAsync(subnet, token);
            var deviceList = activeDevices.ToList();
            session.TotalHostsFound = deviceList.Count;

            _logger.LogInformation("Discovered {Count} active devices.", session.TotalHostsFound);

            // --- RAPORT: Zakończenie mapowania, przejście do skanowania (20%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 20,
                StatusMessage = $"Discovered {session.TotalHostsFound} devices. Starting port scans...",
                DevicesDiscovered = session.TotalHostsFound
            });

            // 3. Process each device concurrently
            int completedDevices = 0;
            int totalDevices = deviceList.Count;

            // Tworzymy zadania z wbudowanym raportowaniem postępu po zakończeniu każdego z nich
            var deviceTasks = deviceList.Select(async device =>
            {
                // Uruchomienie skanowania dla konkretnego urządzenia
                var processedDevice = await ProcessDeviceAsync(device, targetPorts, token);

                // Zabezpieczenie przed błędami współbieżności - bezpieczna inkrementacja licznika
                int currentCompleted = Interlocked.Increment(ref completedDevices);

                // Skalowanie postępu w zakresie od 20% do 90%
                int progressPercent = totalDevices == 0 ? 90 : 20 + (int)((currentCompleted / (double)totalDevices) * 70);

                // --- RAPORT: Aktualizacja po każdym przeskanowanym urządzeniu ---
                progress?.Report(new ScanProgress
                {
                    ProgressPercentage = progressPercent,
                    StatusMessage = $"Scanned {currentCompleted} of {totalDevices} devices...",
                    DevicesDiscovered = totalDevices
                });

                return processedDevice;
            });

            var processedDevices = await Task.WhenAll(deviceTasks);

            // Attach processed devices to the session
            session.Devices = processedDevices.ToList();

            // --- RAPORT: Obliczanie ryzyka (90%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 90,
                StatusMessage = "Calculating overall network risk...",
                DevicesDiscovered = totalDevices
            });

            // 4. Calculate overall network risk based on findings
            session.OverallRiskScore = CalculateOverallRisk(session.Devices);

            // --- RAPORT: Zapis do bazy danych (95%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 95,
                StatusMessage = "Saving audit results to local database...",
                DevicesDiscovered = totalDevices
            });

            // 5. Persist the entire object graph to SQLite in a single transaction
            _logger.LogInformation("Saving audit session to database...");
            _dbContext.AuditSessions.Add(session);
            await _dbContext.SaveChangesAsync(token);

            _logger.LogInformation("Audit complete. Final Risk Score: {Score}", session.OverallRiskScore);

            // Sumowanie wykrytych podatności dla finalnego raportu (opcjonalne, jeśli ScanProgress to obsługuje)
            int totalVulnerabilities = session.Devices
                .SelectMany(d => d.ScannedPorts)
                .SelectMany(p => p.Vulnerabilities)
                .Count();

            // --- RAPORT: Zakończenie (100%) ---
            progress?.Report(new ScanProgress
            {
                ProgressPercentage = 100,
                StatusMessage = "Audit completed successfully.",
                DevicesDiscovered = totalDevices,
                VulnerabilitiesFound = totalVulnerabilities // Jeśli model ScanProgress zawiera tę właściwość
            });

            return session;
        }

        private async Task<Device> ProcessDeviceAsync(Device device, IEnumerable<int> targetPorts, CancellationToken token)
        {
            _logger.LogDebug("Scanning ports for device {Ip}...", device.IpAddress);

            // Port Scan
            var openPorts = await _portScanningStrategy.ScanPortsAsync(device, targetPorts, token);
            var portList = openPorts.ToList();

            // Vulnerability Scan (can also be done concurrently per port)
            var portTasks = portList.Select(async port =>
            {
                var vulns = await _vulnerabilityChecker.AnalyzePortAsync(port, device.IpAddress, token);
                port.Vulnerabilities = vulns.ToList();
                return port;
            });

            var processedPorts = await Task.WhenAll(portTasks);
            device.ScannedPorts = processedPorts.ToList();

            return device;
        }

        private string CalculateOverallRisk(IEnumerable<Device> devices)
        {
            bool hasMedium = false;

            foreach (var device in devices)
            {
                foreach (var port in device.ScannedPorts)
                {
                    foreach (var vuln in port.Vulnerabilities)
                    {
                        if (vuln.SeverityLevel.Equals("Critical", StringComparison.OrdinalIgnoreCase) ||
                            vuln.SeverityLevel.Equals("High", StringComparison.OrdinalIgnoreCase))
                        {
                            return "Critical"; // Fast exit, network is compromised
                        }

                        if (vuln.SeverityLevel.Equals("Medium", StringComparison.OrdinalIgnoreCase))
                        {
                            hasMedium = true;
                        }
                    }
                }
            }

            return hasMedium ? "Medium" : "Low";
        }
    }
}
