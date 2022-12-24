
using System;
using System.Threading.Tasks;

namespace JanoschR.Neosoid {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run(args);
        }

        public static string[] asciiArt = new string[] {
            "\u001b[38;5;211m" + @"       __                     _     _",
            "\u001b[38;5;210m" + @"    /\ \ \___  ___  ___  ___ (_) __| |",
            "\u001b[38;5;209m" + @"   /  \/ / _ \/ _ \/ __|/ _ \| |/ _` |",
            "\u001b[38;5;203m" + @"  / /\  /  __/ (_) \__ \ (_) | | (_| |",
            "\u001b[38;5;204m" + @"  \_\ \/ \___|\___/|___/\___/|_|\__,_|",
            ""
        };

        public void PrintBanner () {
            foreach (string line in asciiArt)
                Console.WriteLine(line);

            Console.WriteLine("  Neosoid, by JanoschR    \u001b[38;5;41mhttps://github.com/JanoschABR/neosoid\u001b[0m");
            Console.WriteLine(new string('-', Console.WindowWidth - 1));
            Console.WriteLine();
        }

        public void Run (string[] args) {
            Logger.EnableANSI();

            Console.Title = "Neosoid";
            Console.SetWindowSize(Console.WindowWidth, 20);
            PrintBanner();

            Guid widget = Guid.Empty;
            if (args.Length > 0) {
                string arg = args[0];

                if (arg.StartsWith("http")) {
                    Logger.Info("Widget URL specified. Parsing suffix GUID...");
                    widget = PulsoidRPC.ParseSuffixGuid(arg);
                } else {
                    widget = Guid.Parse(arg);
                }
            } else {

                Logger.Error("No arguments present. Please specifiy the Widget ID as the first argument.");
                Logger.Info("Press any key to exit.");
                Console.Read();
                return;
            }

            Logger.Info($"Using Widget ID \"{widget}\"");
            var ramiel = PulsoidRPC.RetrieveRamielID(widget);

            if (!ramiel.HasValue) {
                Logger.Error("No ramiel ID!");
                return;
            } else {
                Logger.Info($"Ramiel ID is {ramiel}");
            }

            using (NeosWS nws = new NeosWS()) {
                nws.Start(4444);

                using (PulsoidWSS wss = new PulsoidWSS()) {
                    wss.OnHeartbeatReceived += (rate) => nws.SendMessage($"{rate}");

                    wss.Connect(ramiel.Value);
                    Console.Read();
                }
            }
        }
    }
}
