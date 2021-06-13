using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;
using System.IO; 
namespace DSPAlgorithms.Algorithms
{
    public class InverseDiscreteFourierTransform : Algorithm
    {
        public Signal InputFreqDomainSignal { get; set; }
        public Signal OutputTimeDomainSignal { get; set; } 
        // we made this Struct to Save The Components In DFT As real Part and Imaginary Part
        struct number
        {
            public float real, imaginary; // Two Components To Comlex Number 
        }; 

        public override void Run()
        {
            // Make New refrence  
            OutputTimeDomainSignal = new Signal(new List<float>(), new bool());

            // Get the Number of Samples 
            int N = InputFreqDomainSignal.FrequenciesAmplitudes.Count;

            // x(n) = 1/N summition from(0 to N-1) of (x(k) * e^ j * (2 * Pi * n * k) / N) 
            // Construct the x(k) of Each Sample Using the Phase Shift And the Amplitude  


            // Get the Components of x(k) real and the imaginary in list A cos(theta) +jAsin(theta) 
            // make list of struct 

            bool read_file = true; 
            List<number> xk = new List<number>();
            // If We Run Test Case 
            if (!read_file)
            {
                for (int i = 0; i < N; ++i)
                {
                    number n = new number();  // make new variable of the struct 
                    // Get Amplitude and the Pahse Shift 
                    float A = InputFreqDomainSignal.FrequenciesAmplitudes[i];
                    float phase = InputFreqDomainSignal.FrequenciesPhaseShifts[i];
                    // calculate the real part with cos(theta) * A
                    n.real = (float)A * (float)Math.Cos(phase);
                    // calculate the imaginary part with sin(theta) * A
                    n.imaginary = (float)A * (float)Math.Sin(phase);
                    // add the variable to the list 
                    xk.Add(n);
                }
            }
            // read the same test Case From File Format in DFT the (Amplitude and the Phase Shift) 

            else
            {
                // Open the file To read 
                StreamReader rd = new StreamReader("signal_file.txt");
                // read the file to take the Amplitude and Phase Shift Of Each 
                string line = rd.ReadLine(); 
                while (line != null)
                {
                    if (line.Contains("-----"))
                    {
                        line = rd.ReadLine();  // read the next Line (Next Signal Data)
                        continue; // the end of the signal 
                    }
                    string[] terms = line.Split(' ');
                    float A = (float)Convert.ToDouble(terms[0]);  // First Read The Amplitude of the Sample 
                    float phase = (float)Convert.ToDouble(terms[2]);  // First Read The Amplitude of the Sample 
                    number n = new number();  // make new variable of the struct 
                    // calculate the real part with cos(theta) * A
                    n.real = (float)A * (float)Math.Cos(phase);
                    // calculate the imaginary part with sin(theta) * A
                    n.imaginary = (float)A * (float)Math.Sin(phase);
                    // add the variable to the list 
                    xk.Add(n);
                    line = rd.ReadLine(); // read the next line 
                }

                // Close the stream 
                rd.Close(); 
            }
            for (int n = 0; n < N; ++n) // loop on each Sample (To Get x(n))
            {
                // get the real part (with out j ) 
                // and the Imaginary Part 
                float real_part = 0, imaginary_part = 0;
                float res = 0; // The Output Result Component 
                float All_Imaginary = 0;  // This To Calculate the (r1 * I2 + r2 * I1); 
                float mul_imaginary = 0; // Multiplication of Imaginary  
                for (int k = 0; k < N; ++k)
                {
                    // In the equation we will Have -> real1 , real2 , Imaginary1, Imaginary2
                    // so the calculations is : r1*r2 + im1*im2 + (Imaginary and Real part) 
                    real_part = (float)Math.Cos((k * n * Math.PI * 2) / N); // r2
                    // Multiply by negative one as j *j = j^2 Whih is -1 so we need to remove it 
                    imaginary_part = -(float)Math.Sin((k * n * Math.PI * 2) / N); // I2
                    //x(k).real -> r1
                    //x(k).Imaginary -> I1 
                    res += (real_part * xk[k].real);
                    mul_imaginary += imaginary_part * xk[k].imaginary; 
                    // (r1 + I1) * (r2 + I2) = r1 * r2 + I1 * I2 + (r1 * I2 + r2 * I1) ; 
                    All_Imaginary += (real_part * xk[k].imaginary) + (imaginary_part * xk[k].real); 
                }
                res += All_Imaginary; // Add The Rest Of The Calculations 
                res += mul_imaginary;  // add the Imaginary Part
                res /= N; // Divide By N for Normalization and the (Equation of IDFT)
                // Truncate to Make The Sample with no decimal Digit after decimal Point 
                OutputTimeDomainSignal.Samples.Add((float)Math.Truncate(res));
            }
        }
    }
}
