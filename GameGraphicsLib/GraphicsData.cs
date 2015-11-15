using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace GameGraphicsLib
{
    [Serializable]
    public class GraphicsData
    {
        public GraphicsData()
        {
            Animations = new List<Animation>();
        }
        public GraphicsData(Dictionary<string, Animation> animationList, Dictionary<string, Texture2D> textures )
        {
            Animations = new List<Animation>();
            Textures = new List<BinaryTexture>();
            foreach (Animation cloneAnimation in animationList.Select(animation => animation.Value.CloneAnimation(animation.Key)))
            {
                Animations.Add(cloneAnimation);
            }
            foreach (BinaryTexture newTexture in textures.Select(pair => new BinaryTexture(pair.Value)))
            {
                Textures.Add(newTexture);
            }
        }

        public List<Animation> Animations { get; private set; }
        public List<BinaryTexture> Textures { get; private set; }
    }
}