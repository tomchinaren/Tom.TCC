using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.BAM;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IBusinessActivity acty = new BusinessActivityImpl();
            acty.Start();
            var isOk = acty.Try();
            if (isOk)
            {
                acty.Commit();
            }
            else
            {
                acty.Cancel();
            }
            Console.ReadLine();
        }
    }
}
