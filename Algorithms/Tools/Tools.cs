using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Algorithms.Tools
{
    public class Tools
    {

        #region BinaryImage
        public static Image<Gray, byte> BinaryImage(Image<Gray, byte> inputImage, byte threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.Data[y, x, 0] >= threshold)
                    {
                        result.Data[y, x, 0] = 255;
                    }
                    else
                    {
                        result.Data[y, x, 0] = 0;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Mirroring
        public static Image<Gray, byte> MirrorImage(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y, inputImage.Width - 1 - x, 0];
                }
            }
            return result;
        }

        #endregion

        #region RotateImage90
        public static Image<Gray, byte> RotateImage90(Image<Gray, byte> inputImage, bool clockWise)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);
            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    if (clockWise)
                    {
                        result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    }
                    else
                    {
                        result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    }
                }
            }
            return result;
        }


        #endregion CropImage 
        public static Image<Gray, byte> CropImage(Image<Gray, byte> src, Rectangle rect)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(rect.Size);
            for (int y = 0; y < rect.Height; ++y)
            {
                for (int x = 0; x < rect.Width; ++x)
                {
                    result.Data[y, x, 0] = src.Data[y + rect.Y, x + rect.X, 0];
                }
            }
            return result;
        }

        
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

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion
    }
}