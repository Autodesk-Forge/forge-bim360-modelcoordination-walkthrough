using MCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCAuthWeb.Data
{
    public class ForgeEnvironmentModel
    {
        public ForgeEnvironment Binding { get; set; }

        public string Environment
        {
            get => Binding.ToString();
            set => Binding = Enum.Parse<ForgeEnvironment>(value);
        }
    }
}
