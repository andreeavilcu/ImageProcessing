using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Algorithms.Sections
{
    public class Filters
    {
        private static Image<Gray, byte> ReplicatePad(Image<Gray, byte> inputImage, int halfHeight, int halfWidth)
        {
            int H = inputImage.Height;
            int W = inputImage.Width;
            int paddedH = H + 2 * halfHeight;
            int PaddedW = W + 2 * halfWidth;
            Image<Gray, byte> paddedImage = new Image<Gray, byte>(PaddedW, paddedH);

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    paddedImage.Data[y + halfHeight, x + halfWidth, 0] = inputImage.Data[y, x, 0];

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < halfWidth; x++)
                    paddedImage.Data[y + halfHeight, x, 0] = inputImage.Data[y, 0, 0];

                for (int x = 0; x < halfWidth; x++)
                    paddedImage.Data[y + halfHeight, W + halfWidth + x, 0] = inputImage.Data[y, W - 1, 0];
            }

            for (int x = 0; x < PaddedW; x++)
            {

                for (int y = 0; y < halfHeight; y++)
                    paddedImage.Data[y, x, 0] = paddedImage.Data[halfHeight, x, 0];

                for (int y = 0; y < halfHeight; y++)
                    paddedImage.Data[H + halfHeight + y, x, 0] = paddedImage.Data[H + halfHeight - 1, x, 0];
            }

            return paddedImage;

        }
        #region Filtering
        public static Image<Gray, byte> applyFilter (Image<Gray, byte> image, float[,] filter)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(image.Size);

            int h = filter.GetLength(0);
            int w = filter.GetLength(1);
            int halfH = h / 2;
            int halfW = w / 2;

            using (Image<Gray, byte> paddedImage = ReplicatePad(image, halfH, halfW))
            {
                for (int y = h / 2; y < image.Height - h / 2 - 1; ++y)
                {
                    for (int x = w / 2; x < image.Width - w / 2 - 1; ++x)
                    {
                        double sum = 0;
                        for (int i = -h / 2; i <= h / 2; i++)
                        {
                            for (int j = -w / 2; j <= w / 2; j++)
                            {
                                sum += filter[i + h / 2, j + w / 2] * image.Data[y + i, x + j, 0];
                            }
                        }

                        result.Data[y, x, 0] = Utilities.Utils.ClipPixel(sum);
                    }
                }
            }
            return result;
        }
        public static Image<Gray, byte> ApplySeparableFilter(Image<Gray, byte> inputImage, float[] v)
        {
            if (inputImage == null || v == null)
            {
                return null;
            }

            int l = v.Length;
            int halfL = l / 2; 

            
            Image<Gray, float> tempImage = new Image<Gray, float>(inputImage.Size);

            
            int paddedWidth = inputImage.Width + 2 * halfL;
            Image<Gray, byte> paddedHorizontal = new Image<Gray, byte>(paddedWidth, inputImage.Height);

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    paddedHorizontal.Data[y, x + halfL, 0] = inputImage.Data[y, x, 0];
                }
            }

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                
                for (int x = 0; x < halfL; x++)
                {
                    paddedHorizontal.Data[y, x, 0] = inputImage.Data[y, 0, 0];
                }
                
                for (int x = 0; x < halfL; x++)
                {
                    paddedHorizontal.Data[y, inputImage.Width + halfL + x, 0] = inputImage.Data[y, inputImage.Width - 1, 0];
                }
            }

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    double sum = 0;
                    for (int k = -halfL; k <= halfL; k++)
                    {
                        
                        sum += v[k + halfL] * paddedHorizontal.Data[y, x + halfL + k, 0];
                    }
                    tempImage.Data[y, x, 0] = (float)sum;
                }
            }

            
            Image<Gray, byte> finalImage = new Image<Gray, byte>(inputImage.Size);

            int paddedHeight = inputImage.Height + 2 * halfL;
            Image<Gray, float> paddedVertical = new Image<Gray, float>(inputImage.Width, paddedHeight);

            
            for (int x = 0; x < inputImage.Width; x++)
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    paddedVertical.Data[y + halfL, x, 0] = tempImage.Data[y, x, 0];
                }
            }

            
            for (int x = 0; x < inputImage.Width; x++)
            {
                
                for (int y = 0; y < halfL; y++)
                {
                    paddedVertical.Data[y, x, 0] = tempImage.Data[0, x, 0];
                }
               
                for (int y = 0; y < halfL; y++)
                {
                    paddedVertical.Data[inputImage.Height + halfL + y, x, 0] = tempImage.Data[inputImage.Height - 1, x, 0];
                }
            }

            
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    double sum = 0;
                    for (int k = -halfL; k <= halfL; k++)
                    {
                        
                        sum += v[k + halfL] * paddedVertical.Data[y + halfL + k, x, 0];
                    }
                    
                    finalImage.Data[y, x, 0] = Utils.ClipPixel(sum);
                }
            }

            
            paddedHorizontal.Dispose();
            tempImage.Dispose();
            paddedVertical.Dispose();

            return finalImage;
        }
        
       private static byte Median(float[] h , int l)
        {
            int k = 0;
            float s = 0;
            int n = (l * l) / 2;
            
            while(k <= 255 && s + h[k]<=n)
            {
                s += h[k];
                k += 1;

            }

            return (byte)k;
        }


        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> inputImage, int l)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            using (Image<Gray, byte> paddedImage = ReplicatePad(inputImage, l/2 , l/ 2))
            {
                float[] H = new float[256];
                int r = l / 2;

                for (int y = r; y <= paddedImage.Height - r - 1; y++)
                {
                    for (int x = r; x <= paddedImage.Width - r - 1; x++)
                    {
                        if (x == r)
                        {
                            for(int m = 0; m < 256; m++)
                            {
                                H[m] = 0;
                            }

                            for (int i = -r; i <= r; i++)
                            {
                                for (int j = -r; j <= r; j++)
                                    H[paddedImage.Data[y + i, x + j, 0]]++;
                            }
                        }
                        else
                        {
                            for (int k = -r; k <= r; k++)
                            {
                                H[paddedImage.Data[y + k, x - r - 1, 0]]--;
                                H[paddedImage.Data[y + k, x + r, 0]]++;
                            }
                        }
                        
                        result.Data[y - r, x - r  , 0] = Median(H, l);

                    }
                }

                return result;

                
            }   
            
        }
        #endregion
    }
}