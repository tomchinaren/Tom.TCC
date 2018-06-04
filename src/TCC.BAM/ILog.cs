using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    public interface ILog
    {
        void Info(BusinessActivityStatus lastStatus, BusinessActivityStatus curStatus);
        void Error(BusinessActivityStatus lastStatus, BusinessActivityStatus curStatus);
    }
}
