using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameGraphicsLib
{
    [Serializable]
    public class GameGraphics
    {
        //private ContentManager ContentManager;
        private Dictionary<string, Animation> animationList = new Dictionary<string, Animation>();
        private Dictionary<string, Animation> drawList = new Dictionary<string, Animation>();

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

        public bool AddAnimation(Animation animation)
        {
            if (animationList.ContainsKey(animation.Name))
            {
                return false;
            }

            animationList.Add(animation.Name, animation);
            return true;
        }

        public bool RemoveAnimation(string animationName)
        {
            if (!animationList.ContainsKey(animationName)) return false;
            animationList.Remove(animationName);
            return true;
        }

        public bool RemoveAnimation(Animation animation)
        {
            return RemoveAnimation(animation.Name);
        }

        public bool AddToDrawList(DrawParam param)
        {
            if (!animationList.ContainsKey(param.Animation) || drawList.ContainsKey(param.Animation)) return false;
            Animation animation;
            animationList.TryGetValue(param.Animation, out animation);
            if (animation == null) return false;

            Animation newAnimation = animation.CloneAnimation(param.Animation, this);
            newAnimation.SetPosition(param.Position);
            drawList.Add(param.ObjectName, newAnimation);
            return true;
        }
        

        public bool UpdateDrawList(DrawParam param)
        {
            if (!drawList.ContainsKey(param.ObjectName)) return false;
            Animation animation;
            animationList.TryGetValue(param.Animation, out animation);
            if (animation == null) return false;

            animation.SetPosition(param.Position);
            return true;
        }

        public bool UpdateDrawPosition(DrawParam param)
        {
            if (!drawList.ContainsKey(param.ObjectName)) return false;
            Animation animation;
            drawList.TryGetValue(param.ObjectName, out animation);
            if (animation == null) return false;

            animation.SetPosition(param.Position);
            return true;
        }
        public bool RemoveFromDrawList(string objectName)
        {
            return drawList.Remove(objectName);
        }

        public Animation GetDrawAnimation(string objectName)
        {
            if (!drawList.ContainsKey(objectName)) throw new Exception("Animation does not exist");
            Animation animation;
            drawList.TryGetValue(objectName, out animation);
            if (animation == null ) throw new Exception("Animation is null");
            return animation;
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach (KeyValuePair<string, Animation> drawable in drawList.ToList())
            {
                if (drawable.Value.GetCurrentFrame() > (drawable.Value.GetFrameCount() - 1) && (!drawable.Value.IsLoop))
                {
                    drawList.Remove(drawable.Key);
                    continue;
                }
                //Else
                drawable.Value.UpdateFrame(elapsed);
            }
        }

        public void Draw()
        {
            foreach (KeyValuePair<string, Animation> drawable in drawList)
            {
                drawable.Value.DrawFrame(SpriteBatch, this);
            }
        }

        public void SetSpriteBatch()
        {
            SpriteBatch = new SpriteBatch(GraphicsManager.GraphicsDevice);
        }

        public GraphicsData SaveData()
        {
            return new GraphicsData(animationList, this);
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
        }

        public void ClearAnimationList()
        {
            animationList.Clear();
        }

        
    }
       
}
