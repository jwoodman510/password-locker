using Ksu.Caching;
using Ksu.DataAccess;
using Ksu.DataAccess.Dal;
using StructureMap;
using StructureMap.Pipeline;

namespace Ksu.PasswordLocker.Bootstrap
{
    public class Configuration : Registry
    {
        public Configuration()
        {
            For<IContextProvider>().Use<ContextProvider>();
            For<ICompanyDal>().Use<CompanyDal>();
            For<ICache>().Use<Cache>().SetLifecycleTo<SingletonLifecycle>();
        }
    }
}