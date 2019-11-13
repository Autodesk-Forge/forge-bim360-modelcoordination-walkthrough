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

            // Get the first page of assigned clash groups AKA coordination issues.
            var clashClient = serviceProvider.GetRequiredService<IClashClient>();

            var assignedClashGroups = await clashClient.GetClashTestAssignedClashGroupIntersectionAsync(
                clashResultSampleState.Container, 
                clashResultSampleState.Latest.Id, 
                null, 
                null);

            // Get the details of the clash groups returned
            var assignedClashGroupDetails = await clashClient.GetAssignedClashGroupBatchAsync(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.Id,
                false,
                assignedClashGroups.Groups.Select(cg => cg.Id));

            // get a list of the distince documents for all the assigned clash groups in the page
            var documentUrns = assignedClashGroupDetails
                .SelectMany(cg => cg.ClashData.Documents.Select(d => d.Urn))
                .Distinct()
                .ToArray();

            // get the index manifest and map the document URNs from the clash data to index seed file document keys
            var modelSetIndex = serviceProvider.GetRequiredService<IModelSetIndex>();

            var indexManifest = await modelSetIndex.GetManifest(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.ModelSetId,
                clashResultSampleState.Latest.ModelSetVersion)
                ?? throw new InvalidOperationException($"No indfex manifest found for {clashResultSampleState.Latest.ModelSetVersion}:{clashResultSampleState.Latest.ModelSetVersion}");

            var documentIndexIdMap = documentUrns.ToDictionary(
                doc => doc,
                doc => indexManifest.SeedFiles.SelectMany(sf => sf.Documents).Single(d => d.VersionUrn.Equals(doc, StringComparison.OrdinalIgnoreCase)).Id, 
                StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in documentIndexIdMap)
            {
                ColourConsole.WriteInfo($"{kvp.Key} -> {kvp.Value}");
            }
            
            // Grab all of the objects we are potentally intrested in by getting the 
            // left and right LMV object IDs from the individual clashes.
            List<int> candidateObjectIds = new List<int>();

            foreach (var clashGroupDetail in assignedClashGroupDetails)
            {
                foreach (var clashInstance in clashGroupDetail.ClashData.ClashInstances)
                {
                    if (!candidateObjectIds.Contains(clashInstance.Lvid))
                    {
                        candidateObjectIds.Add(clashInstance.Lvid);
                    }

                    if (!candidateObjectIds.Contains(clashInstance.Rvid))
                    {
                        candidateObjectIds.Add(clashInstance.Rvid);
                    }
                }
            }

            ColourConsole.WriteInfo($"Object IDs: {string.Join(',', candidateObjectIds)}");

            // this will potentially get more data than we want, we will have to remove objects which are not
            // in the documents associated with the clash. ie. the IDs in candidateObjectIds could also
            // be in documents which are not participating in the clash intersection. Watch the in clause
            //for overflowing, this will need to be chunked for large data sets
            string query = "select * from s3object s where s.id in (" + string.Join(',', candidateObjectIds) + ") and count(s.docs) > 0";

            ColourConsole.WriteInfo(query);

            var queryResults = await modelSetIndex.Query(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.ModelSetId,
                clashResultSampleState.Latest.ModelSetVersion, query);

            ColourConsole.WriteSuccess($"Query results downloaded to {queryResults.FullName}");

            // itterate over the results and count the number of rows returned
            // substituteFieldkeys == true below will pull out the unique set of
            // fields in this data
            var reader = new IndexResultReader(queryResults, null);

            var rows = new List<IndexRow>();

            var summary = await reader.ReadToEndAsync(obj =>
            {
                rows.Add(obj);
                return Task.FromResult(true);
            }, false);

            foreach (var clashGroupDetail in assignedClashGroupDetails)
            {
                foreach (var clashInstance in clashGroupDetail.ClashData.ClashInstances)
                {
                    var leftDocumentKey = documentIndexIdMap[clashGroupDetail.ClashData.Documents.Single(d => d.Id == clashInstance.Ldid).Urn];

                    var leftObject = rows.SingleOrDefault(r => r.Id == clashInstance.Lvid && r.DocumentIds.Contains(leftDocumentKey));

                    var rightDocumentKey = documentIndexIdMap[clashGroupDetail.ClashData.Documents.Single(d => d.Id == clashInstance.Rdid).Urn];

                    var rightObject = rows.SingleOrDefault(r => r.Id == clashInstance.Rvid && r.DocumentIds.Contains(rightDocumentKey));

                    ColourConsole.WriteSuccess($"{leftObject.Row.ToString()}\r\nclashes with\r\n{rightObject.Row.ToString()}");
                }
            }
        }
    }
}
