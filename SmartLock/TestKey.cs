using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
{/// <summary>
/// Класс тестового ключа
/// </summary>
    public class TestKey:Key
    {
        TestKey() : base()
        {
        }
        public TestKey(DateTime d, bool[,] m) : base(d, m)
        {
        }
        public bool CheckTestKey(bool[,] test, bool[,] fix)
        {
            int count = 0;
            for (int i = 0; i < test.GetLength(0); i++)
                for (int j = 0; j < test.GetLength(1); j++)
                {
                    if (test[i, j] == (fix[i, j]))
                        count++;
                }
            if (count >= 80) return true;       //задаётся процент схожести ключей 80 элементов из 100
            else return false;

        }
    }
}
