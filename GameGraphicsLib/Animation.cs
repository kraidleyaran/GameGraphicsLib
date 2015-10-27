using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameGraphicsLib;

namespace GameGraphicsLib
{
    [Serializable]
    public class Animation
    {
        public int framecount;

        private BinaryTexture Texture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private bool Paused;
        private float OriginX;
        private float OriginY;

        public float Rotation, Scale, Depth;

        public float PositionX;
        public float PositionY;
        
        public int FramesPerSecond;
        public bool IsLoop;

        public Animation(string name)
        {
            Name = name;
        }
        public Animation(string name, Texture2D texture, int frames, float rotation, float scale, float depth, Vector2 position, Vector2 origin, int framesPerSec )
        {
            SetPosition(position);
            SetOrigin(origin);
            Name = name;
            Texture = new BinaryTexture(texture);
            framecount = frames;
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
            Height = texture.Height;
            Width = texture.Width;
            FramesPerSecond = framesPerSec;
            TimePerFrame = (float) 1/FramesPerSecond;
        }

        public string Name { get; set; }
        public float Height { get; private set; }
        public float Width { get; private set; }

        public void UpdateFrame(float elapsed)
        {
            if (Paused)
                return;

            TotalElapsed += elapsed;
            if (!(TotalElapsed > TimePerFrame)) return;
            Frame++;
            // Keep the Frame between 0 and the total frames, minus one.
            Frame = Frame % framecount;
            TotalElapsed -= TimePerFrame;
        }
        public void DrawFrame(SpriteBatch batch, GameGraphics graphics)
        {
            DrawFrame(batch,graphics, Frame, GetPosition(), GetOrigin());
        }

        /*
        public Animation CloneAnimation(string name)
        {
            return new Animation(name, GetTextureData(), framecount, Rotation, Scale, Depth, GetPosition(), GetOrigin(), FramesPerSecond);
        }
         */

        public Animation CloneAnimation(string name, GameGraphics graphics)
        {
            return new Animation(name, GetTextureData(graphics), framecount, Rotation, Scale, Depth, GetPosition(), GetOrigin(), FramesPerSecond);
        }
        private void DrawFrame(SpriteBatch batch, GameGraphics graphics, int frame, Vector2 screenPos, Vector2 origin)
        {
            Texture2D texture = GetTextureData(graphics);
            int frameWidth = (int)Width / framecount;
            Rectangle sourcerect = new Rectangle(frameWidth * frame, 0,
                frameWidth, texture.Height);
            batch.Draw(texture, screenPos, sourcerect, Color.White,
                Rotation, origin, Scale, SpriteEffects.None, Depth);
        }

        public bool IsPaused
        {
            get { return Paused; }
        }
        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
        }
        public void Stop()
        {
            Pause();
            Reset();
        }
        public void Play()
        {
            Paused = false;
        }
        public void Pause()
        {
            Paused = true;
        }

        public int GetCurrentFrame()
        {
            return Frame;
        }

        public int GetFrameCount()
        {
            return framecount;
        }

        public bool Loop()
        {
            if (IsLoop)
            {
                IsLoop = false;
                return IsLoop;
            }
            IsLoop = true;
            return IsLoop;
        }

        public Vector2 SetPosition(Vector2 position)
        {
            PositionX = position.X;
            PositionY = position.Y;
            return position;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(PositionX, PositionY);
        }

        public Vector2 SetOrigin(Vector2 origin)
        {
            OriginX = origin.X;
            OriginY = origin.Y;
            return new Vector2(OriginX,OriginY);
        }

        public Vector2 GetOrigin()
        {
            return new Vector2(OriginX, OriginY);
        }

        /*
        public Texture2D GetTextureData()
        {
            Texture2D returnTexture = new Texture2D(Graphics.GraphicsManager.GraphicsDevice, Texture.Width, Texture.Height);
            returnTexture.SetData(Texture.ColorData, 0, Texture.ColorData.Length);
            return returnTexture;
        }
         */

        public Texture2D GetTextureData(GameGraphics graphics)
        {
            Texture2D returnTexture = new Texture2D(graphics.GraphicsManager.GraphicsDevice, Texture.Width, Texture.Height);
            returnTexture.SetData(Texture.ColorData, 0, Texture.ColorData.Length);
            return returnTexture;
        }
    }
}