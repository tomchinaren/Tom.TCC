using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum BusinessActivityStatus
    {
        /// <summary>
        /// 初始化未开始|0
        /// </summary>
        Init = 0,

        /// <summary>
        /// 已开始|1
        /// </summary>
        Started = 1,
        /// <summary>
        /// 已终止| -1
        /// </summary>
        Stopped = -1,

        /// <summary>
        /// 开始尝试|100
        /// </summary>
        Trying = 100,
        /// <summary>
        /// 尝试完成|101
        /// </summary>
        Tryed = 101,
        /// <summary>
        /// 尝试失败|102
        /// </summary>
        TryFailed = 102,

        /// <summary>
        /// 开始提交|200
        /// </summary>
        Commiting = 200,
        /// <summary>
        /// 已提交|201
        /// </summary>
        Commited = 201,
        /// <summary>
        /// 提交失败|202
        /// </summary>
        CommitFailed = 202,

        /// <summary>
        /// 开始取消|500
        /// </summary>
        Canceling = 500,
        /// <summary>
        /// 已取消|501
        /// </summary>
        Canceled = 501,
        /// <summary>
        /// 取消失败|502
        /// </summary>
        CancelFailed = 502,
    }
}
