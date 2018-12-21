using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HoneytokenLogon
{
    public class CreateProcessWithHoneytoken
    {
        [Flags]
        enum LogonFlags
        {
            LOGON_NETCREDENTIALS_ONLY = 0x00000002
        }

        [Flags]
        enum CreationFlags
        {
            CREATE_SUSPENDED = 0x00000004
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ProcessInfo
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct StartupInfo
        {
            public int cb;
            public string reserved1;
            public string desktop;
            public string title;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public short reserved2;
            public int reserved3;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true,
         SetLastError = true)]
        static extern bool CreateProcessWithLogonW(
            string principal,
            string authority,
            string password,
            LogonFlags logonFlags,
            string appName,
            string cmdLine,
            CreationFlags creationFlags,
            IntPtr environmentBlock,
            string currentDirectory,
            ref StartupInfo startupInfo,
            out ProcessInfo processInfo);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr h);

        private static uint CreateHoneytokenProcess(string appPath, string domain, string user,
            string password, LogonFlags lf, CreationFlags cf)
        {
            StartupInfo si = new StartupInfo
            {
                cb = Marshal.SizeOf(typeof(StartupInfo))
            };
            ProcessInfo pi = new ProcessInfo
            {
                dwProcessId = 0
            };

            if (CreateProcessWithLogonW(user, domain, password,
            lf,
            appPath, null,
            cf, IntPtr.Zero, null,
            ref si, out pi))
            {
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            }
            else
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return (pi.dwProcessId);
        }

        public static void HT()
        {
            while (true)
            {
                try
                {
                    uint pid = CreateHoneytokenProcess(
                       "C:\\WINDOWS\\notepad.exe",
                    "DOMAIN", "USER", "PASSWORD",
                        LogonFlags.LOGON_NETCREDENTIALS_ONLY,
                        CreationFlags.CREATE_SUSPENDED
                    );
                    Console.WriteLine(pid);
                    System.Threading.Thread.Sleep(86400000);
                    Process.GetProcessById((int)pid).Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Process failed: '{0}'", e);
                    Console.WriteLine("Sleeping for 60 seconds...");
                    System.Threading.Thread.Sleep(60000);
                }
            }
        }
    }
}
