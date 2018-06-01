using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    public interface IAtomicAction
    {
        bool Try();
        bool Commit();
        bool Cancel();
    }
}
