using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;


namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

       
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
       


        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage,
        double threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] > threshold)
                        result.Data[y, x, 0] = 255;
                    else
                        result.Data[y, x, 0] = 0;
                }
            }
            return result;
        }
        #endregion

        #region Crop
        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, Point p1, Point p2, double zoom)
        {
            if (zoom == 0) zoom = 1;

            var x1 = (int)(Math.Max(0, Math.Min(p1.X, p2.X)) / zoom);
            var y1 = (int)(Math.Max(0, Math.Min(p1.Y, p2.Y)) / zoom);
            var width = (int)(Math.Abs(p2.X - p1.X) / zoom);
            var height = (int)(Math.Abs(p2.Y - p1.Y) / zoom);

            if (width <= 1 || height <= 1)
                throw new ArgumentException("Invalid crop region: the selected area is too small.");

            int imageWidth = inputImage.Width;
            int imageHeight = inputImage.Height;

            if (x1 + width > imageWidth || y1 + height > imageHeight)
                throw new ArgumentException("Invalid crop region: exceeds image boundaries.");

            var cropRect = new Rectangle(x1, y1, width, height);

            cropRect.Width = Math.Min(cropRect.Width, imageWidth - cropRect.X);
            cropRect.Height = Math.Min(cropRect.Height, imageHeight - cropRect.Y);

            if (cropRect.Width <= 1 || cropRect.Height <= 1)
                throw new ArgumentException("Invalid crop region: the area is too small or outside the image bounds.");

            return Crop(inputImage, cropRect);
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Point p1, Point p2, double zoom)
        {
            if (zoom == 0) zoom = 1;

            var x1 = (int)(Math.Max(0, Math.Min(p1.X, p2.X)) / zoom);
            var y1 = (int)(Math.Max(0, Math.Min(p1.Y, p2.Y)) / zoom);
            var width = (int)(Math.Abs(p2.X - p1.X) / zoom);
            var height = (int)(Math.Abs(p2.Y - p1.Y) / zoom);

            if (width <= 1 || height <= 1)
                throw new ArgumentException("Invalid crop region: the selected area is too small.");

            int imageWidth = inputImage.Width;
            int imageHeight = inputImage.Height;

            if (x1 + width > imageWidth || y1 + height > imageHeight)
                throw new ArgumentException("Invalid crop region: exceeds image boundaries.");

            var cropRect = new Rectangle(x1, y1, width, height);

            cropRect.Width = Math.Min(cropRect.Width, imageWidth - cropRect.X);
            cropRect.Height = Math.Min(cropRect.Height, imageHeight - cropRect.Y);

            if (cropRect.Width <= 1 || cropRect.Height <= 1)
                throw new ArgumentException("Invalid crop region: the area is too small or outside the image bounds.");

            return Crop(inputImage, cropRect);
        }

        public static Image<Gray, byte> Crop(Image<Gray, byte> inputImage, Rectangle cropRect)
        {
            if (cropRect.Width <= 0 || cropRect.Height <= 0)
                throw new ArgumentException("Invalid crop region.");

            Image<Gray, byte> croppedImage = inputImage.Copy(cropRect);
            return croppedImage;
        }

        public static Image<Bgr, byte> Crop(Image<Bgr, byte> inputImage, Rectangle cropRect)
        {
            if (cropRect.Width <= 0 || cropRect.Height <= 0)
                throw new ArgumentException("Invalid crop region.");

            Image<Bgr, byte> croppedImage = inputImage.Copy(cropRect);
            return croppedImage;
        }
        #endregion

        #region Statistics
        public static (double mean, double standardDeviation) CalculateStatistics(Image<Gray, byte> croppedImage)
        {
            var data = croppedImage.Data;
            int width = croppedImage.Width;
            int height = croppedImage.Height;
            int totalPixels = width * height;

            double sum = 0;
            double sumOfSquares = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte pixelValue = data[y, x, 0];
                    sum += pixelValue;
                    sumOfSquares += pixelValue * pixelValue;
                }
            }

            double mean = sum / totalPixels;

            double variance = (sumOfSquares / totalPixels) - (mean * mean);
            double standardDeviation = Math.Sqrt(variance);

            return (mean, standardDeviation);
        }
        #endregion

        #region Mirror

        public static Image<Gray, byte> Mirror(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, inputImage.Width - x - 1, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Mirror(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, inputImage.Width - x - 1, 0] = inputImage.Data[y, x, 0]; // Blue
                    result.Data[y, inputImage.Width - x - 1, 1] = inputImage.Data[y, x, 1]; // Green
                    result.Data[y, inputImage.Width - x - 1, 2] = inputImage.Data[y, x, 2]; // Red
                }
            }
            return result;
        }

        #endregion

        #region Rotate

        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[x, inputImage.Height - y - 1, 0] = inputImage.Data[y, x, 0]; // Blue
                    result.Data[x, inputImage.Height - y - 1, 1] = inputImage.Data[y, x, 1]; // Green
                    result.Data[x, inputImage.Height - y - 1, 2] = inputImage.Data[y, x, 2]; // Red
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[inputImage.Width - x - 1, y, 0] = inputImage.Data[y, x, 0]; // Blue
                    result.Data[inputImage.Width - x - 1, y, 1] = inputImage.Data[y, x, 1]; // Green
                    result.Data[inputImage.Width - x - 1, y, 2] = inputImage.Data[y, x, 2]; // Red
                }
            }
            return result;
        }

        #endregion
    }
}