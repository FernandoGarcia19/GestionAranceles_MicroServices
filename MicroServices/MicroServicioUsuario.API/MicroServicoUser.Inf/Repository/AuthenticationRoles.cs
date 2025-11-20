using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicoUser.Inf.Repository
{
    public static class AuthenticationRoles
    {
        public static readonly Dictionary<string, int> RoleToCode = new()
        {
            { "Admin", 1 },
            { "Contador", 2 }
        };

        public static readonly Dictionary<int, string> CodeToRole = RoleToCode.ToDictionary(kv => kv.Value, kv => kv.Key);
    }
}
