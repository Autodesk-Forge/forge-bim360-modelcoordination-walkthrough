using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{
    public interface IRevitClashReportBuilder
    {
        Task<IReadOnlyCollection<RevitClashReport>> Build(Guid container, Guid modelSetId, uint verison, Guid clashTestId);
    }
}
