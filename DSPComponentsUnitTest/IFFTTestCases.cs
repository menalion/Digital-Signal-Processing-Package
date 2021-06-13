using DSPAlgorithms.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPComponentsUnitTest
{
    class IFFTTestCases
    {
        [TestMethod]
        public void IFFTTestMethod1()
        {
            InverseFastFourierTransform IFFT = new InverseFastFourierTransform();
            // test case 2

            var FrequenciesAmplitudes = new List<float> { 64, 20.9050074380220f, 11.3137084989848f, 8.65913760233915f, 8, 8.65913760233915f, 11.3137084989848f, 20.9050074380220f };
            var FrequenciesPhaseShifts = new List<float> { 0, 1.96349540849362f, 2.35619449019235f, 2.74889357189107f, -3.14159265358979f, -2.74889357189107f, -2.35619449019235f, -1.96349540849362f };
            var Frequencies = new List<float> { 0, 1, 2, 3, 4, 5, 6, 7 };

            IFFT.InputFreqDomainSignal = new DSPAlgorithms.DataStructures.Signal(true, Frequencies, FrequenciesAmplitudes, FrequenciesPhaseShifts);

            IFFT.Run();

            List<float> outPutSamples = new List<float> { 1, 3, 5, 7, 9, 11, 13, 15 };
            Assert.IsTrue(UnitTestUtitlities.SignalsSamplesAreEqual(outPutSamples, IFFT.OutputTimeDomainSignal.Samples));

        }
    }
}
