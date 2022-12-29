using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Neos {
    public interface INeosServer : IDisposable {
        void Start(int port);
        bool HasStarted();
        void Stop();
        void Send(string message);
        void WaitWhileOpen();
    }
}
