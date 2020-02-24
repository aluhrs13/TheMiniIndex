using Lamar;
using MiniIndex.Core.Mapping;

namespace MiniIndex
{
    public class WebAppServices : ServiceRegistry
    {
        public WebAppServices()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();

                scan.AddAllTypesOf<IMapperConfiguration>();
            });
        }
    }
}