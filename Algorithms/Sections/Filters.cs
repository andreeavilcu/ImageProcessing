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
        #endregion
    }
}