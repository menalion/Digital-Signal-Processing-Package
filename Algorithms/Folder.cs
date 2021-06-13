using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Folder : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputFoldedSignal { get; set; }

        public override void Run()
        {
            List<float> samples_left = new List<float>(); // get values on the left of origin
            List<float> samples_right = new List<float>(); // get the vlaues on the right of the origin 
            List<int> sample_left_indices = new List<int>();// to get the left indices
            List<int> sample_right_indices = new List<int>();// to get the right indices

            List<float> out_put_samples = new List<float>();  // get the output samples  
            List<int> out_put_samples_indices = new List<int>();  // get the output samples indices
            // move on the indexes of the list to get the origin index
            int origin_index = 0;
            for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
            {
                // if the value is zero so it is the origin
                if (InputSignal.SamplesIndices[i] == 0)
                {
                    origin_index = i; // get the value
                    break; // then break
                }
            }

            // count the samples in the both sides of the right and left  
            int count_left = 0, count_right = 0;
            bool center_singal = false; // to know the centre of the signal 
            for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
            {
                if (center_singal) count_right++;
                else count_left++;
                if (InputSignal.SamplesIndices[i] == 0) center_singal = true;

            }
            // get the left samples in the left list and their indices 
            int count_indices = 1; // to add to the list of the indices
            for (int i = origin_index - 1; i >= 0; i--)
            {
                samples_left.Add(InputSignal.Samples[i]);
                sample_left_indices.Add(count_indices);
                count_indices++;
            }

            // get the left samples in the right list and their indices 
            count_indices = -1; // to add to the list of the indices
            for (int i = origin_index + 1; i < InputSignal.SamplesIndices.Count; i++)
            {
                samples_right.Add(InputSignal.Samples[i]);
                sample_right_indices.Add(count_indices);
                count_indices--;
            }

            // get the total list of samples values and indices to merge them 
            for (int i = samples_right.Count - 1; i >= 0; --i) out_put_samples.Add(samples_right[i]);
            // add the value of the origin 
            out_put_samples.Add(InputSignal.Samples[origin_index]);
            for (int i = 0; i < samples_left.Count; ++i) out_put_samples.Add(samples_left[i]);


            // get the total list of samples values and indices to merge them  
            for (int i = sample_right_indices.Count - 1; i >= 0; --i) out_put_samples_indices.Add(sample_right_indices[i]);
            // add the index of the origin 
            out_put_samples_indices.Add(0);
            for (int i = 0; i < sample_left_indices.Count; ++i) out_put_samples_indices.Add(sample_left_indices[i]);

            // make the floded output singal 
            OutputFoldedSignal = new Signal(out_put_samples, out_put_samples_indices, new bool());
            // mark the signal as floded by this way if floded two times then it back to the original one

            OutputFoldedSignal.folded = (!InputSignal.folded);
        }

    }
}
