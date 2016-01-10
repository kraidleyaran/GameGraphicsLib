using GameGraphicsLib.DrawableShapes;

namespace GameGraphicsLib
{
    public interface IDrawn
    {
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        DrawnType DrawnType { get; set; }
    }
}