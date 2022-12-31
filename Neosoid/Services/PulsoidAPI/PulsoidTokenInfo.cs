using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Services.PulsoidAPI {
    public class PulsoidTokenInfo {

        public string client_id;
        public string profile_id;
        public long expires_in;
        public List<string> scopes;
    }
}
