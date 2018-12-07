using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
{
    internal abstract class User
    {
        string login;
        string password;

        public User()
        {
            login = null;
            password = null;
        }
        public User(string l, string p)
        {
            login = l;
            password = p;
        }

    }
}
