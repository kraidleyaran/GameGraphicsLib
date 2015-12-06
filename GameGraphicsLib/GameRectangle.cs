using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib
{
    [Serializable]
    public class GameRectangle
    {
        public GameRectangle(int x, int y, int width,int height)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public static Rectangle ConvertToRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, width,height);
        }

        public static Rectangle ConvertToRectangle(GameRectangle rectangle)
        {
            return ConvertToRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}