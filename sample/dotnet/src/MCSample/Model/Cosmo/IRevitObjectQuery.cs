using Autodesk.Nucleus.Scopes.Entities.V3;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{
    public interface IRevitObjectQuery
    {
        Task<IReadOnlyDictionary<string, RevitObject[]>> GetRevitObjects(Guid container, ModelSetVersion modelSetVersion);
    }
}
