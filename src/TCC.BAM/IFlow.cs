using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    interface IFlow<T>
    {
        bool Exists(Predicate<T> match);
    }
}
