using LocalIotScanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalIotScanner.Core.Interfaces
{
    public interface IHostDiscovery
    {
        /// <summary>
        /// Performs an ICMP sweep on the target subnet.
        /// </summary>
        Task<IEnumerable<Device>> DiscoverActiveHostsAsync(string subnet, CancellationToken token);
    }
}
