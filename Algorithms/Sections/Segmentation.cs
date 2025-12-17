using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using Algorithms.Utilities;

namespace Algorithms.Sections
{
    public class Segmentation
    {
        public static (Image<Gray, byte> accumulatorImage, List<(float rho, float theta)> lines) HoughTransform(Image<Gray, byte> inputImage, int binaryThreshold)
        {
            float[,] sobelX = new float[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            float[,] sobelY = new float[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

          

            Image<Gray, float> gradX = Filters.ApplyFloatFilter(inputImage, sobelX);
            Image<Gray, float> gradY = Filters.ApplyFloatFilter(inputImage, sobelY);

            int width = inputImage.Width;
            int height = inputImage.Height;
            Image<Gray, byte> edges = new Image<Gray, byte>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double mag = Math.Sqrt(Math.Pow(gradX.Data[y, x, 0], 2) + Math.Pow(gradY.Data[y, x, 0], 2));
                    edges.Data[y, x, 0] = mag > binaryThreshold ? (byte)255 : (byte)0;
                }
            }

            int diagonal = (int)Math.Ceiling(Math.Sqrt(width * width + height * height));
            int rhoCount =  diagonal ;
            int thetaCount = 271;
            int[,] accumulator = new int[rhoCount, thetaCount];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (edges.Data[y, x, 0] == 255)
                    {
                        for (int t = 0; t < thetaCount; t++)
                        {
                            double thetaRad = (t - 90) * Math.PI / 180.0;
                            double rho = x * Math.Cos(thetaRad) + y * Math.Sin(thetaRad);
                            int rhoIndex = (int)Math.Round(rho) + diagonal;

                            if (rhoIndex >= 0 && rhoIndex < rhoCount)
                            {
                                accumulator[rhoIndex, t]++;
                            }
                        }
                    }
                }
            }

            Image<Gray, byte> visualAccumulator = new Image<Gray, byte>(thetaCount, rhoCount);
            int maxVotes = 0;
            for (int r = 0; r < rhoCount; r++)
            {
                for (int t = 0; t < thetaCount; t++)
                {
                    if (accumulator[r, t] > maxVotes)
                        maxVotes = accumulator[r, t];
                }
            }

            for (int r = 0; r < rhoCount; r++)
            {
                for (int t = 0; t < thetaCount; t++)
                {
                    if (maxVotes > 0)
                        visualAccumulator.Data[r, t, 0] = (byte)(accumulator[r, t] * 255 / maxVotes);
                }
            }

            List<(float, float)> lines = new List<(float, float)>();
            int voteThreshold = (int)(0.5 * maxVotes);

            for (int r = 1; r < rhoCount - 1; r++)
            {
                for (int t = 1; t < thetaCount - 1; t++)
                {
                    int val = accumulator[r, t];
                    if (val > voteThreshold)
                    {
                        bool isMax = true;
                        for (int dr = -1; dr <= 1; dr++)
                        {
                            for (int dt = -1; dt <= 1; dt++)
                            {
                                if (accumulator[r + dr, t + dt] > val)
                                {
                                    isMax = false;
                                    break;
                                }
                            }
                            if (!isMax) break;
                        }

                        if (isMax)
                        {
                            float theta = (t - 90);
                            float rho = r - diagonal;
                            lines.Add((rho, theta));
                        }
                    }
                }
            }

            gradX.Dispose();
            gradY.Dispose();
            edges.Dispose();

            return (visualAccumulator, lines);
        }
    }
}