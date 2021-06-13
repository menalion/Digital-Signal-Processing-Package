using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;
namespace DSPAlgorithms.Algorithms
{
    public class QuantizationAndEncoding : Algorithm
    {
        // You will have only one of (InputLevel or InputNumBits), the other property will take a negative value
        // If InputNumBits is given, you need to calculate and set InputLevel value and vice versa
        public int InputLevel { get; set; }
        public int InputNumBits { get; set; }
        public Signal InputSignal { get; set; }
        public Signal OutputQuantizedSignal { get; set; }
        public List<int> OutputIntervalIndices { get; set; }
        public List<string> OutputEncodedSignal { get; set; }
        public List<float> OutputSamplesError { get; set; }
        public override void Run()
        {
            // make new refrecnce of each output list 
            OutputQuantizedSignal = new Signal(new List<float>(), new bool()); // add the midpoints values to the putput signal samples 
            OutputIntervalIndices = new List<int>(); // Note the intervall indices are one based (Begins From 1 )
            OutputEncodedSignal = new List<string>(); // using number of bits 
            OutputSamplesError = new List<float>(); //Error of Each One not sqaure of the errors (Error = Output sample value - Input Sample Value)

            int levels = 0; // inital value of the number of levels
                            // if the number of bits is  negatvie we will take the number of levels directly 
            if (InputNumBits <= 0) levels = InputLevel;
            // calcualte the number of levels form the bits (2 ^ number of bits = number of levels )
            else
            {
                int multiplication = 1; 
                for(int i =0;i<InputNumBits; ++i)  multiplication *= 2;
                levels = multiplication; 
            }

            //we get the number of levels we need to get the number of bits needed for these levels 
            // we will use it in the encode 
            int bits =  Convert.ToString(levels - 1, 2).Length ; 
            

            // get the min and max sample of the input signal 
            float min_sample = InputSignal.Samples.Min();
            float max_sample = InputSignal.Samples.Max();

            // get the resolution of the quantization 
            float resolution = (max_sample - min_sample) / levels;

            // make list and append the levels to the list from min to the max with intervals 
            List<float> intervals = new List<float>();
            float temp = min_sample;
            // loop until reach the max_sample value
            while (temp <= max_sample)
            {
                float temp_2 = (float)Math.Round(temp, 2);
                intervals.Add(temp_2);
                temp += resolution; 
            }
            // append the max sample value to the list 
            intervals.Add(max_sample);

            // now make a list with the mid points of each  Interval  
            List<float> midpoints = new List<float>();
            for (int i = 0; i < intervals.Count - 1; ++i)
            {
                // calculate midpoint of each interval 
                float midpoint = (intervals[i] + intervals[i + 1]) / 2;
                midpoints.Add(midpoint);  // add the mid point to the list 
            }

            // Encode each sample to its level   
            // Get the intervall indices for each sample 
            // for each sample loop on the entire intervals to know the postion to encode 
            for (int i =0;i<InputSignal.Samples.Count; ++i)
            {
                float sample = InputSignal.Samples[i];
                for (int j =0;j<intervals.Count - 1; ++j)
                {
                    // if the interval betwen j and j+1 was the correct interval then 
                    if (sample >= intervals[j] && sample <= intervals[j+1])
                    {
                        OutputEncodedSignal.Add(Convert.ToString(j, 2).PadLeft(bits, '0')); // Convert the j (index of interval) 
                        OutputIntervalIndices.Add(j + 1); // as the intervall indices are one based 
                        OutputQuantizedSignal.Samples.Add(midpoints[j]); // add the midpoint as the new sample of the output signal 
                        break; // break the Inside loop to get the next sample 
                    }
                }
            }

            float total_square_Error = 0;
             // Calculate the error for each sample 
             for (int i =0;i<InputSignal.Samples.Count; ++i)
               OutputSamplesError.Add(OutputQuantizedSignal.Samples[i] - InputSignal.Samples[i]);  // Get the result of the diffrence  

            // Calcuate the Sqaure Error  
            for (int i = 0; i < OutputSamplesError.Count; ++i)
            {
                total_square_Error += OutputSamplesError[i] * OutputSamplesError[i]; 
            }
            total_square_Error /= OutputSamplesError.Count; 
            
        } 
        
    }
}
