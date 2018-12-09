using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
{
    public abstract class Key
    {
        DateTime date;
        object pic;
        string vector;

        internal Key()
        {
            date = new DateTime();
            pic = null;
            vector = null;
        }
        internal Key(DateTime d, object p, string v)
        {
            date = d;
            pic = p;
            vector = v;
        }

    }
}
