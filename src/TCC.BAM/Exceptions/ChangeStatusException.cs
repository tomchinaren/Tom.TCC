using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.BAM.Exceptions
{
    public class ChangeStatusException: Exception
    {
        public ChangeStatusException(string message):base(message) {
        }
    }
}
