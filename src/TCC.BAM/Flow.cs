using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    class Flow : IFlow<Tuple<BusinessActivityStatus, BusinessActivityStatus>>
    {
        private List<Tuple<BusinessActivityStatus, BusinessActivityStatus>> _flow;   //key for current status, value for new status

        public Flow()
        {
            InitFlows();
        }

        public bool Exists(Predicate<Tuple<BusinessActivityStatus, BusinessActivityStatus>> match)
        {
            return _flow.Exists(match);
        }

        private void InitFlows()
        {
            _flow = new List<Tuple<BusinessActivityStatus, BusinessActivityStatus>>();
            //start stop
            AddToFlows(_flow, BusinessActivityStatus.Init, BusinessActivityStatus.Started);
            AddToFlows(_flow, BusinessActivityStatus.Init, BusinessActivityStatus.Stopped);
            AddToFlows(_flow, BusinessActivityStatus.Started, BusinessActivityStatus.Stopped);
            //try
            AddToFlows(_flow, BusinessActivityStatus.Started, BusinessActivityStatus.Trying);
            AddToFlows(_flow, BusinessActivityStatus.Trying, BusinessActivityStatus.Tryed);
            AddToFlows(_flow, BusinessActivityStatus.Trying, BusinessActivityStatus.TryFailed);
            //commit
            AddToFlows(_flow, BusinessActivityStatus.Tryed, BusinessActivityStatus.Commiting);
            AddToFlows(_flow, BusinessActivityStatus.Tryed, BusinessActivityStatus.Canceling);
            AddToFlows(_flow, BusinessActivityStatus.Commiting, BusinessActivityStatus.Commited);
            AddToFlows(_flow, BusinessActivityStatus.Commiting, BusinessActivityStatus.CommitFailed);
            //cancel
            AddToFlows(_flow, BusinessActivityStatus.TryFailed, BusinessActivityStatus.Canceling);
            AddToFlows(_flow, BusinessActivityStatus.CommitFailed, BusinessActivityStatus.Canceling);
            AddToFlows(_flow, BusinessActivityStatus.Canceling, BusinessActivityStatus.Canceled);
            AddToFlows(_flow, BusinessActivityStatus.Canceling, BusinessActivityStatus.CancelFailed);
        }

        private void AddToFlows(List<Tuple<BusinessActivityStatus, BusinessActivityStatus>> flows, BusinessActivityStatus curStatus, BusinessActivityStatus newStatus)
        {
            var flowItem = new Tuple<BusinessActivityStatus, BusinessActivityStatus>(curStatus, newStatus);
            flows.Add(flowItem);
        }


    }
}
