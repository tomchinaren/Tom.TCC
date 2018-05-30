using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    interface IBusinessActivityManager
    {
        void commit();
        void rollback();
        void enlistAction();
        void delistAction();
    }
}
