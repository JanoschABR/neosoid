using JanoschR.Neosoid.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services {
    public interface IHeartbeatServiceFactory {
        string GetIdentifier();
        string GetName();

        IHeartbeatService CreateService(KVArgs args);
    }
}
