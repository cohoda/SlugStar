using System.Collections.Concurrent;
using System.Linq;

namespace SlugStar.SlugStore
{
    public class InMemorySlugStore : ISlugStore
    {
        internal static ConcurrentBag<Slug> Cache = new ConcurrentBag<Slug>();

        public bool Exists(string slug)
        {
            return Cache.Any(x => x.Value == slug);
        }

        public void Store(Slug slug)
        {
            Cache.Add(slug);
        }
    }
}