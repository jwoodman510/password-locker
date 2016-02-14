using Ksu.Caching;
using StructureMap;
using StructureMap.Pipeline;

namespace Ksu.PasswordLocker.Bootstrap
{
    public class Configuration : Registry
    {
        public Configuration()
        {
            For<ICache>().Use<Cache>().SetLifecycleTo<SingletonLifecycle>();
        }
    }
}