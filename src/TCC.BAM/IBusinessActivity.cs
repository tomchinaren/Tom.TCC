using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    public interface IBusinessActivity
    {
        void Start();
        void Start(long businessActivityId);
        bool Try();
        bool Commit();
        bool Cancel();
        void EnlistAction(IAtomicAction action);
        void DelistAction(IAtomicAction action);
        void ChangeStatus(BusinessActivityStatus newStatus);
    }
}
