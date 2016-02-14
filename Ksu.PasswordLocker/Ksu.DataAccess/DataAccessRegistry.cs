﻿using Ksu.DataAccess.Dal;
using StructureMap;

namespace Ksu.DataAccess
{
    public class DataAccessRegistry : Registry
    {
        public DataAccessRegistry()
        {
            For<IContextProvider>().Use<ContextProvider>();
            For<ICompanyDal>().Use<CompanyDal>();
            For<IUserDal>().Use<UserDal>();
            For<IUserStore>().Use<UserStore>();
        }
    }
}
