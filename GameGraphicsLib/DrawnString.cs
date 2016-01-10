using GameGraphicsLib.DrawableShapes;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GameGraphicsLib
{
    public class DrawnString : IDrawn, IColorable
    {
        private const DrawnType _shape = DrawnType.String;
        public string Name { get; set; }
        public string Value { get; set; }
        public Color Color { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public DrawnType DrawnType { get { return _shape; } set { } }
        public float Thickness { get; set; }
        public SpriteFont Font { get; set; }

        public DrawnString Clone(string name)
        {
            return new DrawnString
            {
                Name = name,
                Value = Value,
                Color = Color,
                X = X,
                Y = Y,
                Thickness = Thickness,
                Font = Font
            };
        }
    }
}