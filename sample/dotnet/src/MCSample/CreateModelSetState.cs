using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using MCSample.Forge;
using System.Collections.Generic;

namespace MCSample
{
    public class CreateModelSetState
    {
        public ForgeEntity PlansFolder { get; set; }

        public ForgeEntity TestFolderRoot { get; set; }

        public ForgeEntity TestFolder { get; set; }

        public List<ForgeUpload> Uploads { get; set; } = new List<ForgeUpload>();

        public ModelSet ModelSet { get; set; }
    }
}
