using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastConvolution : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputConvolvedSignal { get; set; } 
        // those two singals to save the signal converted form the time domain to the frequency domain
        List<ComplexNumber> FFTSignal1 = new List<ComplexNumber>();
        List<ComplexNumber> FFTSignal2 = new List<ComplexNumber>();
        // this function to get the components of the signal form the givin amplitude and phase shift (For FFT)
        public List<ComplexNumber> get_siganl_form_amplitude_and_pahse_shift(List<float> amplitued_signal, List<float> phase_singal)
        {
            int count_siganl = amplitued_signal.Count; // get the number of the signal compontnets  
            // make new singal form complxe number class (a + or -  jb)
            List<ComplexNumber> signal = new List<ComplexNumber>();
            for (int i = 0; i < count_siganl; i++)
            {
                // make the complex number form the real and imaginary part
                ComplexNumber comblex_num = new ComplexNumber();

                // the real part is the cos of pahse shift multiply the amplitude
                comblex_num.Reyal = amplitued_signal[i] * (float)Math.Cos(phase_singal[i]);

                // the imaginary  part is the sin of pahse shift multiply the amplitude
                comblex_num.Imagenary = amplitued_signal[i] * (float)Math.Sin(phase_singal[i]);

                // add the complex numnber to the desired signal to get
                signal.Add(comblex_num);
            }
            return signal; // return with the singal 
        }

        // this fucntion to get the phase and the amplitude of givin signal (for IFFT)
        public Signal get_pahse_and_amplitude_from_singal(ComplexNumber[] signal_components)
        {
            // make new refernce for this singal as the peridic , list of F amplitudes and list of F pahse shifts
            // note we are in the frequency domain
            Signal frequency_signal_amplitude_phase_shift = new Signal(false, new List<float>(), new List<float>(), new List<float>());
            int count_elements = signal_components.Length; // get the number of the components in the list 
            for (int i = 0; i < count_elements; i++) // loop on the components 
            {
                // the amplitude is the sqr(a * a + b * b) as the a is real part and b is the imaginary part
                frequency_signal_amplitude_phase_shift.FrequenciesAmplitudes.Add((float)Math.Sqrt((signal_components[i].Reyal * signal_components[i].Reyal)
                    + (signal_components[i].Imagenary * signal_components[i].Imagenary)));

                // the pahse shift is the inverse tan of (b / a) as b is the imaginary part and a is the real part (use the A Tan)
                frequency_signal_amplitude_phase_shift.FrequenciesPhaseShifts.Add((float)Math.Atan2(signal_components[i].Imagenary,
                    signal_components[i].Reyal));
            }
            return frequency_signal_amplitude_phase_shift; // return with the resulted singal
        }
        public override void Run()
        {
            int singal_1_size = InputSignal1.Samples.Count;
            int singal_2_size = InputSignal2.Samples.Count;

            //////////////////////////////////////////////////////////////////
            // pad the signals with zeros according to each other

            // loop on the size of singal two to pad signal 1 
            for (int i = 0; i < singal_2_size - 1; i++)  InputSignal1.Samples.Add(0);
            // loop on the size of singal one to pad signal 2
            for (int i = 0; i < singal_1_size - 1; i++)  InputSignal2.Samples.Add(0); 
            //////////////////////////////////////////////////////////////////


            // make object form the fast fourior transform to convert the singal form time domain to frequency domain
            FastFourierTransform FFT = new FastFourierTransform();
            // make the inpur to fast FF as the input signal to covert 
            FFT.InputTimeDomainSignal = InputSignal1;
            FFT.Run();  // run the fast frouior transfrom (like in test Casess) 


            
            // the output form fast fourior transform is the amplitude and the pahse shift so we need to get the signal form them 
            // we call this function for this 
            FFTSignal1 = get_siganl_form_amplitude_and_pahse_shift(FFT.OutputFreqDomainSignal.FrequenciesAmplitudes,FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts);

            // make the same for the singal 2 like singal 1 
            
            // make the inpur to fast FF as the input signal to covert 
            FFT.InputTimeDomainSignal = InputSignal2;
            FFT.Run();// run the fast frouior transfrom (like in test Casess) 
            // the output form fast fourior transform is the amplitude and the pahse shift so we need to get the signal form them 
            // we call this function for this Function
            FFTSignal2 = get_siganl_form_amplitude_and_pahse_shift(FFT.OutputFreqDomainSignal.FrequenciesAmplitudes,FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts);

            // the convolution in the frouior transform is the multiplication between the components of the two signals 
            // both singals are the same size so any one of them 
            int convolution_count = FFTSignal1.Count;
            ComplexNumber[] ConvFFT = new ComplexNumber[convolution_count];

            for (int i = 0; i < convolution_count; i++)
            { 
                // now let's make the convolution in the frequency domain 
                
                ComplexNumber total_complex_number; // here we will save the multiplication of the two numbers
                total_complex_number = new ComplexNumber(); // make new refernce

                // the complex real multiplication number is the (real_1 * real_2) - (imaginary_1 * imaginary_2)
                total_complex_number.Reyal = (FFTSignal1[i].Reyal * FFTSignal2[i].Reyal) - (FFTSignal1[i].Imagenary * FFTSignal2[i].Imagenary);
                // the imiginary component is the  (imaginary_1 * real_2) + (imaginary_2 * real_1) 
                total_complex_number.Imagenary = (FFTSignal1[i].Reyal * FFTSignal2[i].Imagenary) + (FFTSignal1[i].Imagenary * FFTSignal2[i].Reyal);
                ConvFFT[i] = total_complex_number; // add the complex number to the list
            } /*Get pahse , amp to convert to time domain */ 

            // now get the amplitude and the pahse shift form the signal to use them in the inverse fast fourouir transform 
            // to get the singal again in the time domain
            Signal frequency_signal_amplitude_phase_shift = get_pahse_and_amplitude_from_singal(ConvFFT);

            // make new object form the fast fourouir transform 
            InverseFastFourierTransform IFFT = new InverseFastFourierTransform();
            IFFT.InputFreqDomainSignal = frequency_signal_amplitude_phase_shift; //this is the input of fast fourouir transform 
            // fast fourouir transform run to convert the singal (Like in Test Cases)
            IFFT.Run();
            // now we have the singal convolved in the time domain add it to the output singal  
            OutputConvolvedSignal = new Signal(IFFT.OutputTimeDomainSignal.Samples, IFFT.OutputTimeDomainSignal.SamplesIndices, new bool());
        }
    }
}
