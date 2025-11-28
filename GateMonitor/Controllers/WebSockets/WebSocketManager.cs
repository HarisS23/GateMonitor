using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace GateMonitor.Controllers.WebSockets
{
    public class WebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        public void AddSocket(string id, WebSocket socket) => _sockets.TryAdd(id, socket);

        public void RemoveSocket(string id) => _sockets.TryRemove(id, out _);

        public async Task SendMessageAsync(string id, string message)
        {
            if (_sockets.TryGetValue(id, out WebSocket socket))
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
