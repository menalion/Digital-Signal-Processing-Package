using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    
    public class InverseFastFourierTransform : Algorithm
    {
        // This Struct is used for the complex numbers (a (+ or -) jb as a is the real part and b is the imaginary part)
        struct Complex_Number
        {
            public double Real, Imag; // donated for the real and imaginary part of the number
        } 


        public Signal InputFreqDomainSignal { get; set; }
        public Signal OutputTimeDomainSignal { get; set; }

        public override void Run()
        {
            // get the size of the input singal (number of components in the frequency domain)  as N
            int N = InputFreqDomainSignal.FrequenciesAmplitudes.Count;

            // make the references of the out put time domain singal of the list of samples and frequencies 
            OutputTimeDomainSignal = new Signal(false, new List<float>(), new List<float>(), new List<float>()); 
            // make new reference for the samples list 
            OutputTimeDomainSignal.Samples = new List<float>(); 

            // here make array of the signal in the comblex number format 
            Complex_Number[] signal = new Complex_Number[N];
            for (int i = 0; i < N; i++)
            {
                // make a comblex number 
                Complex_Number comblex_num; 
                // the real part is the frequency (omega) multiply by cos(phase shift)
                comblex_num.Real = (float)(InputFreqDomainSignal.FrequenciesAmplitudes[i] *
                Math.Cos(InputFreqDomainSignal.FrequenciesPhaseShifts[i]));

                // the imaginary part is the frequency (omega) multiply by sin(phase shift)
                comblex_num.Imag = (float)(InputFreqDomainSignal.FrequenciesAmplitudes[i] *
                    Math.Sin(InputFreqDomainSignal.FrequenciesPhaseShifts[i]));
                // we get now the comblex component to get the time domain singal so add it to the list 
                signal[i]= comblex_num; // add the element to the array 
            }

            // now we want to convert form the frequency domain to the time domian so we will use this function Inverse_Fast_Furiour_Transform 
            // Inverse_Fast_Furiour_Transform
            Inverse_Fast_Furiour_Transform(ref signal);

            // now loop on the time domain components  
            // in the inverse we divide each component by the total Number of components 
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i].Real = signal[i].Real / signal.Length; // divide each real component with the length (actual component)
            }

            // now add those real values to the output time domian signal 
            for (int i = 0; i < signal.Length; i++)
                // cast to float and round them to 1 for the error correction in the test cases 
                OutputTimeDomainSignal.Samples.Add((float)Math.Round(signal[i].Real,1));
        }

        // this function to convert form frequecny domain to time domain
        static void Inverse_Fast_Furiour_Transform(ref Complex_Number[]signal)
        {
            // get the length of the samples 
            int N = signal.Length;
            // first we make list or array with complex number class with the size of the input components
            Complex_Number[] output = new Complex_Number[N];

            // Base Case  if we reach only one sample int the list Then we return (Do Nothing)
            if (N == 1) return;

            // make two lists one for the even part indices and one for the odd part indices 
            Complex_Number[] even = new Complex_Number[N / 2];
            Complex_Number[] odd = new Complex_Number[N / 2];

            // loop only to the half of the indices of elements we have to split 
            for (int i = 0; i < N / 2; i++)
            {
                even[i] = signal[i * 2]; // donated for even -> 2 * i 
                odd[i] = signal[i * 2 + 1]; // donated for the odd -> 2 * i + 1
            }

            /////////////////////////////////////////////////////////////////////////
            // recursevly call back with each of the two lists (even and odd )
            Inverse_Fast_Furiour_Transform(ref even);
            Inverse_Fast_Furiour_Transform(ref odd);
            ///////////////////////////////////////////////////////////////////////// 

            // loop to the mid of the list (as the rest is just with the period property)
            for (int k = 0; k < N / 2; k++)
            {
                // get the power of the exponential which is -> (2 * PI * k(number of the component))/N(total number of elements)
                double exponentail_power = (2 * Math.PI * k / N);

                // make two complex numbers one for the total result and one for the multiplication operation
                Complex_Number odd_term, total_number;

                // the exponential using the eulr method is cos(x) + j sin(x) 
                // the real part is the cos 
                // the imaginary part is the sin 
                // the x is the exponentail_power 
                odd_term.Real = (float)Math.Cos(exponentail_power);
                odd_term.Imag = (float)Math.Sin(exponentail_power); 


                // for sampels before the mid (we will add the real part for real and imaginary part for imaginary) 
                // the complex real multiplication number is the (real_1 * real_2) - (imaginary_1 * imaginary_2)
                total_number.Real = even[k].Real + (odd_term.Real * odd[k].Real - odd_term.Imag * odd[k].Imag);
                // the imiginary component is the  (imaginary_1 * real_2) + (imaginary_2 * real_1)
                total_number.Imag = even[k].Imag + (odd_term.Real * odd[k].Imag + odd_term.Imag * odd[k].Real);

                // add the total number to the list befor the mid 
                signal[k] = total_number;

                // for sampels after the mid (we will subtract the real part for real and imaginary part for imaginary) 
                // the complex real multiplication number is the (real_1 * real_2) - (imaginary_1 * imaginary_2)
                total_number.Real = even[k].Real - (odd_term.Real * odd[k].Real - odd_term.Imag * odd[k].Imag);
                // the imiginary component is the  (imaginary_1 * real_2) + (imaginary_2 * real_1)
                total_number.Imag = even[k].Imag - (odd_term.Real * odd[k].Imag + odd_term.Imag * odd[k].Real);

                // add the total number to the list after the mid 
                signal[k + N / 2] = total_number;
            }
        }
    }
}
