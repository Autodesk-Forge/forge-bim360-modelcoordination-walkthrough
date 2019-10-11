using MCCommon;
using System;

namespace MCAuthWeb.Data
{
    public class ForgeAppModel
    {
        public ForgeAppConfiguration Binding { get; set; }

        public string Account
        {
            get => Binding.Account.ToString();
            set => Binding.Account = Guid.Parse(value);
        }

        public string Project
        {
            get => Binding.Account.ToString();
            set => Binding.Account = Guid.Parse(value);
        }
    }
}
