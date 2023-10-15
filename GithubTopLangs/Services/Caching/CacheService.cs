using Microsoft.Extensions.Caching.Memory;

namespace GithubTopLangs.Services.Caching
{
    public class CacheService
    {
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private static CacheService? _instance;
        private static readonly object _lock = new();

        public static CacheService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CacheService();
                        }
                    }
                }
                return _instance;
            }
        }

        private CacheService()
        { }

        public string? GetCachedSvg(string name)
        {
            if (_cache.TryGetValue(name, out string card))
            {
                return card;
            }
            return null;
        }

        public void CacheSvg(string name, string svg) => _cache.Set(name, svg);
    }
}