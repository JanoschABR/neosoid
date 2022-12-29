
using JanoschR.Neosoid.Shared;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Timers;

namespace JanoschR.Neosoid.Neos {
    public class NeosServer : INeosServer {
        protected HttpListener listener;
        protected HttpListenerContext context;
        protected WebSocket socket;

        public void Dispose() {
            if (HasStarted()) {
                Stop();
            }
        }

        public bool HasStarted() {
            return (context != null);
        }

        public void Start(int port) {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            context = listener.GetContext();
            if (context.Request.IsWebSocketRequest) {

                var wsContext = context.AcceptWebSocketAsync(null).WaitForResult();
                if (wsContext == null) {
                    Logger.Error("WebSocket context was not retrieved properly!");
                    return;
                }

                socket = wsContext.WebSocket;

            } else {
                Logger.Error("Incoming request was not a WebSocket request!");
            }
        }

        public void Send (string message) {
            if (socket != null) {
                if (socket.State == WebSocketState.Open) {
                    try {
                        var type = WebSocketMessageType.Text; var token = CancellationToken.None;
                        socket.SendAsync(message.ToArraySegmentBuffer(), type, true, token).Wait();
                    } catch (Exception) {

                        // The only reason I've found that would cause an error here is if the remote side suddenly disconnected

                        Stop(); return;
                    }
                } else {
                    Logger.Warn("Cannot send message: Socket is not open!");
                }
            } else {
                Logger.Warn("Cannot send message: Socket has not been created yet!");
            }
        }

        public void WaitWhileOpen () {
            this.WaitWhile(100, () => {
                if (!HasStarted()) return false;
                if (socket == null) return false;
                return (socket.State == WebSocketState.Open);
            });
        }

        public void Stop() {
            if (socket != null) {
                try {
                    var status = WebSocketCloseStatus.NormalClosure; var token = CancellationToken.None;
                    socket.CloseAsync(status, "NeosServer.Stop was called.", token).Wait();
                } catch (Exception) {
                    Logger.Warn("Error occured while attempting to close socket!");
                }
            }

            if (listener != null) {
                try {
                    listener.Stop();
                } catch (Exception) {
                    Logger.Warn("Error occured while attempting to stop listener!");
                }
            }
        }
    }
}
