using Emgu.CV;
using Emgu.CV.Structure;
using System;
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


        #region Moorphological operations color

        private static double GetMagnitudeSquared(Bgr color)
        {
            return color.Blue * color.Blue + color.Green * color.Green + color.Red * color.Red;
        }

        public static Image<Bgr, byte> DilateColor(Image<Bgr, byte> inputImage, int h, int w)
        {
            Image<Bgr, byte> resultImage = new Image<Bgr, byte>(inputImage.Size);

            int padH = h / 2;
            int padW = w / 2;

            using (Image<Bgr, byte> paddedImage = ReplicatePadColor(inputImage, padH, padW))
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    for (int x = 0; x < inputImage.Width; x++)
                    {
                        double maxMagnitude = -1;
                        Bgr maxPixel = new Bgr();

                        for (int i = -padH; i <= padH; i++)
                        {
                            for (int j = -padW; j <= padW; j++)
                            {
                                Bgr currentPixel = paddedImage[y + padH + i, x + padW + j];
                                double magnitude = GetMagnitudeSquared(currentPixel);

                                if (magnitude > maxMagnitude)
                                {
                                    maxMagnitude = magnitude;
                                    maxPixel = currentPixel;
                                }
                            }
                        }
                        resultImage[y, x] = maxPixel;
                    }
                }
            }
            return resultImage;
        }

        public static Image<Bgr, byte> ErodeColor(Image<Bgr, byte> inputImage, int h, int w)
        {
            Image<Bgr, byte> resultImage = new Image<Bgr, byte>(inputImage.Size);

            int padH = h / 2;
            int padW = w / 2;

            using (Image<Bgr, byte> paddedImage = ReplicatePadColor(inputImage, padH, padW))
            {
                for (int y = 0; y < inputImage.Height; y++)
                {
                    for (int x = 0; x < inputImage.Width; x++)
                    {
                        double minMagnitude = double.MaxValue;
                        Bgr minPixel = new Bgr();

                        for (int i = -padH; i <= padH; i++)
                        {
                            for (int j = -padW; j <= padW; j++)
                            {
                                Bgr currentPixel = paddedImage[y + padH + i, x + padW + j];
                                double magnitude = GetMagnitudeSquared(currentPixel);

                                if (magnitude < minMagnitude)
                                {
                                    minMagnitude = magnitude;
                                    minPixel = currentPixel;
                                }
                            }
                        }
                        resultImage[y, x] = minPixel;
                    }
                }
            }
            return resultImage;
        }

        public static Image<Bgr, byte> OpeningColor(Image<Bgr, byte> inputImage, int h, int w)
        {
            Image<Bgr, byte> eroded = ErodeColor(inputImage, h, w);
            Image<Bgr, byte> result = DilateColor(eroded, h, w);
            eroded.Dispose();
            return result;
        }

        public static Image<Bgr, byte> ClosingColor(Image<Bgr, byte> inputImage, int h, int w)
        {
            Image<Bgr, byte> dilated = DilateColor(inputImage, h, w);
            Image<Bgr, byte> result = ErodeColor(dilated, h, w);
            dilated.Dispose();
            return result;
        }

        public static Image<Bgr, byte> ReplicatePadColor(Image<Bgr, byte> inputImage, int halfHeight, int halfWidth)
        {
            int H = inputImage.Height;
            int W = inputImage.Width;
            int paddedH = H + 2 * halfHeight;
            int paddedW = W + 2 * halfWidth;
            Image<Bgr, byte> paddedImage = new Image<Bgr, byte>(paddedW, paddedH);

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    paddedImage[y + halfHeight, x + halfWidth] = inputImage[y, x];

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < halfWidth; x++)
                    paddedImage[y + halfHeight, x] = inputImage[y, 0];

                for (int x = 0; x < halfWidth; x++)
                    paddedImage[y + halfHeight, W + halfWidth + x] = inputImage[y, W - 1];
            }

            for (int x = 0; x < paddedW; x++)
            {
                for (int y = 0; y < halfHeight; y++)
                    paddedImage[y, x] = paddedImage[halfHeight, x];

                for (int y = 0; y < halfHeight; y++)
                    paddedImage[H + halfHeight + y, x] = paddedImage[H + halfHeight - 1, x];
            }

            return paddedImage;
        }
    
}

    #endregion

}