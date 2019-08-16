using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UXCommon
{
    public abstract class ChangeNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string caller = default)
        {
            var ev = PropertyChanged;

            if (ev != null)
            {
                ev.Invoke(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
