using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{
    [Export(typeof(IRevitObjectQuery))]
    internal class RevitObjectQuery : IRevitObjectQuery
    {
        private readonly IForgeIndexClient _indexClient;

        [ImportingConstructor]
        public RevitObjectQuery(IForgeIndexClient indexClient) => _indexClient = indexClient;

        public IndexManifest Manifest { get; private set; }

        public async Task<IReadOnlyDictionary<string, RevitObject[]>> GetRevitObjects(Guid container, ModelSetVersion modelSetVersion)
        {
            var objects = new List<RevitObject>();

            var query = "select s.file, s.db, s.docs, s.id, s.p153cb174 as name, s.p20d8441e as cat, s.p30db51f9 as fam, s.p13b6b3a0 as typ from s3object s";

            await _indexClient.QueryIndex(
                container,
                modelSetVersion.ModelSetId,
                (uint)modelSetVersion.Version,
                query,
                async stream =>
                {
                    var reader = new IndexResultReader(stream, null);

                    await reader.ReadToEndAsync(obj =>
                    {
                        objects.Add(obj.ToObject<RevitObject>());

                        return Task.FromResult(true);
                    },
                    false);
                });

            Manifest = await _indexClient.GetIndexManifest(container, modelSetVersion.ModelSetId, (uint)modelSetVersion.Version);

            var viewableFileObjects = objects
                .Where(o => o.DocumentIds != null && o.DocumentIds.Length > 0)
                .Select(o => new
                {
                    obj = o,
                    urn = Manifest.SeedFiles.Single(f => f.Id == o.File).Urn
                })
                .GroupBy(o => o.urn)
                .ToDictionary(g => g.Key, g => g.Select(i => i.obj).ToArray());

            Debug.WriteLine(JsonConvert.SerializeObject(viewableFileObjects));

            foreach (var obj in viewableFileObjects.Values.SelectMany(arr => arr))
            {
                // TODO: remove GroupBy and Select First when data is fixed.
                obj.ViewableMap =
                    obj.DocumentIds
                    .Select(id => Manifest.SeedFiles.Single(f => f.Id == obj.File).Documents.Single(d => d.Id == id))
                    .ToDictionary(d => d.ViewableId, d => d.Id, StringComparer.OrdinalIgnoreCase);
            }

            return viewableFileObjects;
        }
    }
}
