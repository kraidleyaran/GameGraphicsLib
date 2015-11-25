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
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] ColorData { get; private set; }

        private void ConvertTextureToBinary(Texture2D texture)
        {
            Width = texture.Width;
            Height = texture.Height;
            ColorData = new byte[4 * Width * Height];
            texture.GetData(ColorData, 0, ColorData.Length);
            Name = texture.Name;
        }

        public static Texture2D RemoveTransparentColor(Texture2D texture, Color color)
        {
            int width = texture.Width;
            int height = texture.Height;
            Color[] colorData = new Color[width * height];
            texture.GetData(colorData);
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int pos = x*width + y;
                    Color currentPixel = colorData[pos];
                    if (currentPixel == color)
                    {
                        colorData[pos] = new Color(0, 0, 0, 0);
                    }
                }
            }

            texture.SetData(colorData, 0, colorData.Length);
            return texture;
        }
    }
}