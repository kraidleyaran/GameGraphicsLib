using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameGraphicsLib;

namespace GameGraphicsLib
{
    [Serializable]
    public class Animation : IDrawn
    {
        private const DrawnType _DrawnType = DrawnType.Animation;
        private readonly Dictionary<int, Frame> frames = new Dictionary<int, Frame>();
        
        public int framecount;

        public string Texture;
        private float TimePerFrame;
        private int _frame;
        private float _totalElapsed;
        private bool _paused;
        private float _originX;
        private float _originY;

        public float Rotation, Scale, Depth;
        
        public int FramesPerSecond;
        public bool IsLoop;

        public Animation(string name)
        {
            Name = name;
        }
        public Animation(string name, string texture, float rotation, float scale, float depth, Vector2 position, Vector2 origin, int framesPerSec )
        {
            SetPosition(position);
            SetOrigin(origin);
            Name = name;
            Texture = texture;
            framecount = frames.Count;
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
            FramesPerSecond = framesPerSec;
            TimePerFrame = (float) 1/FramesPerSecond;
        }
        public AnimationStatus Status { get; private set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get;set; }
        public DrawnType DrawnType { get { return _DrawnType; } set { } }
        public Dictionary<int, Frame> Frames { get { return frames; } }
        public int Frame { get { return _frame;} set { _frame = value; } }
        public void UpdateFrame(float elapsed)
        {
            if (_paused)
                return;
            if (Status != AnimationStatus.Playing)
            {
                Status = AnimationStatus.Playing;
            }
            _totalElapsed += elapsed;
            if (!(_totalElapsed > TimePerFrame)) return;
            _frame++;
            // Keep the Frame between 0 and the total frames, minus one.
            _frame = Frame % framecount;
            _totalElapsed -= TimePerFrame;
        }

        /*
        public Animation CloneAnimation(string name)
        {
            return new Animation(name, GetTextureData(), framecount, Rotation, Scale, Depth, GetPosition(), GetOrigin(), FramesPerSecond);
        }
         */

        public Animation CloneAnimation(string name)
        {
            Animation returnAnimation = new Animation(name, Texture, Rotation, Scale, Depth, GetPosition(), GetOrigin(), FramesPerSecond);
            foreach (KeyValuePair<int, Frame> pair in Frames)
            {
                returnAnimation.AddFrame(pair.Value);
            }
            return returnAnimation;
        }

        public bool IsPaused
        {
            get { return _paused; }
        }
        public void Reset()
        {
            Frame = 0;
            _totalElapsed = 0f;
        }
        public void Stop()
        {
            Pause();
            Reset();
            Status = AnimationStatus.Stopped;
        }
        public void Play()
        {
            _paused = false;
            Status = AnimationStatus.Playing;
        }
        public void Pause()
        {
            _paused = true;
            Status = AnimationStatus.Paused;
        }

        public Frame GetCurrentFrame()
        {
            return frames[Frame];
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
            _originX = origin.X;
            _originY = origin.Y;
            return new Vector2(_originX,_originY);
        }

        public Vector2 GetOrigin()
        {
            return new Vector2(_originX, _originY);
        }

        public void AddFrame(Frame frame)
        {
            frames.Add(framecount, frame);
            framecount++;
        }

        public bool RemoveFrame(int frame)
        {
            bool returnBool = frames.Remove(frame);
            if (returnBool)
            {
                framecount--;
            }

            return returnBool;
        }

        public bool SetFrame(int frameNumber, Frame frame)
        {
            if (frameNumber > framecount || frameNumber < 0)
            {
                return false;
            }

            frames[frameNumber] = frame;
            return true;
        }

        public void SetStatus(AnimationStatus status)
        {
            Status = status;
        }

        /*
        public Texture2D GetTextureData()
        {
            Texture2D returnTexture = new Texture2D(Graphics.GraphicsManager.GraphicsDevice, Texture.Width, Texture.Height);
            returnTexture.SetData(Texture.ColorData, 0, Texture.ColorData.Length);
            return returnTexture;
        }
         */
    }
}