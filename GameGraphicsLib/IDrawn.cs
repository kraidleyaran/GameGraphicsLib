using GameGraphicsLib.DrawableShapes;

namespace GameGraphicsLib
{
    public interface IDrawn
    {
        string Name { get; set; }
        float PositionX { get; set; }
        float PositionY { get; set; }
        DrawnType DrawnType { get; set; }
    }
}