using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleServer
{
    public class ClientStore
    {
        private readonly ConcurrentDictionary<string, string> _map =
            new ConcurrentDictionary<string, string>();

        public void AddConnection(string connectionId, string userId)
        {
            _map[connectionId] = userId;
        }

        public void RemoveConnection(string connectionId)
        {
            _map.TryRemove(connectionId, out _);
        }

        public List<string> GetConnections() => _map.Keys.ToList();
    }
}
