using System.Collections.Generic;

namespace MiniIndex.Models
{
    public interface IDeleteOrphaned
    {
        IEnumerable<object> GetChildren();
    }
}