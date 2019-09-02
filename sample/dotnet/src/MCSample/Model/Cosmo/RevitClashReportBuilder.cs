using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{
    [Export(typeof(IRevitClashReportBuilder))]
    public class RevitClashReportBuilder : IRevitClashReportBuilder
    {
        private readonly IForgeModelSetClient _modelSetClient;
        private readonly IForgeClashClient _clashClient;
        private readonly IRevitObjectQuery _revitQuery;

        [ImportingConstructor]
        public RevitClashReportBuilder(IForgeModelSetClient modelSetClient, IForgeClashClient clashClient, IRevitObjectQuery revitQuery)
        {
            _modelSetClient = modelSetClient ?? throw new System.ArgumentNullException(nameof(modelSetClient));
            _clashClient = clashClient ?? throw new System.ArgumentNullException(nameof(clashClient));
            _revitQuery = revitQuery ?? throw new System.ArgumentNullException(nameof(revitQuery));
        }

        public async Task<IReadOnlyCollection<RevitClashReport>> Build(Guid container, Guid modelSetId, uint verison, Guid clashTestId)
        {
            List<RevitClashReport> reportSet = new List<RevitClashReport>();

            Console.WriteLine("  Get model set version");

            var modelSetVersion = await _modelSetClient.GetModelSetVersion(container, modelSetId, verison);

            Debug.WriteLine(JsonConvert.SerializeObject(modelSetVersion, Formatting.Indented));

            Console.WriteLine("  Get clash test");

            var clashTest = await _clashClient.GetModelSetClashTest(container, clashTestId);

            Debug.WriteLine(JsonConvert.SerializeObject(clashTest, Formatting.Indented));

            Console.WriteLine("  Get clash test resources");

            var clashTestResources = await _clashClient.GetModelSetClashTestResources(container, clashTestId);

            Debug.WriteLine(JsonConvert.SerializeObject(clashTestResources, Formatting.Indented));

            Console.WriteLine("  Get clash test document index resource");

            var documentIndex = (await GetDocumentIndex(clashTestResources.Resources.Where(r => r.Type == "scope-version-document.2.0.0").Single()))
                .ToDictionary(i => i.Key, v => new ViewableDocument
                {
                    Index = v.Key,
                    VersionUrn = v.Value,
                    ViewableId = modelSetVersion.DocumentVersions.SingleOrDefault(d => d.VersionUrn.Equals(v.Value))?.ViewableId,
                    SeedFileUrn = modelSetVersion.DocumentVersions.SingleOrDefault(d => d.VersionUrn.Equals(v.Value))?.OriginalSeedFileVersionUrn
                });

            Debug.WriteLine(JsonConvert.SerializeObject(documentIndex, Formatting.Indented));

            Console.WriteLine("  Get clash test clash index resource");

            var clashes = await GetClashes(clashTestResources.Resources.Where(r => r.Type == "scope-version-clash.2.0.0").Single());

            Debug.WriteLine(JsonConvert.SerializeObject(clashes, Formatting.Indented));

            Console.WriteLine("  Get clash test clash instance resource");

            var clashInstances = await GetClashInstances(clashTestResources.Resources.Where(r => r.Type == "scope-version-clash-instance.2.0.0").Single());

            Debug.WriteLine(JsonConvert.SerializeObject(clashInstances, Formatting.Indented));

            Console.WriteLine("  Query revit objects in model set version");

            var revitObjects = await _revitQuery.GetRevitObjects(container, modelSetVersion);

            Debug.WriteLine(JsonConvert.SerializeObject(revitObjects, Formatting.Indented));

            foreach (var clash in clashes.Values)
            {
                var clashInstance = clashInstances[clash.Id];

                var report = new RevitClashReport
                {
                    Container = container
                };

                reportSet.Add(
                    report.ForModelSetVersion(modelSetVersion)
                      .ForClash(clash)
                      .ForClashTest(clashTest)
                      .ForClashInstance(clashInstance, documentIndex)
                      .WithRevitData(revitObjects, documentIndex));
            }

            return reportSet;
        }

        private async Task<IReadOnlyDictionary<int, ClashInstance>> GetClashInstances(ClashTestResource resource)
        {
            var instances = new Dictionary<int, ClashInstance>();

            await _clashClient.DownloadClashTestResource(
                resource,
                async stream =>
                {
                    var instanceReader = new ClashResultReader<ClashInstance>(stream, false);

                    await instanceReader.Read(ci =>
                    {
                        instances[ci.ClashId] = ci;

                        return Task.FromResult(true);
                    });
                },
                true);

            return instances;
        }

        private async Task<IReadOnlyDictionary<int, Clash>> GetClashes(ClashTestResource resource)
        {
            var clashes = new Dictionary<int, Clash>();

            await _clashClient.DownloadClashTestResource(
                resource,
                async stream =>
                {
                    var clashReader = new ClashResultReader<Clash>(stream, false);

                    await clashReader.Read(c =>
                    {
                        clashes[c.Id] = c;

                        return Task.FromResult(true);
                    });
                },
                true);

            return clashes;
        }

        private async Task<IReadOnlyDictionary<int, string>> GetDocumentIndex(ClashTestResource resource)
        {
            var documentIndex = new Dictionary<int, string>();

            await _clashClient.DownloadClashTestResource(
                resource,
                async stream =>
                {
                    var documentIndexReader = new ClashResultReader<ClashDocument>(stream, false);

                    await documentIndexReader.Read(doc =>
                    {
                        documentIndex[doc.Index] = doc.Urn;

                        return Task.FromResult(true);
                    });
                },
                true);

            return documentIndex;
        }
    }
}
