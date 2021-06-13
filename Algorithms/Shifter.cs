using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Shifter : Algorithm
    {
        public Signal InputSignal { get; set; }
        public int ShiftingValue { get; set; }
        public Signal OutputShiftedSignal { get; set; }

        public override void Run()
        {
            // normal -> + -> left make it - 
            // normal -> - ->right make it +
            // first handle if the input signal is floded or not 
            if (!InputSignal.folded) ShiftingValue *= -1;
            // make new list of the new shifted values of the indices 
            List<int> new_indices = new List<int>();
            for(int i=0; i<InputSignal.SamplesIndices.Count; ++i) new_indices.Add(InputSignal.SamplesIndices[i] + ShiftingValue);

            // make the output singal with the new indices and shifited values 
            OutputShiftedSignal = new Signal(InputSignal.Samples, new_indices, new bool());
            // take the statue of the folded signal is true or flase 
            OutputShiftedSignal.folded = false; 
        }
    }
}
