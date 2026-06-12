using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Models
{
    public class AuditSession
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Subnet { get; set; } = string.Empty;
        public int TotalHostsFound { get; set; }
        public string OverallRiskScore { get; set; } = "Unknown";

        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
