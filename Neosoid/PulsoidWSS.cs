using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JanoschR.Neosoid {
    public class PulsoidWSS : IDisposable {

        protected ClientWebSocket socket;
        protected Thread thread;

        public event Action<string> OnMessageReceived;
        public event Action<int> OnHeartbeatReceived;

        public class WSSMessage {
            public long timestamp;
            public WSSHeartbeatData data;
        }
        public class WSSHeartbeatData {
            public int heartRate;
        }

        public void Connect (Guid ramielID) {
            Connect(PulsoidRPC.CreateRamielURL(ramielID));
        }

        public void Connect (string wssURL) {
            Logger.Info($"Connecting to \"{wssURL}\"...");

            socket = new ClientWebSocket();
            socket.ConnectAsync(new Uri(wssURL), CancellationToken.None).Wait();

            thread = new Thread(() => {
                byte[] buffer = new byte[1024];
                ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer);

                while (socket.State == WebSocketState.Open) {
                    var packet = socket.ReceiveAsync(bufferSegment, CancellationToken.None).WaitForResult();

                    if (packet.MessageType == WebSocketMessageType.Close) {
                        Close(); return;

                    } else {
                        string text = Encoding.ASCII.GetString(bufferSegment.ToArray(), 0, packet.Count);
                        OnMessageReceived.SafeInvoke(text);

                        var message = JsonConvert.DeserializeObject<WSSMessage>(text);
                        if (message.data == null) continue;

                        OnHeartbeatReceived.SafeInvoke(message.data.heartRate);
                    }
                }
            });

            thread.Start();
        }

        public Thread GetThread () {
            return thread;
        }
        public ClientWebSocket GetSocket () {
            return socket;
        }

        public void Close () {
            Logger.Info("Closing PulsoidWSS...");

            if (socket != null) {
                var status = WebSocketCloseStatus.NormalClosure;
                socket.CloseAsync(status, "WSS was disposed.", CancellationToken.None).Wait();
            }
        }

        public void Dispose() {
            Logger.Info("Disposing PulsoidWSS...");

            try {
                if (thread != null) {
                    Logger.Warn("Interrupting PulsoidWSS thread...");
                    thread.Interrupt();
                }
            } finally {
                Close();
            }
        }
    }
}
