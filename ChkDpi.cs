using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static Konome.Base64;
using static Konome.GUI.DisplayMetrics;
using static Konome.GUI.DPI;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Kernel32;

namespace Konome.ChkDpi
{
    class ChkDpi
    {
        public string Name { get; set; }
        public string Version { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                DebugConsole.Create(100, 100, 880, 450);

                IntPtr hwnd = (IntPtr)GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out uint processid);
                string fn = Process.GetProcessById((int)processid).MainModule.ModuleName;

                if (fn == "cmd.exe")
                    AttachConsole(processid);

                _ = new ChkDpi(args);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                MessageBox(IntPtr.Zero, e.Message, "Error", MB_FLAGS.MB_ICONERROR);
            }

#if DEBUG
            Console.ReadLine();
#endif
            DebugConsole.Close();
            Environment.Exit(0);
        }

        public ChkDpi(string[] args)
        {
            // Get application name and version.
            Name = Assembly.GetExecutingAssembly().GetName().Name;
            Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            if (args.Length > 0)
            {
                ParseArguments(args);
                return;
            }

            string str;
            str = $"\n{Name} v{Version} by konome\n";
            str += "---------------------------------\n\n";

            // Host OS.
            str += $"OS:\n{Environment.OSVersion}\n\n";

            // Primary monitor size and dpi.
            SetDpiAwareness(DpiAwareness.SYSTEM);
            str += $"Primary Monitor:\n{PrimaryMonitor.X}x{PrimaryMonitor.Y} @ {GetSystemDpi().X} DPI\n\n";

            // Get a list of available DPI awareness contexts.
            str += "Available DPI Awareness Context:\n";
            var dpi_ctx_dict = EnumerateDpiAwarenessContext();
            foreach (var ctx in dpi_ctx_dict)
                str += $"{ctx}\n";

            Console.WriteLine(str);

            // Encode string to base64 hash.
            string hash = EncodeToBase64(str);
            Console.WriteLine($"Bas64 hash: {hash}\n");

            // Copy base64 hash to clipboard.
            CopyToClipboard(hash);

            Console.WriteLine("Data copied to clipboard!");

            if (GetConsoleWindow() == IntPtr.Zero)
                MessageBox(IntPtr.Zero, "Data copied to clipboard!", Name, MB_FLAGS.MB_ICONINFORMATION);

        }

        private static void CopyToClipboard(string str)
        {
            IntPtr hMem = Marshal.StringToHGlobalUni(str);
            OpenClipboard(IntPtr.Zero);
            EmptyClipboard();
            SetClipboardData(CLIPFORMAT.CF_UNICODETEXT, hMem);
            CloseClipboard();
            Marshal.FreeHGlobal(hMem);
        }

        private void ParseArguments(string[] args)
        {
            if (CLI.Parse(args) == 1)
            {
                Console.WriteLine("Error while parsing command-line");
                return;
            }

            foreach (var option in CLI.Options)
            {
                switch(option.Key)
                {
                    case "-d" or "--decode":
                        {
                            string str = DecodeStringFromBase64(option.Value);
                            CopyToClipboard(str);
                            Console.WriteLine($"\n{str}\n");
                            Console.WriteLine("Copied to clipboard!");
                        }
                        break;

                    case "--decode-file":
                        {
                            if (!File.Exists(option.Value))
                            {
                                Console.WriteLine("\nFile does not exist.");
                                return;
                            }

                            string str = File.ReadAllText(option.Value);
                            string decode = DecodeStringFromBase64(str);
                            CopyToClipboard(decode);
                            Console.WriteLine($"\n{decode}\n");
                            Console.WriteLine("Copied to clipboard!");
                        }
                        break;

                    case "-e" or "--encode":
                        {
                            string str = EncodeToBase64(option.Value);
                            CopyToClipboard(str);
                            Console.WriteLine($"\n{str}\n");
                            Console.WriteLine("Copied to clipboard!");
                        }
                        break;

                    case "--encode-file":
                        {
                            if (!File.Exists(option.Value))
                            {
                                Console.WriteLine("\nFile does not exist.");
                                return;
                            }

                            string str = File.ReadAllText(option.Value);
                            string encode = EncodeToBase64(str);
                            CopyToClipboard(encode);
                            Console.WriteLine($"\n{encode}\n");
                            Console.WriteLine("Copied to clipboard!");
                        }
                        break;

                    case "-h" or "--help":
                        Console.WriteLine(
                            $"\n{Name} v{ Version} by konome\n\n" +
                            "-d, --decode HASH\n" +

                            "  Decode base64 data to a readable string.\n\n" +
                            "--decode-file FILE\n"+

                            "  Decode a .txt file containing a base64 hash.\n\n" +
                            "-e, --encode STRING\n"+

                            "  Encode a string value to base64.\n\n"+

                            "--encode-file FILE\n" +
                            "  Encode a file to base64.\n\n" +

                            "-v, --version\n"+
                            "  Show version of this application.\n\n"+

                            "-h, --help\n" +
                            "  Show help.\n\n");;
                        break;

                    case "-v" or "--version":
                        Console.WriteLine($"\n{Name} v{Version} by konome\n");
                        break;
                }
            }
        }
    }
}
