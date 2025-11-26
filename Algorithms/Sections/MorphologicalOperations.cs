using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static Image<Gray, byte> Dilate(Image<Gray, byte> inputImage, int h, int w, int T, int type)
        {
            
            Image<Gray, byte> binaryImage = Tools.Tools.Binary(inputImage, T);
            Image<Gray, byte> resultImage = binaryImage.Copy();

            int padH = h / 2;
            int padW = w / 2;

            
            Image<Gray, byte> paddedImage = Filters.ReplicatePad(binaryImage, padH, padW);

           
            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool conditionMet = false;

                    
                    for (int i = -padH; i <= padH; i++)
                    {
                        for (int j = -padW; j <= padW; j++)
                        {
                            byte pixelVal = paddedImage.Data[y + padH + i, x + padW + j, 0];

                            if (type == 1) 
                            {
                                if (pixelVal == 255) { conditionMet = true; break; }
                            }
                            else 
                            {
                                if (pixelVal == 0) { conditionMet = true; break; }
                            }
                        }
                        if (conditionMet) break;
                    }

                    if (type == 1)
                        resultImage.Data[y, x, 0] = conditionMet ? (byte)255 : (byte)0;
                    else
                        resultImage.Data[y, x, 0] = conditionMet ? (byte)0 : (byte)255;
                }
            }
            return resultImage;
        }

        public static Image<Gray, byte> Erode(Image<Gray, byte> inputImage, int h, int w, int T, int type)
        {
            Image<Gray, byte> binaryImage = Tools.Tools.Binary(inputImage, T);
            Image<Gray, byte> resultImage = binaryImage.Copy();

            int padH = h / 2;
            int padW = w / 2;

            Image<Gray, byte> paddedImage = Filters.ReplicatePad(binaryImage, padH, padW);

            for (int y = 0; y < binaryImage.Height; y++)
            {
                for (int x = 0; x < binaryImage.Width; x++)
                {
                    bool conditionMet = false;

                    for (int i = -padH; i <= padH; i++)
                    {
                        for (int j = -padW; j <= padW; j++)
                        {
                            byte pixelVal = paddedImage.Data[y + padH + i, x + padW + j, 0];

                            if (type == 1) 
                            {
                               
                                if (pixelVal == 0) { conditionMet = true; break; }
                            }
                            else 
                            {
                              
                                if (pixelVal == 255) { conditionMet = true; break; }
                            }
                        }
                        if (conditionMet) break;
                    }

                    if (type == 1)
                        resultImage.Data[y, x, 0] = conditionMet ? (byte)0 : (byte)255;
                    else
                        resultImage.Data[y, x, 0] = conditionMet ? (byte)255 : (byte)0;
                }
            }
            return resultImage;
        }


        public static Image<Gray, byte> Opening(Image<Gray, byte> inputImage, int h, int w, int T, int type)
        {
            Image<Gray, byte> eroded = Erode(inputImage, h, w, T, type);
            Image<Gray, byte> result = Dilate(eroded, h, w, T, type);
            return result;
        }

        public static Image<Gray, byte> Closing(Image<Gray, byte> inputImage, int h, int w, int T, int type)
        {
           
            Image<Gray, byte> dilated = Dilate(inputImage, h, w, T, type);
            Image<Gray, byte> result = Erode(dilated, h, w, T, type);

            return result;
        }

    } 
}