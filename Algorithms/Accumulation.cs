using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Accumulation : Algorithm
    {
        public Signal InputSignals;
        public Signal OutputSignal;

        public override void Run()
        {
            // initalize the output singal 
            OutputSignal = new Signal(new List<float>(), new bool());
            // the accmulator is the summition of all previouse samples 
            float accumulator_samples_value = 0;
            for (int i = 0; i < InputSignals.Samples.Count; i++)
            {
                accumulator_samples_value += InputSignals.Samples[i]; // each time add the sample value to the accumulator_samples_value
                OutputSignal.Samples.Add(accumulator_samples_value); // add the accmulator to the output samples values 

                // add the indices to the list int the output signal 
                OutputSignal.SamplesIndices.Add(i);
            }
        }
    }
}
