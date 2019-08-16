using MCCommon;
using System.Reflection;

namespace MCConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleCommandHost.Run("Model Coordination Developer Configuration Utility", new Assembly[] { Assembly.GetExecutingAssembly() });
        }
    }
}
