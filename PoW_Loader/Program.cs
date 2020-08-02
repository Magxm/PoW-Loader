using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;

using SharpMonoInjector;

namespace PoW_Loader
{
    class Program
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        private static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }

        private static void KillProcessAndChildrens(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();

            // We must kill child processes first!
            if (processCollection != null)
            {
                foreach (ManagementObject mo in processCollection)
                {
                    KillProcessAndChildrens(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
                }
            }

            // Then kill parents.
            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        static void ErrorExit()
        {
            Console.ReadKey();
            System.Environment.Exit(-1);
        }

        static void Main(string[] args)
        {
            if (!(IntPtr.Size == 8))
            {
                Console.WriteLine("Not compiled as 64bit...");
                ErrorExit();
            }

            //Killing the game if already running
            Process[] gP = Process.GetProcessesByName("PathOfWuxia");
            if (gP.Length > 0)
            {
                Console.WriteLine("Game is running. Press Y to kill the game The game will NOT be saved! Press N to cancel.");
                Console.WriteLine("Kill the game? Y/N");
                var keyInfo = Console.ReadKey();
                Console.Write("\n");
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Killing Game Process...");
                    KillProcessAndChildrens(gP[0].Id);
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("Canceling!");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }

            //Starting the game
            ProcessStartInfo gameStartOverSteam = new ProcessStartInfo(@"Cmd.exe");
            gameStartOverSteam.Arguments = "/C START \"C:\\Program Files (x86)\\Steam.exe\" steam://rungameid/1189630";
            gameStartOverSteam.UseShellExecute = false;
            gameStartOverSteam.RedirectStandardInput = true;
            gameStartOverSteam.RedirectStandardOutput = true;
            gameStartOverSteam.RedirectStandardError = true;
            Console.WriteLine("Starting " + gameStartOverSteam.FileName + " " + gameStartOverSteam.Arguments);
            Process.Start(gameStartOverSteam);
            int gameProcessId = 0;
            IntPtr gameProcessHandle = IntPtr.Zero;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Waiting for game process...");
            while (true)
            {
                //Busy wait for game process
                Process[] gameProcesses = Process.GetProcessesByName("PathOfWuxia");
                if (gameProcesses.Length > 0)
                {
                    var _h = Native.OpenProcess(ProcessAccessRights.PROCESS_ALL_ACCESS, false, gameProcesses[0].Id);
                    Console.WriteLine("Found game process!");
                    gameProcessId = gameProcesses[0].Id;
                    gameProcessHandle = _h;
                    break;
                }

                if (timer.ElapsedMilliseconds > 10000)
                {
                    Console.WriteLine("ERROR: Failed starting the game. Game process did not spawn after 10 seconds!");
                    ErrorExit();
                }
            }

            if (gameProcessHandle == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Invalid game process!");
                ErrorExit();
            }

            timer.Restart();
            Console.WriteLine("Waiting for Mono to be loaded...");

            if (!ProcessUtils.Is64BitProcess(gameProcessHandle))
            {
                System.Console.WriteLine("ERROR: Game is not 64bit process!");
                ErrorExit();
            }

            if (gameProcessHandle == IntPtr.Zero)
            {
                System.Console.WriteLine("ERROR: Could not open game process!");
                ErrorExit();
            }

            while (true)
            {
                //Busy waiting for mono to properly load
                IntPtr _mono;
                if (ProcessUtils.GetMonoModule(gameProcessHandle, out _mono))
                {
                    //Found mono module
                    break;
                }

                if (timer.ElapsedMilliseconds > 10000)
                {
                    Console.WriteLine("ERROR: Mono did not properly load after 10 seconds!");
                    ErrorExit();
                }
            }
            Console.WriteLine("Mono loaded!");

            //Thread.Sleep(5000);


            //We inject the ressource patcher.
            Injector injector = new Injector(gameProcessId);
            string assemblyPath = Path.GetFullPath("PoW_EnglishPatch.dll");
            byte[] assembly = null;
            try
            {
                Console.WriteLine("Loading Resource Patcher Module...");
                assembly = File.ReadAllBytes(assemblyPath);
            }
            catch
            {
                ResumeProcess(gameProcessId);
                System.Console.WriteLine("ERROR: Could not read the file " + assemblyPath);
                ErrorExit();
            }

            if (assembly != null)
            {
                Console.WriteLine("Injecting Resource Patcher...");
                injector.Inject(assembly, @"PoW_EnglishPatch", @"Loader", @"Init");
                Console.WriteLine("Injected Resource Patcher!");
            }
            else
            {
                Console.WriteLine("ERROR: Assembly is empty...");
                ErrorExit();

            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
