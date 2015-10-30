using System;
using System.Collections.Generic;
using System.Linq;

namespace GameGraphicsLib
{
    [Serializable]
    public class GraphicsData
    {
        public GraphicsData()
        {
            Animations = new List<Animation>();
        }
        public GraphicsData(Dictionary<string, Animation> animationList, GameGraphics graphics)
        {
            Animations = new List<Animation>();
            foreach (Animation cloneAnimation in animationList.Select(animation => animation.Value.CloneAnimation(animation.Key, graphics)))
            {
                Animations.Add(cloneAnimation);
            }
        }

        public List<Animation> Animations { get; private set; }
    }
}