﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public List<string> GetDrawnNames()
        {
            return new List<string>(nameList.Keys);
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

        public Animation GetAnimation(string name)
        {
            if (!animationList.ContainsKey(name)) throw new KeyNotFoundException("Animation " + name + " does not exist");
            return animationList[name];
        }
        //TODO: Remove this method because the one below it is what I actually wanted to do. But updating the same animation on multiple objects may be useful
        public bool SetLoadedDrawn(IDrawn drawn)
        {
            if (!nameList.ContainsKey(drawn.Name)) return false;
            if (nameList[drawn.Name] != drawn.DrawnType) return false;
            
            drawCache.UpdateCache(drawn);
            if (drawList.ContainsKey(drawn.Name))
            {
                drawList[drawn.Name] = drawn;
            }
            return true;
        }

        public bool SetLoadedDrawn(IDrawn drawn, string parentObjectName)
        {
            if (!drawCache.Cache.ContainsKey(parentObjectName)) return false;
            drawCache.UpdateCache(drawn, parentObjectName);
            if (drawList.ContainsKey(parentObjectName))
            {
                drawList[parentObjectName] = drawn;    
            }
            return true;
        }

        public IDrawnShape GetShape(string name)
        {
            if (!shapeList.ContainsKey(name)) throw new KeyNotFoundException("Shape " + name + " does not exist");
            return shapeList[name];
        }

        public DrawnString GetString(string name)
        {
            if (!stringList.ContainsKey(name)) throw new KeyNotFoundException("String " + name + " does not exist");
            return stringList[name];
        }
        public bool AddDrawable(IDrawn drawable)
        {            
            switch (drawable.DrawnType)
            {
                case DrawnType.Animation:
                    Animation animation = (Animation) drawable;
                    if (nameList.ContainsKey(animation.Name)) return false;
                    animationList.Add(animation.Name, animation);
                    nameList.Add(animation.Name, DrawnType.Animation);
                    return true;
                case DrawnType.Shape:
                    IDrawnShape shape = (IDrawnShape) drawable;
                    if (nameList.ContainsKey(shape.Name)) return false;
                    shapeList.Add(shape.Name, shape );
                    nameList.Add(shape.Name, DrawnType.Shape);
                    textureManager.PixelTextures.Add(shape.Name, CreatePixel(shape.Color));
                    return true;                    
                case DrawnType.String:
                    DrawnString drawString = (DrawnString) drawable;
                    if (nameList.ContainsKey(drawString.Name)) return false;
                    nameList.Add(drawString.Name, DrawnType.String);
                    stringList.Add(drawString.Name, drawString);
                    return true;
            }
            return false;
        }

        public bool SetDrawable(IDrawn drawable)
        {
            if (!nameList.ContainsKey(drawable.Name)) return false;
            if (nameList[drawable.Name] != drawable.DrawnType) return false;
            switch (drawable.DrawnType)
            {
                case DrawnType.Animation:
                    Animation animation = (Animation)drawable;
                    animationList[animation.Name] = animation;
                    return true;
                case DrawnType.Shape:
                    IDrawnShape shape = (IDrawnShape)drawable;
                    shapeList[shape.Name] = shape;
                    return true;
                case DrawnType.String:
                    DrawnString drawString = (DrawnString)drawable;
                    stringList[drawString.Name] = drawString;
                    return true;
            }
            return false;
        }
        public bool RemoveDrawable(string name)
        {
            if (!nameList.ContainsKey(name)) return false;
            switch (nameList[name])
            {
                case DrawnType.Animation:
                    nameList.Remove(name);
                    return animationList.Remove(name);
                case DrawnType.Shape:
                    nameList.Remove(name);
                    return shapeList.Remove(name);
                case DrawnType.String:
                    nameList.Remove(name);
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
                    drawObject.X = param.Position.X;
                    drawObject.Y = param.Position.Y;
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
                    animation.X = param.Position.X;
                    animation.Y = param.Position.Y;
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
                            drawLine.X = param.Position.X;
                            drawLine.Y = param.Position.Y;
                            drawList.Add(line.Name, drawLine);
                            objectCache.Cache.Add(line.Name, drawLine);
                            return true;
                        case ShapeType.Rectangle:
                            DrawnRectangle rectangle = (DrawnRectangle) shape;
                            DrawnRectangle drawRectangle = rectangle.Clone(rectangle.Name);
                            drawRectangle.X = param.Position.X;
                            drawRectangle.Y = param.Position.Y;
                            drawList.Add(rectangle.Name, drawRectangle);
                            objectCache.Cache.Add(rectangle.Name, drawRectangle);
                            return true;
                    }
                    return true;
                case DrawnType.String:
                    if (!stringList.ContainsKey(param.DrawableName)) return false;
                    DrawnString drawString = stringList[param.DrawableName].Clone(param.DrawableName);
                    drawString.X = param.Position.X;
                    drawString.Y = param.Position.Y;
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
                    drawableObject.X = param.Position.X;
                    drawableObject.Y = param.Position.Y;
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
                    animation.X = param.Position.X;
                    animation.Y = param.Position.Y;
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
                            drawLine.X = param.Position.X;
                            drawLine.Y = param.Position.Y;
                            drawList.Add(param.ObjectName, drawLine);
                            objCache.Cache.Add(drawLine.Name, drawLine);
                            return true;
                        case ShapeType.Rectangle:
                            DrawnRectangle rectangle = (DrawnRectangle)shape;
                            DrawnRectangle drawRectangle = rectangle.Clone(rectangle.Name);
                            drawRectangle.X = param.Position.X;
                            drawRectangle.Y = param.Position.Y;
                            drawList.Add(param.ObjectName, drawRectangle);
                            objCache.Cache.Add(drawRectangle.Name, drawRectangle);
                            return true;

                    }
                    break;
                case DrawnType.String:
                    if (!stringList.ContainsKey(param.DrawableName)) return false;
                    DrawnString drawString = stringList[param.DrawableName].Clone(param.DrawableName);
                    drawString.X = param.Position.X;
                    drawString.Y = param.Position.Y;
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
            drawObject.X = param.Position.X;
            drawObject.Y = param.Position.Y;
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
                        animation.UpdateFrame(elapsed);
                        if (animation.Frame > animation.FrameCount && animation.IsLoop)
                        {
                            animation.Reset();
                        }
                        else if (animation.Frame > animation.FrameCount && !animation.IsLoop)
                        {
                             drawList.Remove(drawable.Key);
                            continue;
                        }
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
            Dictionary<string, IDrawn> drawnObjects = animationList.ToDictionary<KeyValuePair<string, Animation>, string, IDrawn>(animation => animation.Key, animation => animation.Value);
            foreach (KeyValuePair<string, IDrawnShape> shape in shapeList)
            {
                drawnObjects.Add(shape.Key, shape.Value);
            }
            foreach (KeyValuePair<string, DrawnString> drawString in stringList)
            {
                drawnObjects.Add(drawString.Key, drawString.Value);
            }
            return new GraphicsData(drawnObjects, textureManager.Textures, textureManager.PixelTextures);
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

        public IDrawn GetDrawable(string name)
        {
            if (!DoesDrawableExist(name)) throw new Exception("Drawable " + name + " does not exist");
            return drawList[name];
        }
        private Texture2D CreatePixel(Color color)
        {
            Texture2D returnPixel = new Texture2D(GraphicsManager.GraphicsDevice, 1,1, false, SurfaceFormat.Color);
            returnPixel.SetData(new[]{color});
            return returnPixel;
        }

        private void DrawLine(DrawnLine line)
        {
            Vector2 position = new Vector2(line.X, line.Y);
            Vector2 endPos = new Vector2(line.EndX, line.EndY);
            Vector2 edge = endPos - position;
            float angle = (float) Math.Atan2(edge.Y, edge.X);
            
            Texture2D pixelTexture = textureManager.PixelTextures[line.Name];
            SpriteBatch.Draw(pixelTexture,new Rectangle((int)position.X,(int)position.Y, (int)edge.Length(),(int)line.Thickness ),
                null,
                line.Color,
                angle,
                Vector2.Zero,
                SpriteEffects.None, 1);
        }

        private void DrawRectangle(DrawnRectangle rectangle)
        {

            DrawnLine top = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                X = rectangle.X,
                Y = rectangle.Y,
                EndX = rectangle.X + rectangle.Size.Width,
                EndY = rectangle.Y,
                Thickness = rectangle.Thickness
            };

            DrawnLine right = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                X = rectangle.X + rectangle.Size.Width,
                Y = rectangle.Y,
                EndX = rectangle.X + rectangle.Size.Width,
                EndY = rectangle.Y + rectangle.Size.Height,
                Thickness = rectangle.Thickness
            };

            DrawnLine bottom = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                X = rectangle.X + rectangle.Size.Width,
                Y = rectangle.Y + rectangle.Size.Height,
                EndX = rectangle.X,
                EndY = rectangle.Y + rectangle.Size.Height,
                Thickness = rectangle.Thickness
            };

            DrawnLine left = new DrawnLine
            {
                Name = rectangle.Name,
                Color = rectangle.Color,
                X = rectangle.X,
                Y = rectangle.Y + rectangle.Size.Height,
                EndX = rectangle.X,
                EndY = rectangle.Y,
                Thickness = rectangle.Thickness
            };

            DrawLine(top);
            DrawLine(right);
            DrawLine(bottom);
            DrawLine(left);

            
        }

        private void DrawString(DrawnString drawString)
        {
            SpriteBatch.DrawString(drawString.Font, drawString.Value, new Vector2(drawString.X, drawString.Y), drawString.Color);
        }

        
    }
       
}
