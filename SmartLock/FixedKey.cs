using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
{/// <summary>
/// Класс эталонного ключа
/// </summary>
    internal class FixedKey:Key
    {
        FixedKey() : base()
        {
        }
        FixedKey(DateTime d, object p, string v) : base(d, p, v)
        {
        }

    }
}
