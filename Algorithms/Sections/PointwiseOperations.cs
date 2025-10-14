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
                        byte intensity = inputImage.Data[y, x, 0];
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


                    byte intensity = inputImage.Data[y, x, 0];
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
                    for (int cIdx = 0; cIdx < 3; cIdx++) 
                    {
                        byte intensity = inputImage.Data[y, x, cIdx];
                        result.Data[y, x, cIdx] = LUT[intensity];
                    }
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

    }
}