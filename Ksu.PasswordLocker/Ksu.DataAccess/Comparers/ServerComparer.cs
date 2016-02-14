using System.Collections.Generic;

namespace Ksu.DataAccess.Comparers
{
    public class ServerComparer : IEqualityComparer<Server>
    {
        private readonly CompareSetting _setting;

        public ServerComparer(CompareSetting setting)
        {
            _setting = setting;
        }

        public bool Equals(Server first, Server second)
        {
            if (_setting == CompareSetting.CompareProperties)
            {
                return first.ServerId == second.ServerId
                       && first.ServerName == second.ServerName;
            }

            if (_setting == CompareSetting.CompareObjects)
            {
                return first.Equals(second);
            }

            return first.ServerId == second.ServerId;
        }

        public int GetHashCode(Server u)
        {
            return _setting == CompareSetting.CompareIds
                ? u.ServerId.GetHashCode()
                : u.GetHashCode();
        }
    }
}
