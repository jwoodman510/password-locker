using Caching;
using DataAccess;
using DataAccess.Dal;
using StructureMap;
using StructureMap.Pipeline;

namespace PasswordLocker.Bootstrap
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