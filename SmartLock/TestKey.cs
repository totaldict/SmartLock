using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
{/// <summary>
/// Класс тестового ключа
/// </summary>
    class TestKey:Key
    {
        TestKey() : base()
        {
        }
        TestKey(DateTime d, object p, string v) : base(d, p, v)
        {
        }

    }
}
