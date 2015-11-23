using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib.DrawableShapes
{
    [Serializable]
    public class DrawnLine : IDrawnShape
    {
        private const DrawnType _DrawnType = DrawnType.Shape;
        private const ShapeType _ShapeType = ShapeType.Line;
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }
        public float Thickness { get; set; }
        public DrawnType DrawnType { get { return _DrawnType; } set { } }
        public ShapeType Shape { get { return _ShapeType; } set { } }
        public Color Color { get; set; }

        public DrawnLine Clone(string name)
        {
            return new DrawnLine
            {
                Name = name,
                PositionX = PositionX,
                PositionY = PositionY,
                EndX = EndX,
                EndY = EndY,
                Thickness = Thickness,
                Color = Color
            };
        }
    }
}