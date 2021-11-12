using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class CurrentUser
    {
        public readonly string id;
        public readonly string login;
        public readonly string roles;

        public CurrentUser(string id, string login, string roles)
        {
            this.id = id;
            this.login = login;
            this.roles = roles;
        }
    }
}
