using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Models
{
    public class Device
    {
        public int Id { get; set; }
        public int AuditSessionId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "Unknown";

        public AuditSession? AuditSession { get; set; }
        public ICollection<ScannedPort> ScannedPorts { get; set; } = new List<ScannedPort>();
    }
}
