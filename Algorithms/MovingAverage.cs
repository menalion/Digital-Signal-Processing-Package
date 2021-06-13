using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class MovingAverage : Algorithm
    {
        public Signal InputSignal { get; set; }
        public int InputWindowSize { get; set; }
        public Signal OutputAverageSignal { get; set; }
 
        public override void Run()
        {
            // first make list of samples for the out put signal
            List<float> out_put_samples = new List<float>(); 
            // move on the signal one dimension with all sampels except the last samples with the same size as window
            for(int i=0;i<=InputSignal.Samples.Count-InputWindowSize;i++)
            {
                float sum = 0; // make sum to put it in the output samples
                for (int j = 0; j < InputWindowSize; j++)
                    sum += InputSignal.Samples[j+i];// calculate the sum of the window indexes with the original index
                out_put_samples.Add(sum / InputWindowSize); // get average (number of elements = window size)
            }
            // put the samples in the output singal with the same statu of the periodic in the input singal
            OutputAverageSignal = new Signal(out_put_samples, InputSignal.Periodic);
        }
    }
}
