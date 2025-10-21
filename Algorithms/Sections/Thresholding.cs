using Emgu.CV;
using Emgu.CV.Structure;
using System;


using static Algorithms.Sections.PointwiseOperations;
using Tools = Algorithms.Tools.Tools; 

namespace Algorithms.Sections
{
    public class Thresholding
    {
       
        public static int MinErrThreshold(Image<Gray, byte> inputImage)
        {
            
            float[] p = ComputeNormalizedHistogram(inputImage);
            int T = 0;
            double minErr = double.MaxValue;

           
            const double epsilon = 1e-10;

           
            for (int t = 1; t <= 254; t++)
            {
                

                double P1 = 0;
                double sum_k_p_1 = 0;
                for (int k = 0; k <= t; k++)
                {
                    P1 += p[k];
                    sum_k_p_1 += k * p[k];
                }

                double P2 = 1.0 - P1;

                
                if (P1 < epsilon || P2 < epsilon)
                    continue;

                
                double mu1 = sum_k_p_1 / P1;

                
                double sigma1_sq = 0;
                for (int k = 0; k <= t; k++)
                {
                    sigma1_sq += Math.Pow(k - mu1, 2) * p[k];
                }
                sigma1_sq /= P1;

                
                double mu2 = 0;
                for (int k = t + 1; k < 256; k++)
                {
                    mu2 += k * p[k];
                }
                mu2 /= P2;

                double sigma2_sq = 0;
                for (int k = t + 1; k < 256; k++)
                {
                    sigma2_sq += Math.Pow(k - mu2, 2) * p[k];
                }
                sigma2_sq /= P2;

                
                double log_s1 = Math.Log(Math.Max(sigma1_sq, epsilon));
                double log_s2 = Math.Log(Math.Max(sigma2_sq, epsilon));
                double log_P1 = Math.Log(P1);
                double log_P2 = Math.Log(P2);

                double Err = 1.0 + P1 * log_s1 + P2 * log_s2 - 2.0 * P1 * log_P1 - 2.0 * P2 * log_P2;

               
                if (Err < minErr)
                {
                    minErr = Err;
                    T = t;
                }
            }

            return T;
        }

        public static IImage MinErrThreshold2(IImage inputImage)
        {
            if (inputImage == null)
                return null;

            Image<Gray, byte> grayImage;
            bool temporaryGrayImage = false;

            if (inputImage is Image<Gray, byte> gray)
            {
                grayImage = gray;
            }
            else if (inputImage is Image<Bgr, byte> color)
            {
                
                grayImage = Tools.Tools.Convert(color);
                temporaryGrayImage = true;
            }
            else
            {
                return (IImage)inputImage.Clone();
            }

            

            int threshold = MinErrThreshold(grayImage);

           
            Image<Gray, byte> result = Tools.Tools.Binary(grayImage, threshold);

            if (temporaryGrayImage)
            {
                
                grayImage.Dispose();
            }

            return result;
        }
    }
}