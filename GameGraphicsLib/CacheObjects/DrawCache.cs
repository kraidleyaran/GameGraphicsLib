using System.Collections.Generic;
using System.Linq;

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

        public void UpdateCache(IDrawn drawnObject)
        {
            foreach (KeyValuePair<string, ObjectCache> pair in Cache.Where(pair => pair.Value.Cache.ContainsKey(drawnObject.Name)))
            {
                pair.Value.Cache[drawnObject.Name] = drawnObject;
            }
        }

        public void UpdateCache(IDrawn drawnObject, string parentObjectName)
        {
            if (!Cache.ContainsKey(parentObjectName)) return;
            ObjectCache objCache = Cache[parentObjectName];
            if (!objCache.Cache.ContainsKey(drawnObject.Name)) return;
            objCache.Cache[drawnObject.Name] = drawnObject;
        }
    }
}