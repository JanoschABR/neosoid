
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace JanoschR.Neosoid {
    public class NeosWS : IDisposable {
        protected HttpListener listener;
        protected HttpListenerContext context;
        protected HttpListenerWebSocketContext wsContext;
        protected CancellationTokenSource asyncReceiveCancellationToken;
        protected Thread thread;

        protected WebSocket socket;
        protected bool ready = false;

        public void SendMessage (string message) {
            if (!ready) return;

            var type = WebSocketMessageType.Text; var token = CancellationToken.None;
            socket.SendAsync(message.ToArraySegmentBuffer(), type, true, token).Wait();
        }

        public void Start (int port) {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            thread = new Thread(() => {
                context = listener.GetContext();
                if (context.Request.IsWebSocketRequest) {

                    while (true) {
                        wsContext = context.AcceptWebSocketAsync(null).WaitForResult();
                        socket = wsContext.WebSocket;

                        Logger.Info($"Incoming connection created: {wsContext.RequestUri}");
                        ready = true;

                        while (socket.State == WebSocketState.Open) {
                            Thread.Sleep(100);
                        }

                        ready = false;
                        Logger.Warn("Incoming connection closed.");
                    }
                }
            });

            thread.Start();
        }

        public Thread GetThread() {
            return thread;
        }
        public WebSocket GetSocket() {
            return socket;
        }

        public void Close() {
            Logger.Info("Disposing NeosWS...");

            if (socket != null) {
                var status = WebSocketCloseStatus.NormalClosure;
                socket.CloseAsync(status, "WSS was disposed.", CancellationToken.None).Wait();
            }
        }

        public void Dispose() {
            Logger.Info("Disposing NeosWS...");

            try {
                if (thread != null) {
                    thread.Interrupt();
                }
            } finally {
                Close();
            }
        }
    }
}
