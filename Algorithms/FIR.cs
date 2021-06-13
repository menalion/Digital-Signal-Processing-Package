using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FIR : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public FILTER_TYPES InputFilterType { get; set; }
        public float InputFS { get; set; }
        public float? InputCutOffFrequency { get; set; }
        public float? InputF1 { get; set; }
        public float? InputF2 { get; set; }
        public float InputStopBandAttenuation { get; set; }
        public float InputTransitionBand { get; set; }
        public Signal OutputHn { get; set; }
        public Signal OutputYn { get; set; }



        // First : (Low or high) pass filter . . . . . .
        public void LOW_OR_HIGH_PASS_FILTER(int window_len, ref List<float?> HD)
        {
            float? hd_equal_zero = 0, hf_not_equal_zero = 0;
            int start, end;
            float? fc = 0; 
            if (InputFilterType == FILTER_TYPES.LOW) // Low Pass Filter 
            {
                fc  = (float?)((InputCutOffFrequency + (InputTransitionBand / 2.0)) / InputFS);
                //HD(0) = 2 * fc -->> (in lowpass)
                hd_equal_zero = 2 * fc;
                hf_not_equal_zero = 2;  // 2 * fc
            }
            else // High Pass Filter
            {
                fc = (float?)((InputCutOffFrequency - (InputTransitionBand / 2.0)) / InputFS);
                //HD(0) = 1 - (2 * fc) -->> (in highpass)
                hd_equal_zero = (1 - (2 * fc));
                hf_not_equal_zero = -2; // -2 * fc 
            }
            start = -(int)window_len / 2;
            end = (int)window_len / 2;
            for (int n = start; n <= end; n++)
            {
                //sin(0)=0 ,,, can't divide by zero , don't take zero value while looping .. 
                // ( {2 or -2} * fc * (2 * PI * loop_counter * fc) ) /  (2 * loop_counter * PI * fc)
                if (n != 0)
                {
                    HD.Add((float?)(((hf_not_equal_zero * fc) * (float?)Math.Sin((double)fc * 2 * n * Math.PI)) / (2 * n * Math.PI * fc)));
                }
                else HD.Add(hd_equal_zero);
            }
        }


        // Second :(Pass or Stop) band filter . . . . . .
        public void PASS_OR_STOP_FILTER(int window_len, ref List<float?> HD)
        {
            // First We Calculate the F1 and F2 
            float? f1 = (float?)((InputF1 - (InputTransitionBand / 2)) / InputFS);
            float? f2 = (float?)((InputF2 + (InputTransitionBand / 2)) / InputFS);
            int start, end;
            float? HD_OF_ZERO = 0; 

            if (FILTER_TYPES.BAND_STOP == InputFilterType)
            {
                f1 = (float?)((InputF1 +(InputTransitionBand / 2)) / InputFS);
                f2 = (float?)((InputF2 - (InputTransitionBand / 2)) / InputFS);
            }
            // Band Stop Will Be -> 1 - 2 * (f2 - f1) 
            if (InputFilterType == FILTER_TYPES.BAND_STOP)
                HD_OF_ZERO = 1 - (2 * (f2 - f1));
            // Band Pass Will Be ->2 * (f2 - f1)
            else
                HD_OF_ZERO = (2 * (f2 - f1));

            start = -(int)window_len / 2;
            end = (int)window_len / 2;
            for (int n = start; n <= end; n++)
                if (n != 0)
                {
                    //bandpass :  ( (2 * f2 * (f2_first / f2_second))  -  (2 * f1 * (f1_first / f1_second)) )
                    //band_stop : ( (2 * f1 * (f1_first / f1_second))  -  (2 * f2 * (f2_first / f2_second)) )
                    float? f1_first = (float?)(Math.Sin(2 * Math.PI * (double)f1 * n));
                    float? f1_second = (float?)(n * 2 * Math.PI * f1);
                    float? f2_first = (float?)Math.Sin(2 * Math.PI * (double)f2 * n);
                    float? f2_second = (float?)(n * 2 * Math.PI * f2);
                    if (InputFilterType == FILTER_TYPES.BAND_STOP)
                        HD.Add((float?)((2 * f1 * (f1_first / f1_second)) - (2 * f2 * (f2_first / f2_second))));
                    else
                        HD.Add((float?)((2 * f2 * (f2_first / f2_second)) - (2 * f1 * (f1_first / f1_second))));
                }
                else HD.Add((float?)HD_OF_ZERO);
        }


        // This Method For PreParing the Window Array 
        public void CREATE_WINDOW(ref int window_len, ref List<float> array_window)
        {

            // First :  (Recatngular ) -> Stop Band Attenuation <= 21 && Transition width = 0.9 / N
            // In This Case the Values of the Window All Will be (1)
            if (InputStopBandAttenuation <= 21)
            {
                window_len = (int)((0.9 * InputFS) / InputTransitionBand);
                // if the Length is even So We Will Make It Odd To Be Valid ++  
                if (window_len % 2 == 0) window_len++;

                int start = -(int)window_len / 2, end = (int)window_len / 2;
                for (int n = start; n <= end; n++)
                    array_window.Add(1);
            }


            // Second : (Hanging ) -> (Stop Band Attenuation > 21 && <= 44) && (Transition width = 3.1 / N)
            // In This Case the Values of the Window All Will be ( 0.5 + (0.5 * cos((2 * PI * n) / N ) )
            else if (InputStopBandAttenuation > 21 && InputStopBandAttenuation <= 44)
            {
                float res;
                window_len = (int)((3.1 * InputFS) / InputTransitionBand);
                // if the Length is even So We Will Make It Odd To Be Valid ++  
                if (window_len % 2 == 0) window_len++;

                int start = -(int)window_len / 2, end = (int)window_len / 2;
                for (int n = start; n <= end; n++)
                {
                    res = (float)0.5 + (float)(0.5 * Math.Cos((2 * Math.PI * n) / window_len));
                    array_window.Add(res);
                }
            }


            // third : (hamming ) -> (Stop Band Attenuation > 44 && <= 53) && (Transition width = 3.3 / N)
            // In This Case the Values of the Window All Will be ( 0.54 + (0.46 * cos ((2 * PI * n) / N)) )
            else if (InputStopBandAttenuation > 44 && InputStopBandAttenuation <= 53)
            {
                float res;
                window_len = (int)((3.3 * InputFS) / InputTransitionBand);
                // if the Length is even So We Will Make It Odd To Be Valid ++  
                if (window_len % 2 == 0) window_len++;
                int start = -(int)window_len / 2, end = (int)window_len / 2;
                for (int i = start; i <= end; i++)
                {
                    res = (float)0.54 + (float)(0.46 * Math.Cos((2 * Math.PI * i) / window_len));
                    array_window.Add(res);
                }
            }


            // fourth : (Blackman ) -> (Stop Band Attenuation > 53 && <= 74) && (Transition width = 5.5 / N)
            // In This Case the Values of the Window All Will be (0.42 + (0.5 * cos((2 * PI * n) / (N - 1))) + (0.08 * cos((4 * PI * n) / (N - 1))) )
            else if (InputStopBandAttenuation > 53 && InputStopBandAttenuation <= 74)
            {
                float w1, w2;
                window_len = (int)(5.5 * InputFS / InputTransitionBand);
                // if the Length is even So We Will Make It Odd To Be Valid ++  
                if (window_len % 2 == 0) window_len++;

                int start = -(int)window_len / 2, end = (int)window_len / 2;
                for (int i = start; i <= end; i++)
                {
                    w1 = (float)(0.5 * Math.Cos((2 * Math.PI * i) / (window_len - 1)));
                    w2 = (float)(0.08 * Math.Cos((4 * Math.PI * i) / (window_len - 1)));
                    array_window.Add(((float)0.42 + w1 + w2));
                }
            }
        }


        // This Method For PreParing the HD(n) Array (List) Based On the Type Of the filter We Will Use 
        public void CREATE_HDn(int length_of_window, ref List<float?> HD)
        {
            //(Low or high) pass filter
            if (InputFilterType == FILTER_TYPES.LOW || InputFilterType == FILTER_TYPES.HIGH)
                LOW_OR_HIGH_PASS_FILTER(length_of_window, ref HD);

            // PASS_OR_STOP_FILTER
            else if (InputFilterType == FILTER_TYPES.BAND_STOP || InputFilterType == FILTER_TYPES.BAND_PASS)
                PASS_OR_STOP_FILTER(length_of_window, ref HD);
        }
        public override void Run()
        {
            OutputHn = new Signal(new List<float>(), new List<int>(), false);
            OutputYn = new Signal(new List<float>(), new List<int>(), false);

            int window_len = 0;
            List<float?> HD = new List<float?>();
            List<float> array_window = new List<float>();

            // In Low Pass Filter We need To Convert From KHZ To HZ 
            if (InputFilterType == FILTER_TYPES.LOW)
            {
                InputFS /= (float)1000.0;
                InputCutOffFrequency /= (float?)1000.0;
                InputTransitionBand /= (float)1000.0;
            }
            // Create Window
            CREATE_WINDOW(ref window_len, ref array_window);
            // Create HD(n) 
            CREATE_HDn(window_len, ref HD);

            int start = -(int)window_len / 2, end = (int)window_len / 2;
            // Copy the Samples Indeices
            for (int i = start; i <= end; i++)
                OutputHn.SamplesIndices.Add(i);
            // Get Samples Values
            for (int i = 0; i < HD.Count; i++)
                OutputHn.Samples.Add((float)(HD[i] * array_window[i]));

            // Doing the Convlultion on the Two Signals and Get the final OutPutYn
            DirectConvolution DC = new DirectConvolution();
            DC.InputSignal1 = OutputHn;
            DC.InputSignal2 = InputTimeDomainSignal;
            DC.Run();
            OutputYn = DC.OutputConvolvedSignal;
        }
    }
}
