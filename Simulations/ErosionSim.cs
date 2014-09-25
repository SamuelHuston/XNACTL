using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNACTL
{
    public class ErosionSim
    {
        public Field Altitude;
        public bool[,] ComputationMask;
        FluidSim PrincipleErosionSource;
        StaticFieldRenderer Renderer;

        float Scale;

        public void Initialize(Field Terrain, FluidSim WaterSim, float scale)
        {
            Altitude = Terrain;
            PrincipleErosionSource = WaterSim;
            Renderer = new StaticFieldRenderer(Terrain);
            Scale = scale;
            ComputationMask = new bool[Width, Height];
        }

        public int Width
        {
            get
            {
                return Altitude.Width;
            }
        }

        public int Height
        {
            get
            {
                return Altitude.Height;
            }
        }

        public void Update()
        {
            //do erosion code here using the PrincipleErosionSource
        }

        public void Draw()
        {
            Renderer.Draw(Scale);
        }
    }
}
