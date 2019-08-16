using Autodesk.Nucleus.Clash.Entities.V3;
using Autodesk.Nucleus.Scopes.Entities.V3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MCSample.Model.Cosmo
{
    public static class RevitClashReportExtensions
    {
        public static RevitClashReport ForModelSetVersion(this RevitClashReport report, ModelSetVersion modelSetVersion)
        {
            report.ModelSetId = modelSetVersion.ModelSetId;
            report.Version = modelSetVersion.Version;

            return report;
        }

        public static RevitClashReport ForClashTest(this RevitClashReport report, ClashTest test)
        {
            report.Test = test.Id;
            report.TestDate = test.CompletedOn.Value.UtcDateTime;

            return report;
        }

        public static RevitClashReport ForClash(this RevitClashReport report, Clash test)
        {
            report.ClashId = test.Id.ToString();
            report.Distance = test.Distance;

            return report;
        }

        public static RevitClashReport ForClashInstance(this RevitClashReport report, ClashInstance instance, IReadOnlyDictionary<int, ViewableDocument> documentMap)
        {
            report.LeftDocument = documentMap[instance.LeftDocumentIndex].VersionUrn;
            report.LeftLmvId = instance.LeftLmvObjectId;
            report.LeftSid = instance.LeftStableObjectId;

            report.RightDocument = documentMap[instance.RightDocumentIndex].VersionUrn;
            report.RightLmvId = instance.RightLmvObjectId;
            report.RightSid = instance.RightStableObjectId;

            return report;
        }

        public static RevitClashReport WithRevitData(this RevitClashReport report, IReadOnlyDictionary<string, RevitObject[]> revitObjects, IReadOnlyDictionary<int, ViewableDocument> documentMap)
        {
            var ldoc = documentMap.Values.Single(d => report.LeftDocument.Equals(d.VersionUrn, StringComparison.OrdinalIgnoreCase));

            if (revitObjects.ContainsKey(ldoc.SeedFileUrn))
            {
                var leftObject = revitObjects[ldoc.SeedFileUrn]
                    .SingleOrDefault(o =>
                        o.ObjectId == report.LeftLmvId &&
                        o.ViewableMap.ContainsKey(ldoc.ViewableId));

                if (leftObject != null)
                {
                    report.LeftName = leftObject.Name;
                    report.LeftCategory = leftObject.Category;
                    report.LeftFamily = leftObject.Family;
                    report.LeftType = leftObject.Type;

                    Debug.WriteLine($"Found {ldoc.SeedFileUrn}, LmvId {report.LeftLmvId} and Viewable {ldoc.ViewableId}");
                }
                else
                {
                    Debug.WriteLine($"No object for {ldoc.SeedFileUrn}, LmvId {report.LeftLmvId} and Viewable {ldoc.ViewableId}");
                }
            }
            else
            {
                Debug.WriteLine($"No objects for seed file {ldoc.SeedFileUrn}");
            }

            var rdoc = documentMap.Values.Single(d => report.RightDocument.Equals(d.VersionUrn, StringComparison.OrdinalIgnoreCase));

            if (revitObjects.ContainsKey(rdoc.SeedFileUrn))
            {
                var rightObject = revitObjects[rdoc.SeedFileUrn]
                    .SingleOrDefault(o =>
                        o.ObjectId == report.RightLmvId &&
                        o.ViewableMap.ContainsKey(rdoc.ViewableId));

                if (rightObject != null)
                {
                    report.RightName = rightObject.Name;
                    report.RightCategory = rightObject.Category;
                    report.RightFamily = rightObject.Family;
                    report.RightType = rightObject.Type;

                    Debug.WriteLine($"Found {rdoc.SeedFileUrn}, LmvId {report.RightLmvId} and Viewable {rdoc.ViewableId}");
                }
                else
                {
                    Debug.WriteLine($"No object for {rdoc.SeedFileUrn}, LmvId {report.RightLmvId} and Viewable {rdoc.ViewableId}");
                }
            }
            else
            {
                Debug.WriteLine($"No objects for seed file {rdoc.SeedFileUrn}");
            }

            return report;
        }
    }
}

