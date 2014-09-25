using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNACTL
{
    public class TerrainGenerator
    {
        public static Field CreateCave(int width, int height, int depth, double wavelength, double fractionalThreshold, double altitudinalThreshold)
        {
            Field terrain = FieldTools.GenerateFractalNoise(width, height, depth, wavelength, fractionalThreshold);
            Field lowerSlice = FieldTools.Slice(terrain, 0, altitudinalThreshold);
            Field upperSlice = FieldTools.Slice(terrain, altitudinalThreshold, 1);
            lowerSlice = FieldTools.ReshapeSquare(lowerSlice);
            lowerSlice = FieldTools.ReshapeSquare(lowerSlice);
            lowerSlice = FieldTools.ReshapeSquare(lowerSlice);
            terrain = FieldTools.Sum(lowerSlice, upperSlice);

            terrain = FieldTools.CreateCentralDepression(terrain);

            return terrain;
        }
    }
}
