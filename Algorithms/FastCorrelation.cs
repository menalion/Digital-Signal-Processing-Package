using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastCorrelation : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public List<float> OutputNonNormalizedCorrelation { get; set; }
        public List<float> OutputNormalizedCorrelation { get; set; }


        // this function to calculate the normalization term of the auto_correlation
        public float get_normalized_term_auto_correlation()
        {
            // the correlation is givin by the formula of -> sqr(for all components i -> x1(i) * x2(i))
            // the normalized corrleation is the (corr / (1 / N) * sqr(summition of all components (xi)^2))
            float sum_values = 0; // make variable sum to add the summition of all compoents (x(i) ^ 2)

            // here we get the square summition of all the components 
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
                sum_values += InputSignal1.Samples[i] * InputSignal1.Samples[i];

            // now get the normalized term (1/N * sqr(sum values * sumvlaues -> summition all values square))
            float Normalized = (float)1 / InputSignal1.Samples.Count * (float)Math.Sqrt(sum_values * sum_values);
            // return the normalization term
            return Normalized;
        }

        public float get_normalized_term_cross_correlation()
        {
            // the correlation is givin by the formula of -> sqr(for all components i -> x1(i) * x2(i))
            // the normalized corrleation is the (corr / (1 / N) * sqr(summition of all components (xi)^2))
            float sum_values_1 = 0, sum_values_2 = 0;
            // here we get the square summition of all the components of the first singal 
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
                sum_values_1 += InputSignal1.Samples[i] * InputSignal1.Samples[i];
            // here we get the square summition of all the components of the Second singal 
            for (int i = 0; i < InputSignal2.Samples.Count; i++)
                sum_values_2 += InputSignal2.Samples[i] * InputSignal2.Samples[i];
            // now get the normalized term (1/N * sqr(sum values_1 * sumvlaues_2 -> summition all values square))
            float Normalized = (float)1 / InputSignal1.Samples.Count * (float)Math.Sqrt(sum_values_1 * sum_values_2);

            return Normalized;// return the normalization term
        }


        // this function to get the components of the signal form the givin amplitude and phase shift (For FFT)
        public List<ComplexNumber> get_siganl_form_amplitude_and_pahse_shift(List<float> amplitued_signal, List<float> phase_singal)
        {
            int count_siganl = amplitued_signal.Count; // get the number of the signal compontnets  
            // make new singal form complxe number class (a + or -  jb)
            List<ComplexNumber> signal = new List<ComplexNumber>();
            for (int i = 0; i < count_siganl; i++)
            {
                // make the ComplexNumber number form the Reyal and Imagenary part
                ComplexNumber comblex_num = new ComplexNumber();

                // the Reyal part is the cos of pahse shift multiply the amplitude
                comblex_num.Reyal = amplitued_signal[i] * (float)Math.Cos(phase_singal[i]);

                // the Imagenary  part is the sin of pahse shift multiply the amplitude
                comblex_num.Imagenary = amplitued_signal[i] * (float)Math.Sin(phase_singal[i]);

                // add the ComplexNumber numnber to the desired signal to get
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
                // the amplitude is the sqr(a * a + b * b) as the a is Reyal part and b is the Imagenary part
                frequency_signal_amplitude_phase_shift.FrequenciesAmplitudes.Add((float)Math.Sqrt((signal_components[i].Reyal * signal_components[i].Reyal)
                    + (signal_components[i].Imagenary * signal_components[i].Imagenary)));

                // the pahse shift is the inverse tan of (b / a) as b is the Imagenary part and a is the Reyal part (use the A Tan)
                frequency_signal_amplitude_phase_shift.FrequenciesPhaseShifts.Add((float)Math.Atan2(signal_components[i].Imagenary,
                    signal_components[i].Reyal));
            }
            return frequency_signal_amplitude_phase_shift; // return with the resulted singal
        }

        // this function for padding the singal 1 and 2 in case of crros correlation and periodic the first one 
        // the padding according to the size of each one 
        public void padding_periodic()
        {
            // get the number of components of the first and second signal
            int N1 = InputSignal1.Samples.Count;
            int N2 = InputSignal2.Samples.Count;
            if (N1 != N2)
            {
                // here we will pad the signal one with the rest of singal 2 and via verse 
                // the padding with zero length is (N1 - N2 + 1) 
                for (int i = 0; i < N2 - 1; i++)
                    // pad singal one with zeros according to singal 2 
                    InputSignal1.Samples.Add(0);
                for (int i = 0; i < N1 - 1; i++)
                    // pad singal two with zeros according to singal 1
                    InputSignal2.Samples.Add(0);
            }
        }

        public override void Run()
        {
            OutputNonNormalizedCorrelation = new List<float>();
            OutputNormalizedCorrelation = new List<float>();

            //////////////////// AUTO COREELATION SECTION//////////////////////////
            // in case the second singal is null so it is fast auto correlation
            if (InputSignal2 == null)
            {

                // we call this function to get the normalization term in case of auto correlation
                float Normalized = get_normalized_term_auto_correlation();
                // make list with the components of the signal in the frequency domain
                List<ComplexNumber> frequency_domain_signal = new List<ComplexNumber>();
                
                // now we want to convert the signal form time domain to the frequncy domain
                FastFourierTransform FFT = new FastFourierTransform(); // object form fast forourio transform
                FFT.InputTimeDomainSignal = InputSignal1; // get the input of FFT form the time domain signal
                FFT.Run(); // run like test cases to get the signal in the frequency domain  

                // now get the signal components in the frequency domain form the calculated Pahse Shifts and Amplitudes
                frequency_domain_signal = get_siganl_form_amplitude_and_pahse_shift(FFT.OutputFreqDomainSignal.FrequenciesAmplitudes,
                    FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts);

                // now make array of the corrleation components in the frequency domain 
                ComplexNumber[] frequency_domain_correlation = new ComplexNumber[frequency_domain_signal.Count];
                for (int i = 0; i < frequency_domain_signal.Count; i++)
                { 
                    // the correlation in the frequency domain is just the multplication of the two singals
                    ComplexNumber  result = new ComplexNumber(); 
                    // the exnpoential in the frequency domain will be -> cos(x) - jsin(x)
                    // multiply the (real* real) - (imaginary * imaginary)
                    result.Reyal = frequency_domain_signal[i].Reyal * frequency_domain_signal[i].Reyal 
                        - (frequency_domain_signal[i].Imagenary * -1) * frequency_domain_signal[i].Imagenary; 
                    // the imaginary part will be with zero as it is only ordinary multiblication between the component and it self
                    result.Imagenary = 0;
                    // add the result to the array of the correlation components in the frequeny domain
                    frequency_domain_correlation[i] = result;
                } 

                // now we want to get the pahse shifts and amplitudes of the singal in the frequqncy domain 
                // this to convert the corrleated signal form frequency domain back to time domian using IFFT
                Signal correlated_signal_amplitudes_shifts = get_pahse_and_amplitude_from_singal(frequency_domain_correlation); 

                // make object form => Iverese Fast Fouriour Transform
                InverseFastFourierTransform IFFT = new InverseFastFourierTransform();
                // the input in IFFT is the amplitudes and the phase shifts in the frequency domain components 
                IFFT.InputFreqDomainSignal = correlated_signal_amplitudes_shifts;
                IFFT.Run(); // run like in test Casees 

                // loop on the components of the time domain signal (output of the IFFT)
                for (int i = 0; i < IFFT.OutputTimeDomainSignal.Samples.Count; i++)
                {
                    // the first is the non normalized  so we only divide by the number of the components in the output signal in time doamin 
                    OutputNonNormalizedCorrelation.Add(IFFT.OutputTimeDomainSignal.Samples[i] / IFFT.OutputTimeDomainSignal.Samples.Count);
                    // the secind one is the normalzied so we divide also by the normazliation term we calculated 
                    OutputNormalizedCorrelation.Add(IFFT.OutputTimeDomainSignal.Samples[i] / IFFT.OutputTimeDomainSignal.Samples.Count / Normalized);
                }
            }

            // second case is the Fast Crossed Correlation (Two different Singals)
            else 
            {
                // we call this function to make the padding for both singals 1 and 2 
                padding_periodic();

                // call this function to get the normalized correlation term (for the cross correlation)
                float Normalized = get_normalized_term_cross_correlation();


                // make list with the components of the signal in the frequency domain
                List<ComplexNumber> frequency_domain_signal_1 = new List<ComplexNumber>();
                List<ComplexNumber> frequency_domain_signal_2 = new List<ComplexNumber>();

                // now we want to convert the signal form time domain to the frequncy domain
                FastFourierTransform FFT = new FastFourierTransform(); // object form fast forourio transform

                // Get the Amplitudes and Phase Shifts in frequqency domain for each signal (signal 1 and signal 2s)

                /////////////// FIRST SINGAL//////////////////////////////////
                FFT.InputTimeDomainSignal = InputSignal1; // get the input of FFT form the time domain signal
                FFT.Run(); // run like test cases to get the signal in the frequency domain  
                frequency_domain_signal_1 = get_siganl_form_amplitude_and_pahse_shift(FFT.OutputFreqDomainSignal.FrequenciesAmplitudes,
                    FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts);

                /////////////// SECOND SINGAL//////////////////////////////////
                FFT.InputTimeDomainSignal = InputSignal2;
                FFT.Run();
                frequency_domain_signal_2 = get_siganl_form_amplitude_and_pahse_shift(FFT.OutputFreqDomainSignal.FrequenciesAmplitudes,
                    FFT.OutputFreqDomainSignal.FrequenciesPhaseShifts);
                /////////////////////////////////////////////////////////////////////////////////////////////

                // now make array of the corrleation components in the frequency domain 
                // the size now after padding is the same for both singals (any one will be used)
                ComplexNumber[] frequency_domain_correlation = new ComplexNumber[frequency_domain_signal_1.Count]; 

                // loop on the components 
                for (int i = 0; i < frequency_domain_signal_1.Count; i++)
                {
                    // the correlation in the frequency domain is just the multplication of the two singals
                    ComplexNumber result = new ComplexNumber();
                    // the exnpoential in the frequency domain will be -> cos(x) - jsin(x)
                    // multiply the (real 1 * real 2) - (imaginary 1 * imaginary 2) -> this is the real part

                    result.Reyal = frequency_domain_signal_1[i].Reyal * frequency_domain_signal_2[i].Reyal
                        - (-1 * frequency_domain_signal_1[i].Imagenary) * frequency_domain_signal_2[i].Imagenary; 

                    // the imaginary part is the (real 1 * imaginary 2  + real 2 * imaginary 1)
                    result.Imagenary = (frequency_domain_signal_1[i].Reyal * frequency_domain_signal_2[i].Imagenary) 
                        + ((-1 * frequency_domain_signal_1[i].Imagenary) * frequency_domain_signal_2[i].Reyal);
                    // now add the result (frequency domain component to the corrleation array ) in the frequency domain 
                    frequency_domain_correlation[i] = result;
                }

                // now we want to get the pahse shifts and amplitudes of the singal in the frequqncy domain 
                // this to convert the corrleated signal form frequency domain back to time domian using IFFT
                Signal correlated_signal_amplitudes_shifts = get_pahse_and_amplitude_from_singal(frequency_domain_correlation);

                // make object form => Iverese Fast Fouriour Transform
                InverseFastFourierTransform IFFT = new InverseFastFourierTransform();
                // the input in IFFT is the amplitudes and the phase shifts in the frequency domain components 
                IFFT.InputFreqDomainSignal = correlated_signal_amplitudes_shifts;
                IFFT.Run(); // run like in test Casees 

                // loop on the components of the time domain signal (output of the IFFT)
                for (int i = 0; i < IFFT.OutputTimeDomainSignal.Samples.Count; i++)
                {
                    // the first is the non normalized  so we only divide by the number of the components in the output signal in time doamin 
                    OutputNonNormalizedCorrelation.Add(IFFT.OutputTimeDomainSignal.Samples[i] / IFFT.OutputTimeDomainSignal.Samples.Count);
                    // the secind one is the normalzied so we divide also by the normazliation term we calculated 
                    OutputNormalizedCorrelation.Add(IFFT.OutputTimeDomainSignal.Samples[i] / IFFT.OutputTimeDomainSignal.Samples.Count / Normalized);
                }
            }
        }

    }
}