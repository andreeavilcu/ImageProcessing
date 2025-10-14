using Emgu.CV;
using Emgu.CV.Structure;
using System;


namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        #region  Contrast and Brightness 
     

        public static Image<Gray, byte> Brightness(Image<Gray, byte> inputImage, double alpha, double beta)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                LUT[i] = (byte)Math.Min(255, Math.Max(0, alpha * i + beta) + 0.5f);
            }

            for (int y = 0; y < inputImage.Height; y++)
                for (int x = 0; x < inputImage.Width; x++)
                   
                    {
                        result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                    }


            return result;
        }

        public static Image<Bgr, byte> Brightness(Image<Bgr, byte> inputImage, double alpha, double beta)
        {
            
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double newValue = alpha * i + beta;
                LUT[i] = (byte)Math.Min(255, Math.Max(0, alpha * i + beta) + 0.5f);
            }

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = LUT[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = LUT[inputImage.Data[y, x, 2]];

                }
            }

            return result;
        }



        public static Image<Bgr, byte> GammaOperator(Image<Bgr, byte> inputImage, double gamma)
        {
            if (gamma <= 0)
            {
                throw new ArgumentException("Gamma trebuie să fie o valoare pozitivă.");
            }

            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            
            double c = 255.0 / Math.Pow(255.0, gamma);

           
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                LUT[i] = (byte)Math.Min(255, Math.Max(0, c * Math.Pow(i, gamma)));
            }

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    
                   result.Data[y, x, 0] = LUT[inputImage.Data[y, x, 0]];
                   result.Data[y, x, 1] = LUT[inputImage.Data[y, x, 1]];
                   result.Data[y, x, 2] = LUT[inputImage.Data[y, x, 2]];

                }
            }

            return result;
        }



        public static Image<Gray, byte> GammaOperator(Image<Gray, byte> inputImage, double gamma)
        {
            if (gamma <= 0)
            {
                throw new ArgumentException("Gamma trebuie sa fie o valoare pozitiva.");
            }

            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);


            double c = 255.0 / Math.Pow(255.0, gamma);


            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                LUT[i] = (byte)Math.Min(255, Math.Max(0, c * Math.Pow(i, gamma)));
            }


            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte intensity = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = LUT[intensity];
                }
            }

            return result;
        }


        #endregion

        #region Normalized histogram
       

        public static float[] ComputeNormalizedHistogram(Image<Gray, byte> inputImage)
        {
            float[] histogram = new float[256];

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte intensity = inputImage.Data[y, x, 0];
                    histogram[intensity]++;
                }
            }

            float totalPixels = inputImage.Width * inputImage.Height;

           
            for (int i = 0; i < 256; i++)
            {
                histogram[i] /= totalPixels;
            }

            return histogram;
        }


        public static float[][] ComputeNormalizedHistogram(Image<Bgr, byte> inputImage)
        {
            float[][] histograms = new float[3][];
            histograms[0] = new float[256]; 
            histograms[1] = new float[256]; 
            histograms[2] = new float[256]; 

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    histograms[0][inputImage.Data[y, x, 0]]++;
                    histograms[1][inputImage.Data[y, x, 1]]++;
                    histograms[2][inputImage.Data[y, x, 2]]++;
                }
            }

            float totalPixels = inputImage.Width * inputImage.Height;

            for (int c = 0; c < 3; c++)
            {
                for (int i = 0; i < 256; i++)
                {
                    histograms[c][i] /= totalPixels;
                }
            }

            return histograms;
        }

        #endregion

        

    }
}