using LocalIotScanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Interfaces
{
    public interface IAuditOrchestratorService
    {
        /// <summary>
        /// Executes a full network audit: Discovery -> Port Scan -> Vulnerability Check -> Save to DB.
        /// </summary>
        Task<AuditSession> RunFullAuditAsync(string subnet, IEnumerable<int> targetPorts, CancellationToken token = default, IProgress<ScanProgress> progress = null);
    }
}
