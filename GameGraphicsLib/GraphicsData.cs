using System;
using System.Collections.Generic;
using System.Linq;
using GameGraphicsLib.DrawableShapes;
using Microsoft.Xna.Framework.Graphics;

namespace GameGraphicsLib
{
    [Serializable]
    public class GraphicsData
    {
        public GraphicsData()
        {
            DrawnObjects = new List<IDrawn>();
            AnimationTextures = new List<BinaryTexture>();
            PixelTextures = new List<BinaryTexture>();
        }
        public GraphicsData(Dictionary<string, IDrawn> drawnObjects, Dictionary<string, Texture2D> animationTextures, Dictionary<string, Texture2D> pixelTextures)
        {
            DrawnObjects = new List<IDrawn>();
            AnimationTextures = new List<BinaryTexture>();
            PixelTextures = new List<BinaryTexture>();

            foreach (KeyValuePair<string, IDrawn> drawnObject in drawnObjects)
            {
                switch (drawnObject.Value.DrawnType)
                {
                    case DrawnType.Animation:
                        Animation animation = (Animation) drawnObject.Value;
                        DrawnObjects.Add(animation.CloneAnimation(animation.Name));
                        break;
                    case DrawnType.Shape:
                        IDrawnShape shape = (IDrawnShape) drawnObject.Value;
                        switch (shape.Shape)
                        {
                            case ShapeType.Line:
                                DrawnLine line = (DrawnLine) shape;
                                DrawnObjects.Add(line.Clone(line.Name));
                                break;
                            case ShapeType.Rectangle:
                                DrawnRectangle rectangle = (DrawnRectangle) shape;
                                DrawnObjects.Add(rectangle.Clone(rectangle.Name));
                                break;
                        }
                        break;
                    case DrawnType.String:
                        DrawnString drawString = (DrawnString) drawnObject.Value;
                        DrawnObjects.Add(drawString.Clone(drawString.Name));
                        break;
                }
            }
            foreach (BinaryTexture newTexture in animationTextures.Select(pair => new BinaryTexture(pair.Value)))
            {
                AnimationTextures.Add(newTexture);
            }
            foreach (BinaryTexture pixelTexture in pixelTextures.Select(pair => new BinaryTexture(pair.Value)))
            {
                PixelTextures.Add(pixelTexture);
            }
        }

        public List<IDrawn> DrawnObjects { get; private set; }
        public List<BinaryTexture> AnimationTextures { get; private set; }
        public List<BinaryTexture> PixelTextures { get; private set; }
    }
}