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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetClashResultsSample
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

            // cache some local state for later...
            var state = new GetClashResultsSampleState();

            // load the state from CreateModelSetSample
            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            var modelSetCreateSampleState = await fileManager.ReadJsonAsync<CreateModelSetSampleState>()
                ?? throw new InvalidOperationException("Could not load CreateModelSetSampleState, have you run CreateModelSetSample?");

            state.Container = modelSetCreateSampleState.ModelSet.ContainerId;

            // load the clash tests for the model set we created in  CreateModelSetSample
            // and try and find the latest clash test to the latest model set verison
            var clashClient = serviceProvider.GetRequiredService<IClashClient>();

            var moelSetClashTests = await clashClient.GetModelSetClashTestsAsync(modelSetCreateSampleState.ModelSet.ContainerId, modelSetCreateSampleState.ModelSet.ModelSetId, null, null);

            if (moelSetClashTests.Tests.Count > 0)
            {
                foreach (var test in moelSetClashTests.Tests.OrderBy(t => t.ModelSetVersion))
                {
                    ColourConsole.WriteSuccess($"Test {test.ModelSetId}:{test.ModelSetVersion:00}; status {test.Status}; completed {test.CompletedOn?.ToString("u")}");
                }

                state.Latest = moelSetClashTests.Tests.OrderBy(t => t.ModelSetVersion).Last();
            }
            else
            {
                ColourConsole.WriteInfo($"No clash test result found for {modelSetCreateSampleState.ModelSet.ModelSetId}, try again later.");
            }

            // make sure we can load this clash test
            if (state.HasLatest)
            {
                _ = await clashClient.GetClashTestAsync(modelSetCreateSampleState.ModelSet.ContainerId, state.Latest.Id)
                    ?? throw new InvalidOperationException($"Error getting model set clash test {state.Latest.Id}");
            }
            else
            {
                ColourConsole.WriteInfo("No latest clash test skipping get model set clash test");
            }

            // get the resources for the latest clash test
            if (state.HasLatest)
            {
                state.ResourceCollection = await clashClient.GetClashTestResourcesAsync(modelSetCreateSampleState.ModelSet.ContainerId, state.Latest.Id);

                if (state.ResourceCollection?.Resources.Count > 0)
                {
                    foreach (var res in state.ResourceCollection.Resources)
                    {
                        ColourConsole.WriteSuccess($"Found clash resource {res.Type}");
                    }
                }
                else
                {
                    ColourConsole.WriteInfo($"No resources found for latest clash test {state.Latest.Id}");
                }
            }
            else
            {
                ColourConsole.WriteInfo("No latest clash test skipping get model set clash test resources");
            }

            // download the reources for the latest clash test
            if (state.HasResources)
            {
                foreach (var resource in state.ResourceCollection.Resources)
                {
                    var name = new Uri(resource.Url).Segments.Last();

                    var fout = fileManager.NewPath(name);

                    ColourConsole.WriteInfo($"Download {fout.Name}");

                    await resource.DownloadClashTestResource(fout);

                    state.LocalResourcePaths[resource.Url] = fout;

                    ColourConsole.WriteSuccess($"Downloaded {fout.Name}");
                }
            }

            // Query the results of the clash test
            if (state.HasResources)
            {
                var documentIndexFile = state.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-document.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                var documentIndexReader = new ClashResultReader<ClashDocument>(documentIndexFile, true);

                var documentIndex = new Dictionary<int, string>();

                await documentIndexReader.Read(doc =>
                {
                    documentIndex[doc.Index] = doc.Urn;

                    return Task.FromResult(true);
                });

                var clashResultFile = state.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-clash.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                var clashIndex = new Dictionary<int, Clash>();

                var clashReader = new ClashResultReader<Clash>(clashResultFile, true);

                await clashReader.Read(c =>
                {
                    clashIndex[c.Id] = c;

                    return Task.FromResult(true);
                });

                // Show counts for the different clash statuses 
                foreach (var group in clashIndex.Values.GroupBy(c => c.Status))
                {
                    ColourConsole.WriteSuccess($"Clash count for status {group.Key}: {group.Count()}");
                }

                // get the clsh instance details
                var clashInstanceFile = state.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-clash-instance.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                var clashInstanceReader = new ClashResultReader<ClashInstance>(clashInstanceFile, true);

                var clashInstanceIndex = new Dictionary<int, ClashInstance>();

                await clashInstanceReader.Read(ci =>
                {
                    clashInstanceIndex[ci.ClashId] = ci;

                    return Task.FromResult(true);
                });

                // pick a random clash an view it
                var rnd = new Random(Guid.NewGuid().GetHashCode());

                var clash = clashIndex.Values.Skip(rnd.Next(0, clashIndex.Count - 1)).Take(1).Single();

                ColourConsole.WriteSuccess($"Clash : {clash.Id}");
                ColourConsole.WriteSuccess($"Left Document : {documentIndex[clashInstanceIndex[clash.Id].LeftDocumentIndex]}");
                ColourConsole.WriteSuccess($"Left Stable Object ID : {clashInstanceIndex[clash.Id].LeftStableObjectId}");
                ColourConsole.WriteSuccess($"Left LMV ID : {clashInstanceIndex[clash.Id].LeftLmvObjectId}");
                ColourConsole.WriteSuccess($"Right Document : {documentIndex[clashInstanceIndex[clash.Id].RightDocumentIndex]}");
                ColourConsole.WriteSuccess($"Right Stable Object ID : {clashInstanceIndex[clash.Id].RightStableObjectId}");
                ColourConsole.WriteSuccess($"Right LMV ID : {clashInstanceIndex[clash.Id].RightLmvObjectId}");
            }

            // save the state for use in subsequent samples
            var stateFile = await fileManager.WriteJsonAsync(state);

            ColourConsole.WriteSuccess($"Sample state written to {stateFile.FullName}");
        }
    }
}
