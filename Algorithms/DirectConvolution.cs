using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class DirectConvolution : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputConvolvedSignal { get; set; }

        /// <summary>
        /// Convolved InputSignal1 (considered as X) with InputSignal2 (considered as H)
        /// </summary>
        public override void Run()
        {
            // make the output singal intilization with new list of samples and idices and the same periodic status of input
            OutputConvolvedSignal = new Signal(new List<float>(), new List<int>(), InputSignal1.Periodic); 

            // to remove the zero element in the value of samples in some singals for simplification 
            while (InputSignal1.Samples[InputSignal1.Samples.Count - 1] == 0)
                InputSignal1.Samples.RemoveAt(InputSignal1.Samples.Count - 1); // use the remove fuction  


            // the number of outputs is the summition of singal 1 and singal 2
            int iterations = InputSignal1.Samples.Count + InputSignal2.Samples.Count;

            bool success_cunvolve = false; // indicate if the we do at least on multiply operation to add them to the output signal
            for (int i = 0; i<iterations; i++)
            {
                // the convolution equation is summition of (x(n)* h(n-k))
                // every time the n take the i of the iterations 
                float sum_values = 0; // a sum to add all values
                int singal_n = i; // for each iteration
                int mask_k = 0; // the mask for the h
                success_cunvolve = false; // for each iteration do new sum , new flag and new k form 0 to n
                // loop on all possible k that is less than or equals to n
                while (mask_k <= singal_n)
                {
                    // the n-k is for singal 2 and k for signal 1 
                    if ((singal_n - mask_k) < InputSignal2.Samples.Count && mask_k < InputSignal1.Samples.Count)
                    {
                        // apply the covolution equation
                        sum_values += (InputSignal1.Samples[mask_k] * InputSignal2.Samples[singal_n - mask_k]);
                        success_cunvolve = true; // make the flag to true (indicates it's successful operation for these values k and n)
                    }
                    mask_k++; // increament the counter
                }
                // if it's a successful operation then add it to the output signal
                if (success_cunvolve) OutputConvolvedSignal.Samples.Add(sum_values);
                
            }

            // the start is form the index as the summiton of the start of the two values 
            int start_index = InputSignal1.SamplesIndices[0] + InputSignal2.SamplesIndices[0]; 
            // each time add the start index then increament it by one
            for (int i = 0; i < OutputConvolvedSignal.Samples.Count; i++) OutputConvolvedSignal.SamplesIndices.Add(start_index++);
        }
    }
}
