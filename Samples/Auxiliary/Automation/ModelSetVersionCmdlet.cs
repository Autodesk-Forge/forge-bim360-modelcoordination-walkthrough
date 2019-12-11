using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace Forge.Automation
{
    public abstract class ModelSetVersionCmdlet : ForgeCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true)]
        public Guid Container { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public Guid ModelSet { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        public int Version { get; set; }
    }
}
