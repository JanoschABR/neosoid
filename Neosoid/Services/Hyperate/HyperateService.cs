﻿using JanoschR.Neosoid.Services;
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

        public IHeartbeatService CreateService(List<string> args) {

            string device = null;
            if (args.Count > 0) {
                device = args[0];
            } else {
                Logger.Error("Cannot create service: No input provided.");
                return null;
            }

            HyperateSocket socket = new HyperateSocket();
            socket.JoinDeviceChannel(device);

            return socket;
        }
    }
}
