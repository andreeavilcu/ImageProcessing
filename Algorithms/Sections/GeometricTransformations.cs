
using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

namespace Algorithms.Sections
{
    public class GeometricTransformations
    { 
        private static double LinearInterpolation(double t, double f0, double f1)
        {
            double frac_t = t - (int)t;
            return frac_t * f1 + (1 - frac_t) * f0;
        }

        private static double BilinearInterpolation(double tx, double ty, double[] f)
        {
            double p0 = LinearInterpolation(tx, f[0], f[1]);
            double p1 = LinearInterpolation(tx, f[2], f[3]);

            return LinearInterpolation(ty, p0, p1);
        }


        public static Image<Gray,byte> ScaleBilinear(Image<Gray, byte> inputImage, double sx, double sy)
        {
            int newWidth = (int)(inputImage.Width * sx);
            int newHeight = (int)(inputImage.Height * sy);

            Image<Gray, byte> resultImage = new Image<Gray, byte>(newWidth, newHeight);
        
            for (int y = 0; y< newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    double xc = x / sx;
                    double yc = y / sy;

                    int x0 = (int)xc;
                    int y0 = (int)yc;

                    double fracX = xc - x0;
                    double fracY = yc - y0;

                    if (x0 >= 0 && x0 < inputImage.Width - 1 && y0 >= 0 && y0 < inputImage.Height - 1)
                    {
                        double[] f = new double[4];
                        f[0] = inputImage.Data[y0, x0, 0];
                        f[1] = inputImage.Data[y0, x0 + 1, 0];
                        f[2] = inputImage.Data[y0 + 1, x0, 0];
                        f[3] = inputImage.Data[y0 + 1, x0 + 1, 0];

                        double val = BilinearInterpolation(fracX, fracY, f);
                        resultImage.Data[y, x, 0] = Utils.ClipPixel(val);
                    }
                    else
                    {
                        if (x0 < inputImage.Width && y0 < inputImage.Height)
                            resultImage.Data[y, x, 0] = inputImage.Data[y0, x0, 0];
                    }
                }
            }
            return resultImage;
        }


        private static double CubicInterpolation(double t, double fm1, double f0, double f1, double f2)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            double a = -t3 + 2 * t2 - t;
            double b = 3 * t3 - 5 * t2 + 2;
            double c = -3 * t3 + 4 * t2 + t;
            double d = t3 - t2;

            return 0.5 * (a * fm1 + b * f0 + c * f1 + d * f2);
        }

        private static double BicubicInterpolation(double tx, double ty, double[,] f)
        {
            double[] arr = new double[4];

            arr[0] = CubicInterpolation(tx, f[0, 0], f[0, 1], f[0, 2], f[0, 3]);
            arr[1] = CubicInterpolation(tx, f[1, 0], f[1, 1], f[1, 2], f[1, 3]);
            arr[2] = CubicInterpolation(tx, f[2, 0], f[2, 1], f[2, 2], f[2, 3]);
            arr[3] = CubicInterpolation(tx, f[3, 0], f[3, 1], f[3, 2], f[3, 3]);

            return CubicInterpolation(ty, arr[0], arr[1], arr[2], arr[3]);
        }

        public static Image<Gray, byte> ScaleBicubic(Image<Gray, byte> inputImage, double sx, double sy)
        {
            int newWidth = (int)(inputImage.Width * sx);
            int newHeight = (int)(inputImage.Height * sy);
            Image<Gray, byte> resultImage = new Image<Gray, byte>(newWidth, newHeight);

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    double xc = x / sx;
                    double yc = y / sy;

                    int x0 = (int)xc;
                    int y0 = (int)yc;

                    double fracX = xc - x0;
                    double fracY = yc - y0;

               
                    if (x0 >= 1 && x0 < inputImage.Width - 2 && y0 >= 1 && y0 < inputImage.Height - 2)
                    {
                        double[,] f = new double[4, 4];

                        for (int i = -1; i <= 2; i++)
                        {
                            for (int j = -1; j <= 2; j++)
                            {                  
                                f[i + 1, j + 1] = inputImage.Data[y0 + i, x0 + j, 0];
                            }
                        }


                        double val = BicubicInterpolation(fracX, fracY, f);

                        resultImage.Data[y, x, 0] = Utils.ClipPixel(val);
                    }
                    else
                    {
                        if (x0 < inputImage.Width && y0 < inputImage.Height)
                            resultImage.Data[y, x, 0] = inputImage.Data[y0, x0, 0];
                    }
                }
            }

            return resultImage;
        }
    }
    
}