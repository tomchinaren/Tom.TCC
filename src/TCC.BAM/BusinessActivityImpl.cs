using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.BAM.Exceptions;

namespace TCC.BAM
{
    public class BusinessActivityImpl : IBusinessActivity
    {
        //properties
        private long _businessActivityID;
        private BusinessActivityStatus _lastStatus;
        private BusinessActivityStatus _status;
        private List<IAtomicAction> _actionList;
        private List<Tuple<BusinessActivityStatus, BusinessActivityStatus>> _statusFlows;   //key for current status, value for new status
        private ILog _log;

        public BusinessActivityImpl(ILog log = null)
        {
            InitFlows();
            _status = BusinessActivityStatus.Init;
            _lastStatus = _status;
            _actionList = new List<IAtomicAction>();
            _log = log;
        }

        private void InitFlows()
        {
            _statusFlows = new List<Tuple<BusinessActivityStatus, BusinessActivityStatus>>();
            //start stop
            AddToFlows(_statusFlows, BusinessActivityStatus.Init, BusinessActivityStatus.Started);
            AddToFlows(_statusFlows, BusinessActivityStatus.Init, BusinessActivityStatus.Stopped);
            AddToFlows(_statusFlows, BusinessActivityStatus.Started, BusinessActivityStatus.Stopped);
            //try
            AddToFlows(_statusFlows, BusinessActivityStatus.Started, BusinessActivityStatus.Trying);
            AddToFlows(_statusFlows, BusinessActivityStatus.Trying, BusinessActivityStatus.Tryed);
            AddToFlows(_statusFlows, BusinessActivityStatus.Trying, BusinessActivityStatus.TryFailed);
            //commit
            AddToFlows(_statusFlows, BusinessActivityStatus.Tryed, BusinessActivityStatus.Commiting);
            AddToFlows(_statusFlows, BusinessActivityStatus.Tryed, BusinessActivityStatus.Canceling);
            AddToFlows(_statusFlows, BusinessActivityStatus.Commiting, BusinessActivityStatus.Commited);
            AddToFlows(_statusFlows, BusinessActivityStatus.Commiting, BusinessActivityStatus.CommitFailed);
            //cancel
            AddToFlows(_statusFlows, BusinessActivityStatus.TryFailed, BusinessActivityStatus.Canceling);
            AddToFlows(_statusFlows, BusinessActivityStatus.CommitFailed, BusinessActivityStatus.Canceling);
            AddToFlows(_statusFlows, BusinessActivityStatus.Canceling, BusinessActivityStatus.Canceled);
            AddToFlows(_statusFlows, BusinessActivityStatus.Canceling, BusinessActivityStatus.CancelFailed);
        }
        private void AddToFlows(List<Tuple<BusinessActivityStatus, BusinessActivityStatus>> flows, BusinessActivityStatus curStatus, BusinessActivityStatus newStatus)
        {
            var flowItem = new Tuple<BusinessActivityStatus, BusinessActivityStatus>(curStatus, newStatus);
            flows.Add(flowItem);
        }

        public void EnlistAction(IAtomicAction action)
        {
            _actionList.Add(action);
        }

        public void DelistAction(IAtomicAction action)
        {
            _actionList.Remove(action);
        }

        public void Start()
        {
            var id = GetLongID();
            Start(id);
        }
        public void Start(long businessActivityID)
        {
            _businessActivityID = businessActivityID;
            ChangeStatus(BusinessActivityStatus.Started);
        }

        public bool Try()
        {
            ChangeStatus(BusinessActivityStatus.Trying);
            var flag = false;
            foreach (var action in _actionList)
            {
                flag = action.Try();
                if(!flag)
                {
                    break;
                }
            }

            var newStatus = flag ? BusinessActivityStatus.Tryed : BusinessActivityStatus.TryFailed;
            ChangeStatus(newStatus);
            return flag;
        }

        public bool Commit()
        {
            ChangeStatus(BusinessActivityStatus.Commiting);
            var flag = false;
            foreach (var action in _actionList)
            {
                flag = action.Commit();
                if (!flag)
                {
                    break;
                }
            }

            var newStatus = flag ? BusinessActivityStatus.Commited : BusinessActivityStatus.CommitFailed;
            ChangeStatus(newStatus);
            return flag;
        }

        public bool Cancel()
        {
            ChangeStatus(BusinessActivityStatus.Canceling);
            var flag = false;
            foreach (var action in _actionList)
            {
                flag = action.Cancel();
                if (!flag)
                {
                    break;
                }
            }
            var newStatus = flag? BusinessActivityStatus.Canceled: BusinessActivityStatus.CancelFailed;
            ChangeStatus(newStatus);
            return flag;
        }


        private long GetLongID()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        public void ChangeStatus(BusinessActivityStatus newStatus)
        {
            bool flag = _statusFlows.Exists(t => t.Item1 == _status && t.Item2 == newStatus);
            if (!flag)
            {
                throw new ChangeStatusException(string.Format("ChangeStatus error, can't change status from {0} to {1}", _status, newStatus));
            }

            _lastStatus = _status;
            _status = newStatus;

            if (_log != null)
            {
                _log.Info(_lastStatus, _status);
            }
        }
    }
}
