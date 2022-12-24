using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid {
    public static class Extensions {

        public static ArraySegment<byte> ToArraySegmentBuffer (this string text) {
            return new ArraySegment<byte>(Encoding.ASCII.GetBytes(text));
        }

        public static int? ToInt (this WebSocketCloseStatus? status) {
            return (status == null ? null : (int?)((int)status));
        }

        public static void SafeInvoke (this Action action, Action<Exception> onError = null) {
            if (action != null) {
                try {
                    action.Invoke();
                } catch (Exception ex) {
                    if (onError != null) onError.Invoke(ex);
                }
            }
        }

        public static void SafeInvoke <T1> (this Action<T1> action, T1 arg1, Action<Exception> onError = null) {
            if (action != null) {
                try {
                    action.Invoke(arg1);
                } catch (Exception ex) {
                    if (onError != null) onError.Invoke(ex);
                }
            }
        }

        public static void SafeInvoke <T1, T2> (this Action<T1, T2> action, T1 arg1, T2 arg2, Action<Exception> onError = null) {
            if (action != null) {
                try {
                    action.Invoke(arg1, arg2);
                } catch (Exception ex) {
                    if (onError != null) onError.Invoke(ex);
                }
            }
        }

        public static T WaitForResult <T> (this Task<T> task) {
            try {
                task.Wait();
                return task.Result;

            } catch (Exception) {
                return default(T);
            }
        }
    }
}
