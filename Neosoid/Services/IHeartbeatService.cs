using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services {
    public interface IHeartbeatService {
        event Action<int> OnHeartbeat;
    }
}
