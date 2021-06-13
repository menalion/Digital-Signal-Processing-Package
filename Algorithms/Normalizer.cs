using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Normalizer : Algorithm
    {
        public Signal InputSignal { get; set; }
        public float InputMinRange { get; set; }
        public float InputMaxRange { get; set; }
        public Signal OutputNormalizedSignal { get; set; }

        public override void Run()
        {
            // Make New Refrence  

            OutputNormalizedSignal = new Signal(new List<float>(), new bool()); 
            // Here I use the min max normalization 
            // RULE :  ((x - min) / (max - min)) * (b - a) + a 
            // The A and B is the beginning of the range and the end of the range  

            // Get the minimum and the maximum Range of Singal Samples
            float min_sample = InputSignal.Samples.Min();
            float max_sample = InputSignal.Samples.Max();
            // loop on all of the samples  
            for(int i =0;i<InputSignal.Samples.Count; ++i)
            {
                float sample = InputSignal.Samples[i];
                OutputNormalizedSignal.Samples.Add(((sample - min_sample) / (max_sample - min_sample)) * (InputMaxRange - InputMinRange) + InputMinRange);
            }
            //throw new NotImplementedException();
        }
    }
}
