using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameGraphicsLib
{
    [Serializable]
    public class BinaryTexture
    {
        public BinaryTexture(Texture2D texture)
        {
            ConvertTextureToBinary(texture);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] ColorData { get; private set; }

        private void ConvertTextureToBinary(Texture2D texture)
        {
            Width = texture.Width;
            Height = texture.Height;
            ColorData = new byte[4 * Width * Height];
            texture.GetData(ColorData, 0, ColorData.Length);
        }
    }
}