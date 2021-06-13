using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class RemoveDCComponent : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }

        // this to remove dc in the normal way 
        public void remove_dc_first_method()
        {
            // Make New Refrence 
            OutputSignal = new Signal(new List<float>(), new bool());
            // first get the mean of the Input Signal samples 
            float mean_a = InputSignal.Samples.Sum() / InputSignal.Samples.Count;

            // As To Remove the DC Component We Must Ensure That The Mean of All Values is Zero 
            // So  We Will Take Each Value and Subtract It From The Mean 
            for (int i = 0; i < InputSignal.Samples.Count; ++i)
                // Round To Three Numbers After The Decimal Point(Like The Output)
                OutputSignal.Samples.Add((float)Math.Round(InputSignal.Samples[i] - mean_a, 1));

        }

        public void remove_dc_second_method()
        {
            // make object form the fast foriour transform 
            FastFourierTransform FFT = new FastFourierTransform();

            // no convert the singal form time domain to the frequency domain 
            FFT.InputTimeDomainSignal = InputSignal;
            FFT.Run();

            // now we will remove the first component form the (freqeuncy list, amplitude list and phase shift list)  at 0 index
            FFT.OutputFreqDomainSignal.Frequencies[0] = 0;
            FFT.OutputFreqDomainSignal.FrequenciesAmplitudes[0] = 0;
            FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts[0] = 0 ;

            // make object form inverse fast foriour transform 
            InverseFastFourierTransform IFFT = new InverseFastFourierTransform(); 
            // the input is from the FFT (the out put )
            IFFT.InputFreqDomainSignal = FFT.OutputFreqDomainSignal;
            IFFT.Run();
            // Convert Back the singal to the time domian
            OutputSignal = IFFT.OutputTimeDomainSignal; 
        }
        public override void Run()
        {
            // for the first method with out FFT and IFFT 
            remove_dc_first_method();

            // for the second method wiht FFT and IFFT 
            remove_dc_second_method();
        }
    }
}
