
using System.Collections.Generic;

namespace Ksu.DataAccess.Comparers
{
    public class ServerLoginComparer : IEqualityComparer<ServerLogin>
    {
        private readonly CompareSetting _setting;

        public ServerLoginComparer(CompareSetting setting)
        {
            _setting = setting;
        }

        public bool Equals(ServerLogin first, ServerLogin second)
        {
            if (_setting == CompareSetting.CompareProperties)
            {
                return first.ServerLoginId == second.ServerLoginId
                       && first.ServerId == second.ServerId
                       && first.DepartmentId == second.DepartmentId
                       && first.UserName == second.UserName
                       && first.PasswordHash == second.PasswordHash;
            }

            if (_setting == CompareSetting.CompareObjects)
            {
                return first.Equals(second);
            }

            return first.ServerLoginId == second.ServerLoginId;
        }

        public int GetHashCode(ServerLogin u)
        {
            return _setting == CompareSetting.CompareIds
                ? u.ServerLoginId.GetHashCode()
                : u.GetHashCode();
        }
    }
}
