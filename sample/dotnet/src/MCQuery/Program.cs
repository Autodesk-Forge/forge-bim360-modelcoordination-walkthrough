using MCCommon;
using MCSample.Model;
using System.Reflection;

namespace MCQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleCommandHost.Run(
                "Model Coordination Query API",
                new Assembly[]
                {
                    Assembly.GetExecutingAssembly(),
                    typeof(IForgeIndexClient).Assembly
                });
        }
    }
}
