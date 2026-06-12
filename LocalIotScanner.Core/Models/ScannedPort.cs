using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Models
{
    public class ScannedPort
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int PortNumber { get; set; }
        public string Protocol { get; set; } = "TCP"; // TCP or UDP
        public string State { get; set; } = "Closed"; // Open, Closed, Filtered
        public string DetectedService { get; set; } = "Unknown";

        public Device? Device { get; set; }
        public ICollection<Vulnerability> Vulnerabilities { get; set; } = new List<Vulnerability>();
    }
}
