using System;
using Microsoft.Xna.Framework;

namespace GameGraphicsLib
{
    [Serializable]
    public class DrawParam
    {
        public DrawParam(string objectName, string drawableName, Vector2 position, DrawnType drawType)
        {
            ObjectName = objectName;
            DrawableName = drawableName;
            Position = position;
            DrawType = drawType;

        }

        public string ObjectName { get; private set; }
        public string DrawableName { get; private set; }
        public Vector2 Position { get; private set; }
        public DrawnType DrawType { get; private set; }
    }
}