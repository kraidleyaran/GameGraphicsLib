using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib
{
    [Serializable]
    public class DrawParam
    {
        public DrawParam(string objectName, string animation, Vector2 postiion)
        {
            ObjectName = objectName;
            Animation = animation;
            Position = postiion;
        }

        public string ObjectName { get; private set; }
        public string Animation { get; private set; }
        public Vector2 Position { get; private set; }
    }
}