using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;
using System.IO;

namespace DSPAlgorithms.Algorithms
{
    public class DiscreteFourierTransform : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public float InputSamplingFrequency { get; set; } // for drawing the signal 
        public Signal OutputFreqDomainSignal { get; set; }
        public override void Run()
        {
            // first we want to get the time of the algorithm so we make a wathc
            var watch = new System.Diagnostics.Stopwatch();
            // then triger it to start
            watch.Start(); 


            // make new refrences 
            OutputFreqDomainSignal = new Signal(new List<float>(), new bool());
            OutputFreqDomainSignal.FrequenciesAmplitudes = new List<float>();
            OutputFreqDomainSignal.FrequenciesPhaseShifts = new List<float>();

            // Get N The Number of Samples 
            int N = InputTimeDomainSignal.Samples.Count; 
            for (int k = 0; k<InputTimeDomainSignal.Samples.Count; ++k) // loop to get each x(k) 
            {
                // we will split the results into two parts the real part and the imaginary part  
                float real_part = 0, imaginary_part = 0;  
                // loop on x(n) the samples 
                for (int n=0;n<InputTimeDomainSignal.Samples.Count; ++n)
                {
                    // new eqauation is 
                    // x(n) cos((kn 2 * 180) / N) This is the real part 
                    // -j x(n) sin((kn * 2 * 180) / N) This is the Imaginary Part  
                    // x(n) is the value of the sample  
                    // we will sum all x(n) for all Samples  
                    
                    // calculate the real part 
                    real_part += InputTimeDomainSignal.Samples[n] *
                        (float)Math.Cos((2 * Math.PI * k * n) / N);  
                    
                    // calculate the Imaginary Part 
                    imaginary_part += -(InputTimeDomainSignal.Samples[n] *
                        (float) Math.Sin((2 * Math.PI * k * n) / N));
                }

                // we calculated The Total Real Part and Imaginary Part for x(k) 

                // Get the Amplitude  = sqrt(real ^2 + Imaginary ^2)   
                float number_for_angle = (float)((imaginary_part / real_part) * (float)Math.PI) / 180; 
                OutputFreqDomainSignal.FrequenciesAmplitudes.Add(((float)Math.Sqrt((real_part * real_part) + (imaginary_part * imaginary_part))));

                // Get The Phase Shift as It's Tan^-1 for (b/a)   
                // Atan Takes Two Parameters And Return The Negatvie Angle between Them  So Put (-1) And Multiply By It 
                OutputFreqDomainSignal.FrequenciesPhaseShifts.Add((float)Math.Atan2(imaginary_part, real_part));
            }

            // Put the Frequencis Of The Samples According To The Equation Omega = (2 PI * fs) / N 
            // Make List Of Angular Frequences
            List<float> angular_frequences = new List<float>();
            // The Begin Is Zero with repect to zero  
            angular_frequences.Add(0);
            for (int i = 0; i < InputTimeDomainSignal.Samples.Count; ++i)
                angular_frequences.Add(((float)Math.PI * 2 * InputSamplingFrequency * (i + 1)) / N);
            

            // Save Data To File To Use It In the IDFT 
            // we will Save For Each Signal : Polar Form Which is the A (cos(theta) + jsin(Theta))
            // Another Approch is That  |A|e^(j Theta) 
            StreamWriter wr = new StreamWriter("signal_file.txt");
            for (int i = 0; i < InputTimeDomainSignal.Samples.Count; ++i)
            {
                // phase.ToString() + " )" + "j sin( " + phase.ToString() + " )"
                float A = Math.Abs(OutputFreqDomainSignal.FrequenciesAmplitudes[i]); // Get Amplitude 
                float phase = OutputFreqDomainSignal.FrequenciesPhaseShifts[i]; // Get Phase Shift  

                wr.Write(string.Format("{0:0.###############}", Convert.ToDouble(A))); // write the Amplitude With 15 decimal Placese
                wr.Write(" (" + "cos( ");
                wr.Write(string.Format("{0:0.###############}", Convert.ToDouble(phase))); // Write the Phase Shift With 15 Decimal Places
                wr.WriteLine(" )" + "j sin( " + phase.ToString() + " )"); 
            }
            wr.WriteLine("-------------------------------------"); // Write the End Of The Signal Samples
            wr.Close();
            // Now We Have The Amplitude And Frequency Of Each Smaple Of the Descrete Signal in Foriour Transform 
            // (DFT) 
            // Calculate the List Of X-axis in the Plot 
            // Sigma (radian frequency) -> in DFT = (2 * PI * fs * i) / N as i is the index of sample  

            // add the frequencies to the output signal to plot them  
            OutputFreqDomainSignal.Frequencies = new List<float>(); 
            for (int i = 0; i < N; ++i)
                OutputFreqDomainSignal.Frequencies.Add((2 * InputSamplingFrequency * (i + 1) * (float)Math.PI) / N);

            // Now We need To Plot Two Charts -> 1)Amplitude and X_axis
            //                                   2) Phase Shift And X_axis 
            // The Code Is In The Discrete Fourior Transform .cs inside the Components 

            // now top the clock after the code has  finished
            watch.Stop();
            Console.WriteLine("Execution Time: ", watch.ElapsedMilliseconds);
        }
    }
}
