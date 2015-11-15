using System.Collections.Generic;

namespace GameGraphicsLib.CacheObjects
{
    public class ObjectCache: ICache
    {
        public ObjectCache()
        {
            Cache = new Dictionary<string, IDrawn>();
        }
        public Dictionary<string, IDrawn> Cache { get; private set; }

        public void ClearCache()
        {
            Cache.Clear();
            Cache = new Dictionary<string, IDrawn>();
        }
    }
}