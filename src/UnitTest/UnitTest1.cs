using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TCC.BAM;
using TCC.BAM.Exceptions;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        #region init
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

        class Action : IAtomicAction
        {
            public enum AcitonStep
            {
                Try,
                Commit,
                Cancel
            }


            public Dictionary<AcitonStep, int> TypeDict { get; set; } //value:0| auto,1|true,2|false
            public Action(string name)
            {
                Name = name;

                TypeDict = new Dictionary<AcitonStep, int>() {
                    { AcitonStep.Try, 0},
                    { AcitonStep.Commit, 0},
                    { AcitonStep.Cancel, 0},
                };
            }

            public string Name { get; set; }

            public bool Cancel()
            {
                return GetRand(AcitonStep.Cancel);
            }

            public bool Commit()
            {
                return GetRand(AcitonStep.Commit);
            }

            public bool Try()
            {
                return GetRand(AcitonStep.Try);
            }

            private bool GetRand(AcitonStep acitonStep)
            {
                if (TypeDict[acitonStep] == 1)
                {
                    Console.WriteLine("{0} {1} {2}", acitonStep, Name, TypeDict[acitonStep]);
                    return true;
                }
                if (TypeDict[acitonStep] == 2)
                {
                    Console.WriteLine("{0} {1} {2}", acitonStep, Name, TypeDict[acitonStep]);
                    return false;
                }

                var ticks = (int)DateTime.Now.Ticks;
                var rand = new Random(ticks);
                var num = rand.Next(0, 100);
                Console.WriteLine("ticks {0} ,num {1}", ticks, num);
                var flag = num > 50;
                Console.WriteLine("{0} {1} {2}", acitonStep, Name, flag);
                return flag;
            }
        }
        private IBusinessActivity GetAct(params int[] types)
        {
            var log = new Log();
            var acty = new BusinessActivityImpl(log);
            Action A = new UnitTest.UnitTest1.Action("A");
            Action B = new UnitTest.UnitTest1.Action("B");
            acty.EnlistAction(A);
            acty.EnlistAction(B);

            if (types.Length > 0)
            {
                var i = 0;
                A.TypeDict[Action.AcitonStep.Try] = types[i++];
                A.TypeDict[Action.AcitonStep.Commit] = types[i++];
                A.TypeDict[Action.AcitonStep.Cancel] = types[i++];

                B.TypeDict[Action.AcitonStep.Try] = types[i++];
                B.TypeDict[Action.AcitonStep.Commit] = types[i++];
                B.TypeDict[Action.AcitonStep.Cancel] = types[i++];
            }

            return acty;
        }
        #endregion

        [TestMethod]
        public void TestTryFail()
        {
            var acty = GetAct(2, 1, 1, 1, 1, 1);
            acty.Start();
            var isOk = acty.Try();
            Assert.IsFalse(isOk);
        }

        [TestMethod]
        public void TestCommitOk()
        {
            var acty = GetAct(1,1,1,1,1,1);
            acty.Start();
            var isOk = acty.Try();
            isOk = acty.Commit();
            Assert.IsTrue(isOk);
        }

        [TestMethod]
        public void TestCommitFail()
        {
            var acty = GetAct(1, 2, 1, 1, 1, 1);
            acty.Start();
            var isOk = acty.Try();
            isOk = acty.Commit();
            Assert.IsFalse(isOk);
        }

        [TestMethod]
        public void TestCancelOk()
        {
            var acty = GetAct(1, 1, 1, 1, 1, 1);
            acty.Start();
            var isOk = acty.Try();
            isOk = acty.Cancel();
            Assert.IsTrue(isOk);
        }

        [TestMethod]
        public void TestCancelFail()
        {
            var acty = GetAct(1, 1, 1, 1, 1, 2);
            acty.Start();
            var isOk = acty.Try();
            isOk = acty.Cancel();
            Assert.IsFalse(isOk);
        }

        [TestMethod]
        public void TestDelistAction()
        {
            var acty = GetAct(1, 1, 1, 1, 1, 1);
            Action C = new UnitTest.UnitTest1.Action("C");
            acty.EnlistAction(C);
            acty.DelistAction(C);
            acty.Start();
            var isOk = acty.Try();
            isOk = acty.Commit();
            Assert.IsTrue(isOk);
        }

        [TestMethod]
        public void TestChangeStatusException()
        {
            try
            {
                var acty = GetAct();
                acty.Start();
                acty.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.IsTrue(ex is ChangeStatusException);
            }
        }

        [TestMethod]
        public void TestAuto()
        {
            var acty = GetAct();
            acty.Start();
            var isOk = acty.Try();
            Console.WriteLine(isOk);
        }


    }



}
