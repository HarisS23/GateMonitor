using GateMonitor.Controllers.WebSockets;
using WebSocketManager = GateMonitor.Controllers.WebSockets.WebSocketManager;

namespace GateMonitor.Services
{
    public static class MachineStateService
    {
        private static readonly object _lock = new object();
        private static bool _isMachineOn = false;
        private static WebSocketManager _wsManager;

        public static void Initialize(WebSocketManager wsManager)
        {
            _wsManager = wsManager;
        }

        public static bool IsMachineOn
        {
            get
            {
                lock (_lock) { return _isMachineOn; }
            }
            set
            {
                bool changed = false;
                lock (_lock)
                {
                    if (_isMachineOn != value)
                    {
                        _isMachineOn = value;
                        changed = true;
                    }
                }

                if (changed && _wsManager != null)
                {
                    string msg = _isMachineOn ? "ON" : "OFF";
                    _ = _wsManager.BroadcastMessageAsync(msg);
                }
            }
        }
    }
}