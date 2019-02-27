using System;
using System.Threading;
using System.Windows.Forms;

namespace WebProxy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //The console for displaying cache and blocking info
            ManagementConsole managementConsole = new ManagementConsole();

            //This will listen for connection requests on port 4444 on a separate thread
            HttpPortListener portListener = new HttpPortListener(4444, "127.0.0.1", 100);
            try
            {
                Thread listenThread = new Thread(() => portListener.PortListener(managementConsole));
                listenThread.Name = "HttpListener";
                listenThread.SetApartmentState(ApartmentState.STA);
                listenThread.Start();
            }
            catch (NotSupportedException nse)
            {
                Console.WriteLine("Unsupported http request\n{0}\n", nse);
            }

            Application.Run(managementConsole);
        }
    }
}
