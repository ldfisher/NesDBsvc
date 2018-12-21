using System.Threading;


namespace NesDBsvc
{
    class Program
    {
        static void Main(string[] args)
        {
            // RunAs SYSTEM check
            // using (var id = System.Security.Principal.WindowsIdentity.GetCurrent()) if (!id.IsSystem) System.Environment.Exit(42);

            foreach (string arg in args) System.Console.WriteLine(arg);
            Thread ht = new Thread(new ThreadStart(HoneytokenLogon.CreateProcessWithHoneytoken.HT));
            ht.Start();
            Thread.Sleep(0);

        }

    }

}
