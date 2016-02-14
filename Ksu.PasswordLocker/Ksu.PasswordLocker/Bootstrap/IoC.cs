using Ksu.DataAccess;
using StructureMap;

namespace Ksu.PasswordLocker.Bootstrap
{
    public static class IoC
    {
        private static readonly IContainer Container;

        static IoC()
        {
            Container = new Container(new Configuration());
            Container.Configure(c => c.AddRegistry(new DataAccessRegistry()));
        }

        public static T Resolve<T>()
        {
            return Container.GetInstance<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Container.GetInstance<T>(name);
        }

        public static void BuildUp(object target)
        {
            Container.BuildUp(target);
        }
    }
}