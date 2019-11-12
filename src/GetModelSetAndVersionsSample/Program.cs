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
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using Sample.Forge.Coordination;
using Sample.Forge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetModelSetAndVersionsSample
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

            // load the state from CreateModelSetSample
            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            var modelSetCreateSampleState = await fileManager.ReadJsonAsync<CreateModelSetSampleState>();

            ColourConsole.WriteSuccess($"Loaded sample state");

            // make sure this model set exists
            var modelSetClient = serviceProvider.GetRequiredService<IModelSetClient>();

            var modelSet = await modelSetClient.GetModelSetAsync(modelSetCreateSampleState.ModelSet.ContainerId, modelSetCreateSampleState.ModelSet.ModelSetId)
                ?? throw new InvalidOperationException($"Could not find model set {modelSetCreateSampleState.ModelSet.ModelSetId}, have you run CreateModelSetSample?");

            ColourConsole.WriteSuccess($"Found model set {modelSet.ModelSetId}");

            // See if we can find any model set versions, remember there is a time dalay between documents being extracted
            // by BIM 360 Docs and the model coordination system's processing of these extracted documents. If you have jumped
            // immediatly to this sample having run CreateModelSetSample it could be that you are working faster than Docs :-)
            var modelSetVersions = await modelSetClient.GetModelSetVersionsAsync(modelSetCreateSampleState.ModelSet.ContainerId, modelSetCreateSampleState.ModelSet.ModelSetId, null, null);

            if (modelSetVersions.ModelSetVersions.Count > 0)
            {
                foreach (var version in modelSetVersions.ModelSetVersions.OrderBy(v => v.Version))
                {
                    ColourConsole.WriteSuccess($"Found model set version {version.Version:00} : {version.CreateTime.ToString("u")}, {version.Status}");
                }
            }
            else
            {
                ColourConsole.WriteInfo($"No model set verisons found for {modelSetCreateSampleState.ModelSet.ModelSetId}, try again later.");
            }

            // If we found model set versions, demonstrate loading a model set by verison number
            if (modelSetVersions?.ModelSetVersions.Count > 0)
            {
                ColourConsole.WriteInfo("Demonstrate loading model set version by number");

                var version = await modelSetClient.GetModelSetVersionAsync(modelSetCreateSampleState.ModelSet.ContainerId, modelSetCreateSampleState.ModelSet.ModelSetId, 1);

                ColourConsole.WriteSuccess($"First model set version {version.Version:00} : {version.CreateTime.ToString("u")}, {version.Status}");
            }

            if (modelSetVersions?.ModelSetVersions.Count > 0)
            {
                ColourConsole.WriteInfo("Demonstrate loading latest model set version");

                var latest = await modelSetClient.GetModelSetVersionLatestAsync(modelSetCreateSampleState.ModelSet.ContainerId, modelSetCreateSampleState.ModelSet.ModelSetId);

                ColourConsole.WriteSuccess($"Latest model set version {latest.Version:00} : {latest.CreateTime.ToString("u")}, {latest.Status}");
            }
        }
    }
}
