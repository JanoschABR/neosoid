
using JanoschR.Neosoid.Neos;
using JanoschR.Neosoid.Services;
using JanoschR.Neosoid.Services.PulsoidRPC;
using JanoschR.Neosoid.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;
using Timer = System.Timers.Timer;

namespace JanoschR.Neosoid {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run(args);

            Logger.Warn("Application has exited main loop. Press any key to close the application completely.");
            Console.Read();
        }

        public static string[] asciiArt = new string[] {
            "\u001b[38;5;209m" + @"       __                     _     _",
            "\u001b[38;5;209m" + @"    /\ \ \___  ___  ___  ___ (_) __| |",
            "\u001b[38;5;210m" + @"   /  \/ / _ \/ _ \/ __|/ _ \| |/ _` |",
            "\u001b[38;5;203m" + @"  / /\  /  __/ (_) \__ \ (_) | | (_| |",
            "\u001b[38;5;204m" + @"  \_\ \/ \___|\___/|___/\___/|_|\__,_|",
            ""
        };

        public void PrintBanner () {
            foreach (string line in asciiArt)
                Console.WriteLine(line);

            Console.WriteLine("  Neosoid, by JanoschR    \u001b[38;5;41mhttps://github.com/JanoschABR/neosoid\u001b[0m");
            Console.WriteLine(new string('-', Console.WindowWidth - 1));
        }

        public void Run (string[] _args) {
            Logger.EnableANSI();

            Console.Title = "Neosoid";
            Console.SetWindowSize(Console.WindowWidth, 20);
            PrintBanner();

            Logger.EnableDebug(false);
            Logger.AllowStylization(true);

            KVArgs args = new KVArgs(_args);
            if (!args.Require("port", out string _port)) return;

            if (args.HasFlag("debug")) {
                Logger.EnableDebug(true);
            }

            if (args.HasFlag("minimize")) {
                WindowUtils.MinimizeConsoleWindow();
            }

            if (!int.TryParse(_port, out int port)) {
                Logger.Error("Argument with key \"port\" must be an integer.");
                return;
            }
            Logger.Info($"Using port {port}");

            IHeartbeatServiceFactory factory = null;
            if (!args.Require("service", out string service)) return;

            factory = ServiceDiscovery.GetFactory(service);
            if (factory == null) {
                Logger.Error($"Could not find Service with Identifier \"{service}\"");
                return;
            }

            try {
                Logger.Info($"Using {factory.GetName()} service.");
                Console.WriteLine();

                var instance = factory.CreateService(args);
                if (instance == null) return;

                Container<int> heartRate = new Container<int>(0);
                instance.OnHeartbeat += heartRate.Set;

                while (true) {
                    using (NeosServer server = new NeosServer()) {
                        server.Start(port);

                        Timer timer = new Timer();
                        timer.Interval = 1000;
                        timer.Elapsed += new ElapsedEventHandler((a, b) => {
                            server.Send($"{heartRate.Get()}");
                        });

                        timer.Start();
                        server.WaitWhileOpen();

                        timer.Stop();
                        server.Stop();
                    }
                }
            } catch (Exception ex) {
                Logger.Error($"Error occured during service runtime: [{ex.GetType().FullName}] {ex.Message}");
                return;
            }
        }
    }
}
