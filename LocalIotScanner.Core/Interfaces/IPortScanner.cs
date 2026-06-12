using LocalIotScanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Interfaces
{
    public interface IPortScanner
    {
        /// <summary>
        /// Checks a predefined list of ports for a given active host.
        /// </summary>
        Task<IEnumerable<ScannedPort>> ScanPortsAsync(Device device, IEnumerable<int> targetPorts, CancellationToken token);
    }
}
