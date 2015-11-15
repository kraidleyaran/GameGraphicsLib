using System.Collections.Generic;

namespace GameGraphicsLib
{
    public class AnimationCache
    {

        public AnimationCache()
        {
            Cache = new Dictionary<string, Animation>();
        }

        public Dictionary<string, Animation> Cache { get; private set; }

        public void ClearCache()
        {
            Cache.Clear();
            Cache = new Dictionary<string, Animation>();
        }
    }
}