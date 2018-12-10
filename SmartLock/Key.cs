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
        //object pic;
        public bool [,] matrix;

        internal Key()
        {
            date = new DateTime();
            //pic = null;
            matrix = null;
        }
        internal Key(DateTime d, bool[,] m)
        {
            date = d;
            matrix = m;
        }

    }
}
