using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock
    
{   [Serializable]
    /// <summary>
    /// Класс эталонного ключа
    /// </summary>
    public class FixedKey:Key
    {
        FixedKey() : base()
        {
        }
        public FixedKey(DateTime d, bool[,] m) : base(d, m)
        {
            
        }
        
    }
}
