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
            Test();
            Console.ReadLine();
        }

        static void Test()
        {
            try
            {
                var log = new Log();
                IBusinessActivity acty = new BusinessActivityImpl(log);
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class Log : ILog
    {
        public void Info(BusinessActivityStatus lastStatus, BusinessActivityStatus curStatus)
        {
            Console.WriteLine("Info lastStatus:{0}, curStatus: {1}", lastStatus, curStatus);
        }

        public void Error(BusinessActivityStatus lastStatus, BusinessActivityStatus curStatus)
        {
            Console.WriteLine("Error lastStatus:{0}, curStatus: {1}", lastStatus, curStatus);
        }
    }
}
