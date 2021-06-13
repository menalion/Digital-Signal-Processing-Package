using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Adder : Algorithm
    {
        public List<Signal> InputSignals { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            OutputSignal = new Signal(new List<float>(), new bool());
            // get the maximum number of samples in any one of the input signals  
            int number_of_samples = 0;
            for (int i = 0; i < InputSignals.Count; ++i)
            {
                if (InputSignals[i].SamplesIndices.Count > number_of_samples)
                    number_of_samples = InputSignals[i].SamplesIndices.Count; 
            }
            // Now Loop on the samples and sum them in temp_sum_sample 
            // Then add the variable temp to the Outputsignal Sample 
            for(int index_sample = 0; index_sample< number_of_samples; ++index_sample)
            {
                float temp_sample_sum = 0;
                for (int i = 0; i < InputSignals.Count; ++i)
                {
                    if (index_sample < InputSignals[i].SamplesIndices.Count)
                        temp_sample_sum += InputSignals[i].Samples[index_sample]; 
                }
                OutputSignal.Samples.Add(temp_sample_sum);
            }
            //throw new NotImplementedException();
        }
    }
}