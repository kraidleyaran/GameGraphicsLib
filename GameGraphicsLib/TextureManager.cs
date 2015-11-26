using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GameGraphicsLib
{
    public class TextureManager
    {
        public TextureManager()
        {
            Textures = new Dictionary<string, Texture2D>();
            PixelTextures = new Dictionary<string, Texture2D>();
        }

        public Dictionary<string, Texture2D> Textures { get; private set; }
        public Dictionary<string, Texture2D> PixelTextures { get; private set; }
        
        public static Texture2D ConvertDataToTexture(BinaryTexture data, GraphicsDevice graphics)
        {
            Texture2D returnTexture = new Texture2D(graphics,data.Width,data.Height);
            returnTexture.SetData(data.ColorData, 0, data.ColorData.Length);
            returnTexture.Name = data.Name;
            return returnTexture;
        }

        public void ClearAllTextures()
        {   
            ClearPixelTextures();
            ClearAnimationTextures();
        }

        public void ClearPixelTextures()
        {
            PixelTextures.Clear();
            PixelTextures = new Dictionary<string, Texture2D>();
        }

        public void ClearAnimationTextures()
        {
            Textures.Clear();
            Textures = new Dictionary<string, Texture2D>();
        }
    }
}