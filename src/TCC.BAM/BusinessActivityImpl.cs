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
        //static properties
        private static Flow _flow;
        //properties
        private long _businessActivityID;
        private BusinessActivityStatus _lastStatus;
        private BusinessActivityStatus _status;
        private List<IAtomicAction> _actionList;
        private ILog _log;

        static BusinessActivityImpl()
        {
            _flow = new BAM.Flow();
        }

        public BusinessActivityImpl(ILog log = null)
        {
            _status = BusinessActivityStatus.Init;
            _lastStatus = _status;
            _actionList = new List<IAtomicAction>();
            _log = log;
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
            bool flag = _flow.Exists(t => t.Item1 == _status && t.Item2 == newStatus);
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
