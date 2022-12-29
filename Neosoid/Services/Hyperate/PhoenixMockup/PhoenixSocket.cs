using JanoschR.Neosoid.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services.Hyperate.PhoenixMockup {
    public class PhoenixSocket : GenericWebSocketListener {
        protected virtual void HandlePhoenixMessage (PhoenixMessage message, string json) { }

        public void SendPhoenixMessage (PhoenixMessage message) {
            string json = JsonConvert.SerializeObject (message);
            SendMessage(json);
        }

        protected override void HandleMessageReceived(string text) {
            PhoenixMessage message = JsonConvert.DeserializeObject<PhoenixMessage>(text);
            HandlePhoenixMessage(message, text);
        }
    }
}
