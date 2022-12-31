using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static JanoschR.Neosoid.Services.PulsoidRPC.PulsoidRPC;

namespace JanoschR.Neosoid.Services.PulsoidAPI {
    public class PulsoidTokenAPI {

        public static PulsoidTokenInfo GetTokenInfo (Guid guid) {
            try {

                // Create the request
                var rpcURL = "https://dev.pulsoid.net/api/v1/token/validate";
                var httpRequest = (HttpWebRequest)WebRequest.Create(rpcURL);
                httpRequest.ContentType = "application/json";
                httpRequest.Method = "GET";
                httpRequest.Headers.Add("Authorization", $"Bearer {guid}");

                // Read response data
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {

                    string result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<PulsoidTokenInfo>(result);
                }

            } catch (Exception) {
                return null;
            }
        }

        public static bool CheckTokenValidity (Guid guid) {

            var info = GetTokenInfo(guid);
            if (info == null) {
                Logger.Error("The specified token is invalid.");
                return false;
            } else {

                bool auth = info.scopes.Contains("data:heart_rate:read");
                if (!auth) {
                    Logger.Error("The specified token does not grant data:heart_rate:read permissions.");
                }

                return auth;
            }
        }

    }
}
