using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib
{
    [Serializable]
    public class Frame
    {
        public Frame(GameRectangle textureSource)
        {
            TextureSource = textureSource;
        }
        public GameRectangle TextureSource { get; private set; } 
    }
}