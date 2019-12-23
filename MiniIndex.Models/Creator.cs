using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Models
{
    public class Creator : IEntity
    {
        public Creator()
        {
            Sites = Enumerable.Empty<SourceSite>().ToList();
        }

        public int ID { get; set; }
        public string Name { get; set; }

        public ICollection<SourceSite> Sites { get; set; }

        public string PatreonURL { get; set; }
        public string ThingiverseURL { get; set; }
        public string WebsiteURL { get; set; }
        public string ShapewaysURL { get; set; }
    }
}