using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNACTL
{
    public class FluidSim
    {
        Field Terrain;

        SpriteFont Font;
        Texture2D Tile;
        SpriteBatch Batch = XNACore.Batch;

        Field TotalAltitude;
        Field CurrentWater;
        Field NextWater;
        double[,] DeltaWater;

        double FlowFraction = 1;

        double StartingWater = 0;

        StaticFieldRenderer TerrainRenderer;
        DynamicFieldRenderer FluidRenderer;

        Vector2[,] AltitudeGradient;
        Vector2[,] FluidGradient;
        bool[,] ComputationMask;
        Line[,] Lines;

        float Scale;

        double TotalWater = 0;

        Vector2 LocalGradient;
        double LocalFreeWater;
        Vector2 LocalTotalDisplacement;
        Point LocalDisplacementPoint;
        Vector2 LocalOffset;
        double[,] LocalTemp;
        Point LocalMousePoint;

        double LocalXGrad;
        double LocalYGrad;

        int Width;
        int Height;

        MouseState MState;

        public Boolean RenderTotalGradient = true;
        public Boolean RenderFluid = true;

        public void Initialize(ErosionSim TerrainSim, float scale)
        {
            Scale = scale;

            LocalMousePoint = Point.Zero;

            Terrain = TerrainSim.Altitude;
            ComputationMask = TerrainSim.ComputationMask;
            Width = Terrain.Width;
            Height = Terrain.Height;

            TotalAltitude = new Field(Width, Height);
            CurrentWater = new Field(Width, Height);
            CurrentWater.Add(StartingWater);
            NextWater = new Field(Width, Height);
            DeltaWater = new double[Width, Height];

            TerrainRenderer = new StaticFieldRenderer(Terrain);
            FluidRenderer = new DynamicFieldRenderer(CurrentWater);

            AltitudeGradient = new Vector2[Width, Height];
            Tile = XNACore.Content.Load<Texture2D>("tile");
            Font = XNACore.Content.Load<SpriteFont>("Font");
            Lines = new Line[Width, Height];

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Lines[x, y] = new Line(x, y, 0, 0, 0);

            CurrentWater.Add(StartingWater);

            ResetStates();
            ComputeGradient();
        }

        private void ResetStates()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    TotalAltitude.Data[x, y] = CurrentWater.Data[x, y] + Terrain.Data[x, y];
        }

        private void ResetWater()
        {
            TotalWater = 0;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    NextWater.Data[x, y] = 0;
                    TotalWater += CurrentWater.Data[x, y];
                }
        }

        private void ComputeGradient()
        {
            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                {
                    LocalXGrad = 3 * TotalAltitude.Data[x - 1, y - 1] + 10 * TotalAltitude.Data[x - 1, y] + 3 * TotalAltitude.Data[x - 1, y + 1]
                        - 3 * TotalAltitude.Data[x + 1, y - 1] - 10 * TotalAltitude.Data[x + 1, y] - 3 * TotalAltitude.Data[x + 1, y + 1];
                    LocalYGrad = 3 * TotalAltitude.Data[x - 1, y - 1] + 10 * TotalAltitude.Data[x, y - 1] + 3 * TotalAltitude.Data[x + 1, y - 1]
                        - 3 * TotalAltitude.Data[x - 1, y + 1] - 10 * TotalAltitude.Data[x, y + 1] - 3 * TotalAltitude.Data[x + 1, y + 1];

                    AltitudeGradient[x, y].X = (float)LocalXGrad;
                    AltitudeGradient[x, y].Y = (float)LocalYGrad;

                    if (RenderTotalGradient)
                        Lines[x, y].Initialize(x, y, (float)LocalXGrad, (float)LocalYGrad, (int)Scale);
                }
        }

        #region Compute Free Water
        private double ComputeTotalFreeWater(int y, int x)
        {
            double freeWater = 0;

            if (CurrentWater.Data[x, y] > 0)
            {
                freeWater = ComputeDiagonalFreeWater(y, x, freeWater);
                freeWater = ComputeCardinalFreeWater(y, x, freeWater);
                freeWater *= FlowFraction;
            }

            return freeWater;
        }
        private double ComputeCardinalFreeWater(int y, int x, double freeWater)
        {
            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x, y - 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x, y - 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x, y - 1]);

            //y = 0
            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x - 1, y])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x - 1, y])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x - 1, y]);

            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x + 1, y])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x + 1, y])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x + 1, y]);

            //y = +1
            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x, y + 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x, y + 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x, y + 1]);
            return freeWater;
        }
        private double ComputeDiagonalFreeWater(int y, int x, double freeWater)
        {
            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x - 1, y - 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x - 1, y - 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x - 1, y - 1]);

            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x + 1, y - 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x + 1, y - 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x + 1, y - 1]);

            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x - 1, y + 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x - 1, y + 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x - 1, y + 1]);

            if (TotalAltitude.Data[x, y] > TotalAltitude.Data[x + 1, y + 1])
                if (Terrain.Data[x, y] > TotalAltitude.Data[x + 1, y + 1])
                    freeWater = CurrentWater.Data[x, y];
                else
                    freeWater = Math.Max(freeWater, TotalAltitude.Data[x, y] - TotalAltitude.Data[x + 1, y + 1]);
            return freeWater;
        }
        #endregion

        public void Update()
        {
            MState = Mouse.GetState();
            LocalMousePoint.X = (int)((float)MState.X / Scale);
            LocalMousePoint.Y = (int)((float)MState.Y / Scale);

            if (LocalMousePoint.X > 0 && LocalMousePoint.X < Width && LocalMousePoint.Y > 0 && LocalMousePoint.Y < Height && MState.LeftButton == ButtonState.Pressed)
                CurrentWater.Data[LocalMousePoint.X, LocalMousePoint.Y] += 0.1;

            ResetStates();
            ComputeGradient();

            ResetWater();

            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                {
                    LocalGradient = AltitudeGradient[x, y];
                    LocalFreeWater = ComputeTotalFreeWater(y, x);

                    if (CurrentWater.Data[x, y] > 0)
                    {
                        LocalTotalDisplacement = new Vector2(x + LocalGradient.X, y + LocalGradient.Y);
                        LocalDisplacementPoint.X = (int)Math.Floor(LocalTotalDisplacement.X);
                        LocalDisplacementPoint.Y = (int)Math.Floor(LocalTotalDisplacement.Y);
                        LocalOffset.X = LocalTotalDisplacement.X - LocalDisplacementPoint.X;
                        LocalOffset.Y = LocalTotalDisplacement.Y - LocalDisplacementPoint.Y;

                        NextWater.Data[x, y] += CurrentWater.Data[x, y] - LocalFreeWater / FlowFraction;

                        if (LocalDisplacementPoint.X > 1 && LocalDisplacementPoint.X < Width - 1 && LocalDisplacementPoint.Y > 1 && LocalDisplacementPoint.Y < Height - 1)
                        {
                            NextWater.Data[LocalDisplacementPoint.X, LocalDisplacementPoint.Y] += (1.0 - LocalOffset.X) * (1.0 - LocalOffset.Y) * LocalFreeWater * FlowFraction;
                            NextWater.Data[LocalDisplacementPoint.X + 1, LocalDisplacementPoint.Y] += LocalOffset.X * (1.0 - LocalOffset.Y) * LocalFreeWater * FlowFraction;
                            NextWater.Data[LocalDisplacementPoint.X, LocalDisplacementPoint.Y + 1] += (1.0 - LocalOffset.X) * LocalOffset.Y * LocalFreeWater * FlowFraction;
                            NextWater.Data[LocalDisplacementPoint.X + 1, LocalDisplacementPoint.Y + 1] += LocalOffset.X * LocalOffset.Y * LocalFreeWater * FlowFraction;
                        }
                        else
                            NextWater.Data[x, y] = LocalFreeWater * (1.0 - FlowFraction);

                        DeltaWater[x, y] = NextWater.Data[x, y] - CurrentWater.Data[x, y];
                    }
                }

            LocalTemp = CurrentWater.Data;
            CurrentWater.Data = NextWater.Data;
            NextWater.Data = LocalTemp;
        }

        public void Draw()
        {
            if (RenderFluid)
                FluidRenderer.Draw(Scale);

            if (RenderTotalGradient)
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        Lines[x, y].Draw(Tile);

            if (LocalMousePoint.X > 0 && LocalMousePoint.X < Width && LocalMousePoint.Y > 0 && LocalMousePoint.Y < Height)
            {
                Batch.DrawString(Font, "Total Altitude " + TotalAltitude.Data[LocalMousePoint.X, LocalMousePoint.Y], new Vector2(0, 0), Color.Red);
                Batch.DrawString(Font, "Water " + CurrentWater.Data[LocalMousePoint.X, LocalMousePoint.Y], new Vector2(0, 20), Color.Red);
                Batch.DrawString(Font, "Gradient " + AltitudeGradient[LocalMousePoint.X, LocalMousePoint.Y], new Vector2(0, 40), Color.Red);
            }

            Batch.DrawString(Font, "Total Water " + TotalWater, new Vector2(0, 70), Color.Red);
        }
    }
}
