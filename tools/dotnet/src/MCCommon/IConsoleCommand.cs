using System.Threading.Tasks;

namespace MCCommon
{
    public interface IConsoleCommand
    {
        uint Order { get; }

        uint Group { get; }

        string Display { get; }

        Task DoInput();

        bool Continue();

        Task RunCommand();
    }
}
