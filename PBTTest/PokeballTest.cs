using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightApplication3;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace PBTTest
{
    [TestClass]
    public class PokeballTest
    {
        [TestMethod]
        public void MasterballTest()
        {
            ItemStore it = new ItemStore();

            Pokeball mBall = (Pokeball)(it.get(ItemName.MasterBall));

            Pokemon pMon = new Pokemon(-80, "", 100, Pokemon.PokeType.Bug, Pokemon.PokeType.Bug, null, 106, 106,  110, 90, 154, 90, 130, 3);
            bool correct = true;

            for (int i = 0; i < 100000; ++i)
            { 
               if (!pMon.ThrowBall(mBall))
                {
                    Trace.WriteLine(pMon.captureRate);
                    correct = false;
                    break;
                }
            }

            Assert.AreEqual(correct, true);

        }
    }
}
