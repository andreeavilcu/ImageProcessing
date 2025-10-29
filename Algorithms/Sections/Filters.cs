using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        #region Filtering
        public static Image<Gray, byte> applyFilter (Image<Gray, byte> image, float[,] filter)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(image.Size);

            int h = filter.GetLength(0);
            int w = filter.GetLength(1);

            for(int y =  h/2; y<image.Height - h/2 - 1; ++y)
            {
                for(int x = w/2; x < image.Width -  w/2 - 1; ++x)
                {
                    double sum = 0;
                    for (int i = -h/2; i<= h/2; i++)
                    {
                        for(int j =  -w/2; j<= w/2; j++)
                        {
                            sum += filter[i + h / 2, j + w / 2] * image.Data[y + i, x + j, 0];
                        }
                    }

                    result.Data[y, x, 0] = Utilities.Utils.ClipPixel(sum);
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
        #endregion
    }
}