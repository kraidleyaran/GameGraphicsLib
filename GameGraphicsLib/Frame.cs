using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib
{
    [Serializable]
    public class Frame
    {
        public Frame(int width, int height, GameRectangle textureSource)
        {
            Width = width;
            Height = height;
            TextureSource = textureSource;
        }
        public float Height { get; private set; }
        public float Width { get; private set; }
        public GameRectangle TextureSource { get; private set; } 
    }
}