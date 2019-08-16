using System.Threading.Tasks;
using System.Windows.Input;

namespace UXCommon
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}