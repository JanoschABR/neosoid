

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services.PulsoidRPC {
    public class PulsoidService : IHeartbeatServiceFactory {
        public string GetIdentifier() {
            return "pulsoid";
        }

        public string GetName() {
            return "Pulsoid RPC";
        }

        public IHeartbeatService CreateService(List<string> args) {

            string input = null;
            if (args.Count > 0) {
                input = args.StealFirst();
            } else {
                Logger.Error("Cannot create service: No input provided.");
                return null;
            }

            Guid guid;
            if (input.Contains("pulsoid.net")) {
                guid = PulsoidRPC.ParseSuffixGuid(input);
            } else {
                guid = Guid.Parse(input);
            }

            var ramiel = PulsoidRPC.RetrieveRamielID(guid);
            if (!ramiel.HasValue) {
                Logger.Error("Cannot create service: RPC returned no ramiel stream.");
                Logger.Warn("It seems the Pulsoid service does not know this widget. Make sure what you entered is correct and try again.");
                return null;
            }

            var wssURL = PulsoidRPC.CreateRamielURL(ramiel.Value);
            PulsoidListener listener = new PulsoidListener();
            listener.Connect(wssURL);

            return listener;
        }
    }
}
