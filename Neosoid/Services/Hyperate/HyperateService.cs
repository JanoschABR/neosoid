using JanoschR.Neosoid.Services;
using JanoschR.Neosoid.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services.Hyperate {
    public class HyperateService : IHeartbeatServiceFactory {
        public string GetIdentifier() {
            return "hyperate";
        }

        public string GetName() {
            return "HypeRate";
        }

        public IHeartbeatService CreateService(KVArgs args) {

            /*string device = null;
            if (args.Count > 0) {
                device = args.StealFirst();
            }*/

            if (!args.Require("device", out string device)) return null;

            HyperateSocket socket = new HyperateSocket();
            socket.JoinDeviceChannel(device);

            return socket;
        }
    }
}
