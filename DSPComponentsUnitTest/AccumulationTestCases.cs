using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DSPAlgorithms.DataStructures;
using DSPAlgorithms.Algorithms;

namespace DSPComponentsUnitTest
{
    [TestClass]
    public class AccumulationTestCases
    {
        //Add 2 signals -- 1,2
        [TestMethod]
        public void AccumulationTestMethod1()
        {
            // test case 1 ..
            Accumulation a = new Accumulation();
            a.InputSignals = new Signal(new List<float>() { 1, 2, 3, 4, 5 }, false);
            Signal expectedOutput = new Signal(new List<float>() { 1, 3, 6, 10, 15 }, false);
            a.Run();
            Assert.IsTrue(UnitTestUtitlities.SignalsSamplesAreEqual(expectedOutput.Samples, a.OutputSignal.Samples));
        }
    }
}