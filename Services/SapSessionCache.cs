using System;
using System.Collections.Concurrent;

namespace SapGateway.Services
{
    public class SapSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
    }

    public class SapSessionCache
    {
        private readonly ConcurrentDictionary<string, SapSession> _cache = new();

        public bool TryGet(string company, out SapSession? session)
        {
            if (_cache.TryGetValue(company, out var s) && s.ExpireAt > DateTime.UtcNow)
            {
                session = s;
                return true;
            }
            session = null;
            return false;
        }

        public void Set(string company, SapSession session) => _cache[company] = session;
        public void Remove(string company) => _cache.TryRemove(company, out _);
    }
}
