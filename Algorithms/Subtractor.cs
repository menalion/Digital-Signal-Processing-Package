using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Subtractor : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputSignal { get; set; }

        /// <summary>
        /// To do: Subtract Signal2 from Signal1 
        /// i.e OutSig = Sig1 - Sig2 
        /// </summary>
        public override void Run()
        {
            OutputSignal = new Signal(new List<float>(), new bool());
            // get the maximum number of samples in any one of the input signals  
            int number_of_samples;
            if (InputSignal1.SamplesIndices.Count >= InputSignal2.SamplesIndices.Count)
                number_of_samples = InputSignal1.SamplesIndices.Count;
            else
                number_of_samples = InputSignal2.SamplesIndices.Count;

            // multiply the second signal with -1 then ad it 
            for (int index_sample = 0; index_sample < number_of_samples; ++index_sample)
            {
                float sample1, sample2;
                // check for sample of signal 1 
                if (index_sample < InputSignal1.Samples.Count) sample1 = InputSignal1.Samples[index_sample];
                else sample1 = 0;

                // check for sample of signal 2
                if (index_sample < InputSignal2.Samples.Count) sample2 = InputSignal2.Samples[index_sample];
                else sample2 = 0;
                OutputSignal.Samples.Add(sample1 + (sample2 * -1));
            }
            //throw new NotImplementedException();
        }
    }
}