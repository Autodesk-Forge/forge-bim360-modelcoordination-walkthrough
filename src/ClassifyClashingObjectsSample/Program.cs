/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
using Autodesk.Forge.Bim360.ModelCoordination.Clash;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using Sample.Forge.Coordination;
using Sample.Forge.Data;
using Sample.Forge.Issue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifyClashingObjectsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var configuration = new SampleConfiguration();

            //-----------------------------------------------------------------------------------------------------
            // Sample Configuration
            //-----------------------------------------------------------------------------------------------------
            // Either add a .adsk-forge/SampleConfiguration.json file to the Environment.SpecialFolder.UserProfile
            // folder (this will be different on windows vs mac/linux) or pass the optional configuration
            // Dictionary<string, string> to set AuthToken, Account and Project values on SampleConfiguration
            //
            // configuration.Load(new Dictionary<string, string>
            // {
            //     { "AuthToken", "Your Forge App OAuth token" },
            //     { "AccountId", "Your BIM 360 account GUID (no b. prefix)" },
            //     { "ProjectId", "Your BIM 360 project GUID (no b. prefix)"}
            // });
            // 
            // See: SampleConfigurationExtensions.cs for more information.
            //-----------------------------------------------------------------------------------------------------

            configuration.Load();

            // create a service provider for IoC composition
            var serviceProvider = new ServiceCollection()
                .AddSampleForgeServices(configuration)
                .BuildServiceProvider();

            // load the state from GetClashResultsSample
            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            var clashResultSampleState = await fileManager.ReadJsonAsync<GetClashResultsSampleState>()
                ?? throw new InvalidOperationException("Could not load GetClashResultsSampleState.json, have you run GetClashResultsSample?");

            // get the model set versoin associated with the clash test
            var modelSetClient = serviceProvider.GetRequiredService<IModelSetClient>();

            var modelSetVersion = await modelSetClient.GetModelSetVersionAsync(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.ModelSetId,
                clashResultSampleState.Latest.ModelSetVersion);

            ColourConsole.WriteSuccess($"Loaded model set version {clashResultSampleState.Latest.ModelSetId}:{clashResultSampleState.Latest.ModelSetVersion}");

            // Get the first page of assigned clash groups AKA coordination issues.
            var clashClient = serviceProvider.GetRequiredService<IClashClient>();

            var assignedClashGroups = await clashClient.GetClashTestAssignedClashGroupIntersectionAsync(
                clashResultSampleState.Container, 
                clashResultSampleState.Latest.Id, 
                null, 
                null);

            ColourConsole.WriteSuccess($"Loaded page 1 of asigned clash issues with test context {clashResultSampleState.Latest.Id}");

            // Get the details of the clash groups returned
            var assignedClashGroupDetails = await clashClient.GetAssignedClashGroupBatchAsync(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.Id,
                false,
                assignedClashGroups.Groups.Select(cg => cg.Id));

            ColourConsole.WriteSuccess($"Loaded {assignedClashGroups.Groups.Count} clash groups");

            // get the index manifest and map the document URNs from the clash data to index seed file document keys
            var modelSetIndex = serviceProvider.GetRequiredService<IModelSetIndex>();

            var indexManifest = await modelSetIndex.GetManifest(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.ModelSetId,
                clashResultSampleState.Latest.ModelSetVersion)
                ?? throw new InvalidOperationException($"No indfex manifest found for {clashResultSampleState.Latest.ModelSetVersion}:{clashResultSampleState.Latest.ModelSetVersion}");

            ColourConsole.WriteSuccess($"Loaded index manifest {clashResultSampleState.Latest.ModelSetId}:{clashResultSampleState.Latest.ModelSetVersion}");

            // map of document version URN -> document index key
            var documentObjectIndexIdMap = modelSetVersion.DocumentVersions.ToDictionary(
                doc => doc.VersionUrn,
                doc => new IndexDocumentObjects
                {
                    SeedFileVersionUrn = doc.OriginalSeedFileVersionUrn,
                    DocumentVersionUrn = doc.VersionUrn,
                    IndexFileKey = indexManifest.SeedFiles.Single(sf => sf.Urn.Equals(doc.OriginalSeedFileVersionUrn, StringComparison.OrdinalIgnoreCase)).Id,
                    IndexDocumentKey = indexManifest.SeedFiles.SelectMany(sf => sf.Documents).Single(d => d.VersionUrn.Equals(doc.VersionUrn, StringComparison.OrdinalIgnoreCase)).Id
                },
                StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in documentObjectIndexIdMap)
            {
                ColourConsole.WriteInfo($"File {kvp.Value.IndexFileKey} contains document {kvp.Value.IndexDocumentKey}");
            }
            
            // Grab all of the objects we are intrested in by getting the left and right LMV object IDs from the individual clashes.
            foreach (var clashGroupDetail in assignedClashGroupDetails)
            {
                var documentIdMap = clashGroupDetail.ClashData.Documents.ToDictionary(d => d.Id, d => d.Urn);

                foreach (var clashInstance in clashGroupDetail.ClashData.ClashInstances)
                {
                    documentObjectIndexIdMap[documentIdMap[clashInstance.Ldid]].AddObjectId(clashInstance.Lvid);
                    documentObjectIndexIdMap[documentIdMap[clashInstance.Rdid]].AddObjectId(clashInstance.Rvid);
                }
            }

            var rows = new List<IndexRow>();

            foreach (var kvp in documentObjectIndexIdMap)
            {
                if (kvp.Value.Objects.Count == 0)
                {
                    continue;
                }

                ColourConsole.WriteInfo($"Query file key {kvp.Value.IndexFileKey}, objects {string.Join(',', kvp.Value.Objects)}");

                // get the objects in this seed file using WHERE IN - watch out for ovweflow
                // this might need to be chunked into multiple queries if you have lots of objects...
                string query = $"select * from s3object s where s.file = '{kvp.Value.IndexFileKey}' and s.id in (" + string.Join(',', kvp.Value.Objects) + ")";

                ColourConsole.WriteInfo(query);

                var queryResults = await modelSetIndex.Query(
                    clashResultSampleState.Container,
                    clashResultSampleState.Latest.ModelSetId,
                    clashResultSampleState.Latest.ModelSetVersion, query);

                ColourConsole.WriteSuccess($"Query results downloaded to {queryResults.FullName}");

                // itterate over the results and pull out the rows
                var reader = new IndexResultReader(queryResults, null);

                var summary = await reader.ReadToEndAsync(obj =>
                {
                    rows.Add(obj);
                    return Task.FromResult(true);
                }, false);
            }

            // get the issue container for this project
            var dataClient = serviceProvider.GetRequiredService<IForgeDataClient>();

            dynamic obj = await dataClient.GetProjectAsJObject()
                ?? throw new InvalidOperationException($"Could not load prject {configuration.ProjectId}");

            Guid issueContainer = obj.data.relationships.issues.data.id;

            var issueClient = serviceProvider.GetRequiredService<IForgeIssueClient>();

            foreach (var clashGroupDetail in assignedClashGroupDetails)
            {
                dynamic issue = await issueClient.GetIssue(issueContainer, clashGroupDetail.IssueId);

                ColourConsole.WriteSuccess($"Issue : {issue.data.attributes.title}");

                foreach (var clashInstance in clashGroupDetail.ClashData.ClashInstances)
                {
                    var leftDocument = documentObjectIndexIdMap[clashGroupDetail.ClashData.Documents.Single(d => d.Id == clashInstance.Ldid).Urn];
                    var leftObject = rows.SingleOrDefault(r => r.Id == clashInstance.Lvid && r.DocumentIds.Contains(leftDocument.IndexDocumentKey));

                    var rightDocument = documentObjectIndexIdMap[clashGroupDetail.ClashData.Documents.Single(d => d.Id == clashInstance.Rdid).Urn];
                    var rightObject = rows.SingleOrDefault(r => r.Id == clashInstance.Rvid && r.DocumentIds.Contains(rightDocument.IndexDocumentKey));

                    ColourConsole.WriteInfo($"  {(string)leftObject.Data["p153cb174"]} clashes with {(string)rightObject.Data["p153cb174"]}");
                }
            }
        }
    }
}
