using MiniIndex.Core.Minis;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Core
{
    public class CoreServiceInfo
    {
        public CoreServiceInfo(IEnumerable<IParser> parsers)
        {
            SupportedSites = parsers.Select(p => p.Site).ToList();
        }

        public IEnumerable<string> SupportedSites { get; }
    }
}