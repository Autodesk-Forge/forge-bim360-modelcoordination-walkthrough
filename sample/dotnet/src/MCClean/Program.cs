using MCCommon;
using System.Reflection;

namespace MCClean
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleCommandHost.Run("Model Coordination Developer Project Cleaner", new Assembly[] { Assembly.GetExecutingAssembly() });
        }
    }
}
