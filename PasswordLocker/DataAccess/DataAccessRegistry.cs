using DataAccess.Dal;
using Microsoft.Practices.Unity;

namespace DataAccess
{
    public static class DataAccessRegistry
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<ICompanyDal, CompanyDal>();
        }
    }
}
