using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{

    // This Struct is used for the complex numbers (a (+ or -) jb as a is the real part and b is the imaginary part)
    struct Complex_Number
    {
        public double Real, Imag; // donated for the real and imaginary part of the number
    }
    public class FastFourierTransform : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public int InputSamplingFrequency { get; set; }
        public Signal OutputFreqDomainSignal { get; set; }

        // get the number of components of the signal in time domain as N
        public int N { get; set; }
        public override void Run()
        {

            // first we want to get the time of the algorithm so we make a wathc
            var watch = new System.Diagnostics.Stopwatch();
            // then triger it to start
            watch.Start();

            // make new refrence for the output signal as for : list of frequency amplitudes , frequency phase shifts 
            // and the frequncy (angular frequency)
            OutputFreqDomainSignal = new Signal(false, new List<float>(), new List<float>(), new List<float>());

            // get the number of components of the input signal in the time domian
            N = InputTimeDomainSignal.Samples.Count;

            // make array or list of the samples that will be calculated in the frequency domain 
            // initialy  make the real part as the value of the sampel in the time domain 
            // the imiginary part as 0
            Complex_Number[] Samples = new Complex_Number[N];
            for (int i = 0; i < N; i++)
            {
                // make the complex number (real and imaginary part)
                Complex_Number complex_sample ;
                complex_sample.Real = InputTimeDomainSignal.Samples[i]; // assign the value of the sample to the real part
                complex_sample.Imag = 0; // assign the value of the imaginary part to 0
                Samples[i] = complex_sample; // add the complex number to the list 
            }

            // now call this fucntion to convert from the time domain to the frequency domain using the Fast_Foriour_Transform
            Fast_Foriour_Transform(ref Samples);


            for (int k = 0; k < N; k++)
            {
                // the amplitude is the sqr(a * a + b * b) as the a is real part and b is the imaginary part
                OutputFreqDomainSignal.FrequenciesAmplitudes.Add((float)Math.Sqrt((Samples[k].Real * Samples[k].Real)
                    + (Samples[k].Imag * Samples[k].Imag)));

                // the pahse shift is the inverse tan of (b / a) as b is the imaginary part and a is the real part (use the A Tan)
                OutputFreqDomainSignal.FrequenciesPhaseShifts.Add((float)Math.Atan2(Samples[k].Imag, Samples[k].Real));
                // calculate the frequency (digital angular frequency using this equation)->
                // (2 * FS * PI * number of component) / N AS 
                // N : the number of components in the frequency domain 
                // FS : Sampling Frequency
                OutputFreqDomainSignal.Frequencies.Add((2 * InputSamplingFrequency * (k + 1) * (float)Math.PI)
                    / InputTimeDomainSignal.Samples.Count);
            }

            // now top the clock after the code has  finished
            watch.Stop();
            Console.WriteLine("Execution Time: ", watch.ElapsedMilliseconds);
        }

        // This fucntion to convert form the time domain to the frequency domain 
        void Fast_Foriour_Transform(ref Complex_Number[] Samples)
        {
            // get the length of the samples 
            int N = Samples.Length;
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
                even[i] = Samples[i * 2]; // donated for even -> 2 * i 
                odd[i] = Samples[i * 2 + 1]; // donated for the odd -> 2 * i + 1
            }

            /////////////////////////////////////////////////////////////////////////
            // recursevly call back with each of the two lists (even and odd )
            Fast_Foriour_Transform(ref even);
            Fast_Foriour_Transform(ref odd);
            ///////////////////////////////////////////////////////////////////////// 

            // loop to the mid of the list (as the rest is just with the period property)
            for (int k = 0; k < N / 2; k++)
            {
                // get the power of the exponential which is -> (2 * PI * k(number of the component))/N(total number of elements)
                double exponentail_power = (2 * Math.PI * k / N);

                // make two complex numbers one for the total result and one for the multiplication operation
                Complex_Number odd_term, total_number;

                // the exponential using the eulr method is cos(x) - j sin(x) 
                // the real part is the cos 
                // the imaginary part is the sin 
                // the x is the exponentail_power 
                odd_term.Real = Math.Cos(exponentail_power);
                odd_term.Imag = -1 * Math.Sin(exponentail_power);


                // for sampels before the mid (we will add the real part for real and imaginary part for imaginary) 
                // the complex real multiplication number is the (real_1 * real_2) - (imaginary_1 * imaginary_2)
                total_number.Real = even[k].Real + (odd_term.Real * odd[k].Real - odd_term.Imag * odd[k].Imag);
                // the imiginary component is the  (imaginary_1 * real_2) + (imaginary_2 * real_1)
                total_number.Imag = even[k].Imag + (odd_term.Real * odd[k].Imag + odd_term.Imag * odd[k].Real);

                // add the total number to the list befor the mid 
                Samples[k] = total_number;

                // for sampels after the mid (we will subtract the real part for real and imaginary part for imaginary) 
                // the complex real multiplication number is the (real_1 * real_2) - (imaginary_1 * imaginary_2)
                total_number.Real = even[k].Real - (odd_term.Real * odd[k].Real - odd_term.Imag * odd[k].Imag);
                // the imiginary component is the  (imaginary_1 * real_2) + (imaginary_2 * real_1)
                total_number.Imag = even[k].Imag - (odd_term.Real * odd[k].Imag + odd_term.Imag * odd[k].Real);

                // add the total number to the list after the mid 
                Samples[k + N / 2] = total_number;
            }
        }
    }
}
