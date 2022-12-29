using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid {
    public class Logger {

        protected class AnsiInjector {
            private const int STD_OUTPUT_HANDLE = -11;
            private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
            private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

            [DllImport("kernel32.dll")]
            private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            public static extern uint GetLastError();

            public static void Inject () {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                if (!GetConsoleMode(iStdOut, out uint outConsoleMode)) {
                    Console.WriteLine("Failed to get output console mode");
                    Console.ReadKey();
                    return;
                }

                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                if (!SetConsoleMode(iStdOut, outConsoleMode)) {
                    Console.WriteLine($"Failed to set output console mode. Error code: {GetLastError()}");
                    Console.ReadKey();
                    return;
                }
            }
        }

        public static void EnableANSI () {
            AnsiInjector.Inject();
        }

        protected static bool debug = false;
        public static void EnableDebug (bool enable) {
            debug = enable;
        }

        protected static bool stylize = true;
        public static void AllowStylization(bool allow) {
            stylize = allow;
        }

        public static void DebugInfo (string message) {
            if (!debug) return;
            Console.WriteLine($" \u001b[38;5;236m[DInf] {message}\u001b[0m");
        }
        public static void DebugWarn (string message) {
            if (!debug) return;
            Console.WriteLine($" \u001b[38;5;58m[DWrn] {message}\u001b[0m");
        }

        public static void Good (string message) {
            if (!stylize) {
                Info(message);
                return;
            }
            Console.WriteLine($" \u001b[38;5;255m[Info] \u001b[38;5;34m{message}\u001b[0m");
        }
        public static void Important (string message) {
            if (!stylize) {
                Info(message);
                return;
            }
            Console.WriteLine($" \u001b[38;5;205m[Info] {message}\u001b[0m");
        }

        public static void Info (string message) {
            Console.WriteLine ($" \u001b[38;5;255m[Info] {message}\u001b[0m");
        }
        public static void Warn (string message) {
            Console.WriteLine($" \u001b[38;5;220m[Warn] {message}\u001b[0m");
        }
        public static void Error (string message) {
            Console.WriteLine($" \u001b[38;5;196m[Fail] {message}\u001b[0m");
        }
    }
}
