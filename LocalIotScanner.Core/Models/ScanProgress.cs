using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Models
{
    public class ScanProgress
    {
        public int ProgressPercentage { get; set; }
        public string StatusMessage { get; set; } = "Idle";
        public int DevicesDiscovered { get; set; }
        public int VulnerabilitiesFound { get; set; }
    }
}
