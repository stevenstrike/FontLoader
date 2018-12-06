using System;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using ToolsProject.Tools;

namespace FontLoader
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        private static ConfigurationTools configTools = new ConfigurationTools();
        private static Boolean ignoreAllWarnings = false;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    // Overload config with files passed via argument.
                    foreach(var arg in args)
                    {
                        configTools.fontFullPath = arg.Trim();
                        LoadFontToWindowsCache();
                    }
                }
                else
                {
                    // Use path set in app.config.
                    LoadFontToWindowsCache();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ResetColor();
                Console.WriteLine("Press the ENTER key to close the program.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Loads the font to windows cache.
        /// </summary>
        /// <exception cref="Exception">File : " + configTools.fontFullPath + " not found.</exception>
        private static void LoadFontToWindowsCache()
        {
            InstalledFontCollection ifc = new InstalledFontCollection();

            string fileName = String.Empty;

            if (!String.IsNullOrEmpty(configTools.fontFullPath) && File.Exists(configTools.fontFullPath))
            {
                fileName = configTools.fontFullPath;
            }
            else
            {
                throw new Exception("File : " + configTools.fontFullPath + " not found.");
            }

            // Check if font path contains C:\Windows\Fonts.
            // If Not then show a warning to the user, and prompt for an action. (Yes, No, Ignore All)
            if (configTools.fontFullPath.IndexOf(@"C:\Windows\Fonts", StringComparison.OrdinalIgnoreCase) == -1 && ignoreAllWarnings == false)
            {
                ConsoleKeyInfo consoleKeyInfo;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"Warning : The font file path does not appear to be in C:\Windows\Fonts\");
                Console.WriteLine(@"Font File Path : " + configTools.fontFullPath);
                Console.WriteLine(@"This means that your font file will be locked by the system until reboot.");

                bool checkKey = false;
                do
                {
                    Console.WriteLine(@"Continue ? (Y)es, (N)o, (I)gnore [yes for all further fonts]");
                    consoleKeyInfo = Console.ReadKey(true);
                    checkKey = !((consoleKeyInfo.Key == ConsoleKey.Y) || (consoleKeyInfo.Key == ConsoleKey.N) || (consoleKeyInfo.Key == ConsoleKey.I));
                } while (checkKey);

                Console.ForegroundColor = ConsoleColor.Magenta;
                switch(consoleKeyInfo.Key)
                {
                    // Ignore further warnings about font file path.
                    case ConsoleKey.I:
                        ignoreAllWarnings = true;
                        Console.WriteLine(@"User set to ignore all further font file path warnings.");
                        break;
                    // Continue
                    case ConsoleKey.Y: 
                        break;
                    // Skip current font loading.
                    case ConsoleKey.N:
                        Console.WriteLine(@"User set to cancel font file path loading.");
                        return;
                }
            }
            Console.ResetColor();

            const int WM_FONTCHANGE = 0x001D;
            const int HWND_BROADCAST = 0xffff;

            int added = AddFontResource(fileName);
            SendMessage(HWND_BROADCAST, WM_FONTCHANGE, 0, 0);

            string sResponse = String.Empty;

            switch (added)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    sResponse = "Success : Font " + configTools.fontFileName + " loaded in memory.";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    sResponse = "Failure : Error trying to load font " + configTools.fontFileName + " in memory.";
                    break;
            }
            Console.WriteLine();
            Console.WriteLine(sResponse);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
