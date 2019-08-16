using System;
using System.Collections.Generic;
using System.Text;

namespace MCSample
{
    public class PctPorcessedEventArgs : EventArgs
    {
        public PctPorcessedEventArgs(uint percent) => Percent = percent > 100 ? 100 : percent;

        public uint Percent { get; private set; }
    }
}
