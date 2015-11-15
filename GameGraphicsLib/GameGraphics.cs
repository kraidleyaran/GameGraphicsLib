using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Xml.Schema;
using GameGraphicsLib.CacheObjects;
using GameGraphicsLib.DrawableShapes;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameGraphicsLib
{
    [Serializable]
    public class GameGraphics
    {
        //private ContentManager ContentManager;
        private Dictionary<string, Animation> animationList = new Dictionary<string, Animation>();
        private Dictionary<string, IDrawnShape> shapeList = new Dictionary<string, IDrawnShape>();
        private Dictionary<string, DrawnString> stringList= new Dictionary<string, DrawnString>();

        private Dictionary<string, IDrawn> drawList = new Dictionary<string, IDrawn>();

        private Dictionary<string, DrawnType> nameList = new Dictionary<string, DrawnType>();

        

        private DrawCache drawCache = new DrawCache();

        public TextureManager textureManager = new TextureManager();

        public GameGraphics(Game game)
        {   
            GraphicsManager = new GraphicsDeviceManager(game);
            //ContentManager = game.Content;   
        }

        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDeviceManager GraphicsManager { get; private set; }
        public Texture2D LoadTexture(string location)
        {
            using (Stream fileStream = new FileStream(location, FileMode.Open))
            {
                Texture2D returnTexture = Texture2D.FromStream(GraphicsManager.GraphicsDevice, fileStream);
                fileStream.Close();
                return returnTexture;
            }
        }

        public bool AddTexture(string name, Texture2D texture)
        {
            if (textureManager.Textures.ContainsKey(name))
            {
                return false;
            }
            textureManager.Textures.Add(name, texture);
            return true;
        }

        public bool RemoveTexture(string name)
        {
            return textureManager.Textures.Remove(name);
        }

        public bool AddDrawable(IDrawn drawable)
        {
            switch (drawable.DrawnType)
            {
                case DrawnType.Animation:
                    Animation animation = (Animation) drawable;
                    if (nameList.ContainsKey(animation.Name)) return false;
                    animationList.Add(animation.Name, animation);
                    return true;
                case DrawnType.Shape:
                    IDrawnShape shape = (IDrawnShape) drawable;
                    if (nameList.ContainsKey(shape.Name)) return false;
                    shapeList.Add(shape.Name, shape );
                    textureManager.PixelTextures.Add(shape.Name, CreatePixel(shape.Color));
                    return true;                    
                case DrawnType.String:
                    DrawnString drawString = (DrawnString) drawable;
                    if (nameList.ContainsKey(drawString.Name)) return false;
                    stringList.Add(drawString.Name, drawString);
                    return true;
            }
            return false;
        }

        public bool RemoveDrawable(string name)
        {
            if (!nameList.ContainsKey(name)) return false;
            nameList.Remove(name);
            switch (nameList[name])
            {
                case DrawnType.Animation:
                    return animationList.Remove(name);
                case DrawnType.Shape:
                    return shapeList.Remove(name);
                case DrawnType.String:
                    return stringList.Remove(name);
            }

            return false;
        }

        public bool AddToDrawList(DrawParam param)
        {
            if (!nameList.ContainsKey(param.DrawableName) || drawList.ContainsKey(param.ObjectName)) return false;
            bool objectCacheExists = drawCache.Cache.ContainsKey(param.ObjectName);
            if (objectCacheExists)
            {
                ObjectCache cache = drawCache.Cache[param.ObjectName];
                if (cache.Cache.ContainsKey(param.DrawableName))
                {
                    IDrawn drawObject = cache.Cache[param.DrawableName];
                    drawObject.PositionX = param.Position.X;
                    drawObject.PositionY = param.Position.Y;
                    switch (drawObject.DrawnType)
                    {
                        case DrawnType.Animation:
                            Animation animation = (Animation) drawObject;
                            animation.Reset();
                            drawList.Add(param.ObjectName, animation);
                            return true;
                        default:
                            drawList.Add(param.ObjectName, drawObject);
                            return true;
                    }
                }
            }
            else
            {
                drawCache.Cache.Add(param.ObjectName, new ObjectCache());
            }
            ObjectCache objectCache = drawCache.Cache[param.ObjectName];
            switch (param.DrawType)
            {
                case DrawnType.Animation:
                    if (!animationList.ContainsKey(param.DrawableName)) return false;
                    Animation animation = animationList[param.DrawableName].CloneAnimation(param.DrawableName);
                    animation.PositionX = param.Position.X;
                    animation.PositionY = param.Position.Y;
                    drawList.Add(param.ObjectName, animation);
                    objectCache.Cache.Add(param.DrawableName, animation);
                    return true;
                case DrawnType.Shape:
                    if (!shapeList.ContainsKey(param.DrawableName)) return false;
                    IDrawnShape shape = shapeList[param.DrawableName];
                    switch (shape.Shape)
                    {
                        case ShapeType.Line:
                            DrawnLine line = (DrawnLine) shape;
                            DrawnLine drawLine = line.Clone(line.Name);
                            drawLine.PositionX = param.Position.X;
                            drawLine.PositionY = param.Position.Y;
                            drawList.Add(line.Name, drawLine);
                            objectCache.Cache.Add(line.Name, drawLine);
                            return true;
                        case ShapeType.Rectangle:
                            DrawnRectangle rectangle = (DrawnRectangle) shape;
                            DrawnRectangle drawRectangle = rectangle.Clone(rectangle.Name);
                            drawRectangle.PositionX = param.Position.X;
                            drawRectangle.PositionY = param.Position.Y;
                            drawList.Add(rectangle.Name, drawRectangle);
                            objectCache.Cache.Add(rectangle.Name, drawRectangle);
                            return true;
                    }
                    return true;
                case DrawnType.String:
                    if (!stringList.ContainsKey(param.DrawableName)) return false;
                    DrawnString drawString = stringList[param.DrawableName].Clone(param.DrawableName);
                    drawString.PositionX = param.Position.X;
                    drawString.PositionY = param.Position.Y;
                    drawList.Add(param.ObjectName, drawString);
                    objectCache.Cache.Add(drawString.Name, drawString);
                    return true;
            }

            return false;
        }

        public bool UpdateDrawList(DrawParam param)
        {
            if (!drawList.ContainsKey(param.ObjectName)) return false;
            if (drawCache.Cache.ContainsKey(param.ObjectName))
            {
                ObjectCache cache = drawCache.Cache[param.ObjectName];
                if (cache.Cache.ContainsKey(param.DrawableName))
                {
                    IDrawn drawableObject = cache.Cache[param.DrawableName];
                    drawableObject.PositionX = param.Position.X;
                    drawableObject.PositionY = param.Position.Y;
                    switch (drawableObject.DrawnType)
                    {
                        case DrawnType.Animation:
                            Animation animation = (Animation) drawableObject;
                            animation.Reset();
                            drawList[param.ObjectName] = animation;
                            return true;
                        default:
                            drawList[param.ObjectName] = drawableObject;
                            return true;
                    }
                }
            }
            else
            {
                drawCache.Cache.Add(param.ObjectName, new ObjectCache());
            }
            ObjectCache objCache = drawCache.Cache[param.ObjectName];
            switch (param.DrawType)
            {
                case DrawnType.Animation:
                    if (!animationList.ContainsKey(param.DrawableName)) return false;
                    Animation animation = animationList[param.DrawableName].CloneAnimation(param.DrawableName);
                    animation.Reset();
                    animation.PositionX = param.Position.X;
                    animation.PositionY = param.Position.Y;
                    drawList.Add(param.ObjectName, animation);
                    objCache.Cache.Add(animation.Name, animation);
                    break;
                case DrawnType.Shape:
                    if (!shapeList.ContainsKey(param.DrawableName)) return false;
                    IDrawnShape shape = shapeList[param.DrawableName];
                    switch (shape.Shape)
                    {
                        case ShapeType.Line:
                            DrawnLine line = (DrawnLine) shape;
                            DrawnLine drawLine = line.Clone(line.Name);
                            drawLine.PositionX = param.Position.X;
                            drawLine.PositionY = param.Position.Y;
                            drawList.Add(param.ObjectName, drawLine);
                            objCache.Cache.Add(drawLine.Name, drawLine);
                            return true;
                        case ShapeType.Rectangle:
                            DrawnRectangle rectangle = (DrawnRectangle)shape;
                            DrawnRectangle drawRectangle = rectangle.Clone(rectangle.Name);
                            drawRectangle.PositionX = param.Position.X;
                            drawRectangle.PositionY = param.Position.Y;
                            drawList.Add(param.ObjectName, drawRectangle);
                            objCache.Cache.Add(drawRectangle.Name, drawRectangle);
                            return true;

                    }
                    break;
                case DrawnType.String:
                    if (!stringList.ContainsKey(param.DrawableName)) return false;
                    DrawnString drawString = stringList[param.DrawableName].Clone(param.DrawableName);
                    drawString.PositionX = param.Position.X;
                    drawString.PositionY = param.Position.Y;
                    drawList.Add(param.ObjectName, drawString);
                    objCache.Cache.Add(drawString.Name, drawString);
                    return true;
            }
            return false;
        }

        public bool UpdateDrawPosition(DrawParam param)
        {
            if (!drawList.ContainsKey(param.ObjectName)) return false;
            IDrawn drawObject = drawList[param.ObjectName];
            drawObject.PositionX = param.Position.X;
            drawObject.PositionY = param.Position.Y;
            drawList[param.ObjectName] = drawObject;
            return true;
        }
        public bool RemoveFromDrawList(string objectName)
        {
            return drawList.Remove(objectName);
        }

        public Animation GetDrawAnimation(string objectName)
        {
            if (!drawList.ContainsKey(objectName)) throw new Exception("Animation does not exist");
            Animation animation = (Animation) drawList[objectName];
            if (animation == null ) throw new Exception("Animation is null");
            return animation;
        }

        public DrawnString GetDrawString(string objectName)
        {
            if (!drawList.ContainsKey(objectName)) throw new Exception("String " + objectName + " does not exist");
            DrawnString drawString = (DrawnString) drawList[objectName];
            return drawString;
        }

        public IDrawnShape GetDrawShape(string shapeName)
        {
            if (!drawList.ContainsKey(shapeName)) throw new Exception("Shape " + shapeName + " does not exist");
            IDrawnShape shape = (IDrawnShape) drawList[shapeName];
            return shape;
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach (KeyValuePair<string, IDrawn> drawable in drawList.ToList())
            {
                switch (drawable.Value.DrawnType)
                {
                    case DrawnType.Animation:
                        Animation animation = (Animation) drawable.Value;
                        if (animation.Frame >= animation.framecount && animation.IsLoop)
                        {
                            animation.Reset();
                        }
                        else if (animation.Frame >= animation.framecount && !animation.IsLoop)
                        {
                            drawList.Remove(drawable.Key);
                            continue;
                        }
                        animation.UpdateFrame(elapsed);
                        drawList[drawable.Key] = animation;
                        break;
                }
            }
        }

        public void Draw()
        {
            foreach (KeyValuePair<string, IDrawn> drawable in drawList)
            {
                switch (drawable.Value.DrawnType)
                {
                    case DrawnType.Animation:
                        Animation animation = (Animation) drawable.Value;
                        Texture2D drawTexture = textureManager.Textures[animation.Texture];
                        Rectangle frameRectangle = GameRectangle.ConvertToRectangle(animation.Frames[animation.Frame].TextureSource);
                        SpriteBatch.Draw(drawTexture, animation.GetPosition(), frameRectangle, Color.White, animation.Rotation, animation.GetOrigin(), animation.Scale, SpriteEffects.None, animation.Depth);
                        break;
                    case DrawnType.Shape:
                        IDrawnShape shape = (IDrawnShape) drawable.Value;
                        switch (shape.Shape)
                        {
                            case ShapeType.Line:
                                DrawnLine drawLine = (DrawnLine)shape;
                                DrawLine(drawLine);
                                break;
                            case ShapeType.Rectangle:
                                DrawnRectangle drawRectangle = (DrawnRectangle)shape;
                                DrawRectangle(drawRectangle);
                                break;
                        }
                        break;
                    case DrawnType.String:
                        DrawnString drawString = (DrawnString) drawable.Value;
                        DrawString(drawString);
                        break;
                }
            }
        }

        public void SetSpriteBatch()
        {
            SpriteBatch = new SpriteBatch(GraphicsManager.GraphicsDevice);
        }

        public GraphicsData SaveData()
        {
            return new GraphicsData(animationList, textureManager.Textures);
        }

        public bool DoesAnimationExist(string animationName)
        {
            return animationList.ContainsKey(animationName);
        }

        public bool DoesDrawableExist(string drawableName)
        {
            return drawList.ContainsKey(drawableName);
        }

        public void ClearDrawList()
        {
            drawList.Clear();
            drawList = new Dictionary<string, IDrawn>();
        }

        public void ClearAnimationList()
        {
            animationList.Clear();
            animationList = new Dictionary<string, Animation>();
        }

        public void ClearCache()
        {
            foreach (KeyValuePair<string, ObjectCache> pair in drawCache.Cache)
            {
                pair.Value.ClearCache();
            }
            drawCache.ClearCache();
        }

        public static Color ConvertSystemColorToXNA(System.Drawing.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        private Texture2D CreatePixel(Color color)
        {
            Texture2D returnPixel = new Texture2D(SpriteBatch.GraphicsDevice, 1,1, false, SurfaceFormat.Color);
            returnPixel.SetData(new[]{color});
            return returnPixel;
        }

        private void DrawLine(DrawnLine line)
        {
            Vector2 position = new Vector2(line.PositionX, line.PositionY);
            Vector2 endPos = new Vector2(line.EndX, line.EndY);
            float angle =
                (float)
                    Math.Atan2(line.EndX - line.PositionX,line.EndY - line.PositionY);
            float distance = Vector2.Distance(position,endPos);
            
            Texture2D pixelTexture = textureManager.PixelTextures[line.Name];
            SpriteBatch.Draw(pixelTexture, position, null, line.Color, angle, Vector2.Zero, new Vector2(distance, line.Thickness), SpriteEffects.None, 0);
        }

        private void DrawRectangle(DrawnRectangle rectangle)
        {

            DrawnLine top = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                PositionX = rectangle.PositionX,
                PositionY = rectangle.PositionY,
                EndX = rectangle.PositionX + rectangle.Size.Width,
                EndY = rectangle.PositionY,
                Thickness = rectangle.Thickness
            };

            DrawnLine right = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                PositionX = rectangle.PositionX + rectangle.Size.Width,
                PositionY = rectangle.PositionY,
                EndX = rectangle.PositionX + rectangle.Size.Width,
                EndY = rectangle.PositionY + rectangle.Size.Height,
                Thickness = rectangle.Thickness
            };

            DrawnLine bottom = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                PositionX = rectangle.PositionX + rectangle.Size.Width,
                PositionY = rectangle.PositionY + rectangle.Size.Height,
                EndX = rectangle.PositionX,
                EndY = rectangle.PositionY + rectangle.Size.Height,
                Thickness = rectangle.Thickness
            };

            DrawnLine left = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                PositionX = rectangle.PositionX,
                PositionY = rectangle.PositionY + rectangle.Size.Height,
                EndX = rectangle.PositionX,
                EndY = rectangle.PositionY,
                Thickness = rectangle.Thickness
            };

            DrawLine(top);
            DrawLine(right);
            DrawLine(bottom);
            DrawLine(left);

            
        }

        private void DrawString(DrawnString drawString)
        {
            SpriteBatch.DrawString(drawString.Font, drawString.Value, new Vector2(drawString.PositionX, drawString.PositionY), drawString.Color);
        }

        
    }
       
}
