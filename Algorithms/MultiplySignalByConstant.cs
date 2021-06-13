using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class MultiplySignalByConstant : Algorithm
    {
        public Signal InputSignal { get; set; }
        public float InputConstant { get; set; }
        public Signal OutputMultipliedSignal { get; set; }

        public override void Run()
        {
            // Get New Reference 
            OutputMultipliedSignal = new Signal(new List<float>(), new bool());
            // loop on the samples of the input Signal and Then multiply each one with the constant 
            // After Multiplication Add It To the Output Signal 
            for (int i =0; i<InputSignal.Samples.Count; ++i)
                OutputMultipliedSignal.Samples.Add(InputSignal.Samples[i] * InputConstant); 
            //throw new NotImplementedException();
        }
    }
}
