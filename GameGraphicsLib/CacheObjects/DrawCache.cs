using System.Collections.Generic;

namespace GameGraphicsLib.CacheObjects
{
    public class DrawCache : ICache
    {

        public DrawCache()
        {
            Cache = new Dictionary<string, ObjectCache>();
        }

        public Dictionary<string, ObjectCache> Cache { get; private set; }

        public void ClearCache()
        {
            Cache.Clear();
            Cache = new Dictionary<string, ObjectCache>();
        }

        
    }
}