namespace GameGraphicsLib.DrawableShapes
{
    public interface IDrawnShape : IColorable, IDrawn
    {
        float Thickness { get; set; }

        ShapeType Shape { get; set; }
    }
}