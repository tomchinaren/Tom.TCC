using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    public class BusinessActivityImpl : IBusinessActivity
    {
        //properties
        private long _businessActivityID;
        private BusinessActivityStatus _status;
        private List<IAtomicAction> _actionList;
        private Dictionary<BusinessActivityStatus, BusinessActivityStatus> _statusCanChangedDict;   //key for current status, value for new status

        public BusinessActivityImpl()
        {
            _status = BusinessActivityStatus.Init;
            _actionList = new List<IAtomicAction>();

            _statusCanChangedDict = new Dictionary<BusinessActivityStatus, BusinessActivityStatus>() {
                //start stop
                { BusinessActivityStatus.Init,BusinessActivityStatus.Started },
                { BusinessActivityStatus.Init,BusinessActivityStatus.Stopped },
                { BusinessActivityStatus.Started,BusinessActivityStatus.Stopped },
                //try
                { BusinessActivityStatus.Started,BusinessActivityStatus.Trying },
                { BusinessActivityStatus.Trying,BusinessActivityStatus.Tryed },
                { BusinessActivityStatus.Trying,BusinessActivityStatus.TryFailed },
                //commit
                { BusinessActivityStatus.Tryed,BusinessActivityStatus.Commiting },
                { BusinessActivityStatus.Tryed,BusinessActivityStatus.Canceling },
                { BusinessActivityStatus.Commiting,BusinessActivityStatus.Commited },
                { BusinessActivityStatus.Commiting,BusinessActivityStatus.CommitFailed },
                //cancel
                { BusinessActivityStatus.TryFailed,BusinessActivityStatus.Canceling },
                { BusinessActivityStatus.CommitFailed,BusinessActivityStatus.Canceling },
                { BusinessActivityStatus.Canceling,BusinessActivityStatus.CancelFailed },

            };
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
            _status = BusinessActivityStatus.Started;
        }

        public bool Try()
        {
            var flag = false;
            foreach(var action in _actionList)
            {
                flag = action.Try();
                if(!flag)
                {
                    break;
                }
            }
            return flag;
        }

        public bool Commit()
        {
            var flag = false;
            foreach (var action in _actionList)
            {
                flag = action.Commit();
                if (!flag)
                {
                    break;
                }
            }
            if (flag)
            {
                _status = BusinessActivityStatus.Commited;
            }
            else
            {
                _status = BusinessActivityStatus.CommitFailed;
            }
            return flag;
        }

        public bool Cancel()
        {
            var flag = false;
            foreach (var action in _actionList)
            {
                flag = action.Cancel();
                if (!flag)
                {
                    break;
                }
            }
            if (flag)
            {
                _status = BusinessActivityStatus.Canceled;
            }
            else
            {
                _status = BusinessActivityStatus.CancelFailed;
            }
            return flag;
        }


        private long GetLongID()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        public void ChangeStatus(BusinessActivityStatus newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
