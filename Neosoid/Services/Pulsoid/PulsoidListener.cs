using JanoschR.Neosoid.Services;
using JanoschR.Neosoid.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services.Pulsoid {
    public class PulsoidListener : GenericWebSocketListener, IHeartbeatService {
        protected override void HandleMessageReceived(string text) {
            var message = JsonConvert.DeserializeObject<WSSMessage>(text);
            if (message.data == null) return;

            OnHeartbeat.SafeInvoke(message.data.heartRate);
        }

        public event Action<int> OnHeartbeat;

        public class WSSMessage {
            public long timestamp;
            public WSSHeartbeatData data;
        }
        public class WSSHeartbeatData {
            public int heartRate;
        }
    }
}
