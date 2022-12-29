
using JanoschR.Neosoid.Services.Hyperate.PhoenixMockup;
using JanoschR.Neosoid.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JanoschR.Neosoid.Services.Hyperate {
    public class HyperateSocket : PhoenixSocket, IHeartbeatService {
        protected static string accessKey = "fwCC3S77TV6gX7LFZb0Lv8cnW21JTbdGHkTUDNIRAL58eqmiI5FEVgLzpbUcMr9E";

        protected override void HandlePhoenixMessage(PhoenixMessage message, string json) {
            if (message.GetEvent() == "hr_update") {
                if (message.HasPayloadValue("hr")) {

                    int hr = (int)message.GetPayloadValue<long>("hr");
                    OnHeartbeat.SafeInvoke(hr);

                } else {
                    Logger.DebugWarn($"Received hr_update message without hr value in payload: {json}");
                }

            } else if (message.GetEvent() == "phx_reply") {
                if (message.HasPayloadValue("status")) {
                    string status = message.GetPayloadValue<string>("status");
                    
                    if (message.GetRef() == 64) {
                        // Reply is for Join message

                        if (status == "error") {
                            Logger.Error($"Could not join Phoenix channel: {json}");
                        } else {
                            if (status != "ok") {
                                Logger.Warn($"Received unknown request reply: {json}");
                            } else {
                                Logger.Good("Successfully joined Phoenix channel!");
                            }
                        }
                    } else {

                        if (status == "error") {
                            Logger.Error($"Received negative status reply: {json}");
                        } else {
                            if (status != "ok") {
                                Logger.Warn($"Received unknown status reply: {json}");
                            }
                        }
                    }

                } else {
                    Logger.DebugWarn($"Received phx_reply message without status in payload: {json}");
                }
            }
        }

        public void SendConnectionHeartbeat (Timer owner) {

            if (GetState() != System.Net.WebSockets.WebSocketState.Open) {
                owner.Stop();
                return;
            }

            PhoenixMessage heartbeatMessage = new PhoenixMessage() {
                @event = "heartbeat", @ref = 0,
                topic = $"phoenix",
                payload = new Dictionary<string, object>()
            };

            SendPhoenixMessage(heartbeatMessage);
        }

        public void JoinDeviceChannel (string device = null) {
            if (device == null) {
                Logger.Warn("Device not specified! Using internal testing channel...");
                device = "internal-testing";

            } else {
                Logger.Info($"Joining Phoenix Channel \"{device}\"...");
            }

            PhoenixMessage joinMessage = new PhoenixMessage() {
                @event = "phx_join", @ref = 64,
                topic = $"hr:{device}",
                payload = new Dictionary<string, object>()
            };

            SendPhoenixMessage(joinMessage);

            Timer heartbeatTimer = new Timer();
            heartbeatTimer.Interval = 25000;
            heartbeatTimer.Elapsed += new ElapsedEventHandler((a, b) => SendConnectionHeartbeat(heartbeatTimer));
            heartbeatTimer.Start();
        }

        public HyperateSocket () {
            Connect($"wss://app.hyperate.io/socket/websocket?token={accessKey}");
        }

        public event Action<int> OnHeartbeat;
    }
}
