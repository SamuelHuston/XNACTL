using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNACTL
{
    public class PlantSim
    {
        ErosionSim TerrainSim;
        FluidSim WaterSim;
        
        int Width;
        int Height;
        float Scale;

        public void Initialize(ErosionSim terrainSim, FluidSim waterSim, float scale)
        {
            TerrainSim = terrainSim;
            WaterSim = waterSim;
            Width = terrainSim.Width;
            Height = terrainSim.Height;
            Scale = scale;
        }

        public void Update()
        {
        }

        public void Draw()
        {
        }
    }
}
