using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace GameGraphicsLib.DrawableShapes
{
    public class DrawnRectangle : IDrawnShape
    {
        private const DrawnType _DrawnType = DrawnType.Shape;
        private const ShapeType _Shape = ShapeType.Rectangle;
        
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
        public float Thickness { get; set; }
        public DrawnType DrawnType { get { return _DrawnType; } set { } }
        public ShapeType Shape { get { return _Shape;} set { } }

        public DrawnRectangle Clone(string name)
        {
            return new DrawnRectangle
            {
                Name = name,
                PositionX = PositionX,
                PositionY = PositionY,
                Size = Size,
                Color = Color,
                Thickness = Thickness
            };
        }
    }
}