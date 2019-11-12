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

namespace CreateModelSetSample
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

            var state = new CreateModelSetSampleState();

            // find the project files folder
            var forgeDataClient = serviceProvider.GetRequiredService<IForgeDataClient>();

            state.PlansFolder = await forgeDataClient.FindTopFolderByName("Plans") ?? throw new InvalidOperationException("Could not find Plans folder!");

            ColourConsole.WriteSuccess($"Found project plans folder {state.PlansFolder.Id}");

            // try and find the sample root folder MC_SAMPLE in the root of Plans, if not found create it.
            state.TestFolderRoot = await forgeDataClient.FindFolderByName(state.PlansFolder.Id, CreateModelSetSampleState.MC_SAMPLE_FOLDER_NAME);

            if (state.TestFolderRoot == null)
            {
                state.TestFolderRoot = await forgeDataClient.CreateFolder(state.PlansFolder.Id, CreateModelSetSampleState.MC_SAMPLE_FOLDER_NAME)
                    ?? throw new InvalidOperationException($"Create {CreateModelSetSampleState.MC_SAMPLE_FOLDER_NAME} failed!");
            }
            
            // Now that the MC sample root folder has been created make a new folder beneath this folder for this run
            // of this console application. This is just a MC_{UTD-DATA-NOW} folder. This is not a convention for
            // Model Coordination in general, it is just how this sample code is laid out.
            var testFolderName = $"MC_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";

            state.TestFolder = await forgeDataClient.CreateFolder(state.TestFolderRoot.Id, testFolderName) 
                ?? throw new InvalidOperationException($"Create {testFolderName} failed!");

            ColourConsole.WriteSuccess($"Created test folder {testFolderName}");

            // Find the models that this sample code uploads into the test folder created above. This will trigger the 
            // BIM 360 Docs extraction workflow for these files. The upload process has two phases, (1) sending the bytes
            // to OSS (Autodesk cloud bucket storage) and (2) registerig these OSS resources with Forge-DM to make them
            // accessable via BIM 360 Docs.
            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            state.Uploads.Add(new ForgeUpload
            {
                File = fileManager.GetPathToSampleModelFile(SampleModelSet.Audubon.V1, "Audubon_Architecture.rvt")
            });

            state.Uploads.Add(new ForgeUpload
            {
                File = fileManager.GetPathToSampleModelFile(SampleModelSet.Audubon.V1, "Audubon_Structure.rvt")
            });

            state.Uploads.Add(new ForgeUpload
            {
                File = fileManager.GetPathToSampleModelFile(SampleModelSet.Audubon.V1, "Audubon_Mechanical.rvt")
            });

            // For each file :-
            // - Create an OSS storage location
            // - Uplaod the files to OSS in chunks
            // - Create the Item(and Version) representing the bytes via Forge - DM
            foreach (var upload in state.Uploads)
            {
                ColourConsole.WriteInfo($"Upload {upload.File.FullName}");

                upload.Storage = await forgeDataClient.CreateOssStorage(state.TestFolder.Id, upload.File.Name) 
                    ?? throw new InvalidOperationException($"Crete OSS storage object for {upload.File.Name} failed!");

                upload.Result = await forgeDataClient.Upload(upload.File, upload.Storage);

                await forgeDataClient.CreateItem(state.TestFolder.Id, upload.Storage.Id, upload.File.Name);

                ColourConsole.WriteSuccess($"Uploaded {upload.File.Name}");
            }

            // create a model set for the new folder
            var modelSetClient = serviceProvider.GetRequiredService<IModelSetClient>();

            state.ModelSet = await modelSetClient.CreateModelSet(
                configuration.ProjectId,
                state.TestFolder.Name,
                new ModelSetFolder[]
                {
                    new ModelSetFolder
                    {
                        FolderUrn = state.TestFolder.Id
                    }
                });

            ColourConsole.WriteSuccess($"Created model set {state.ModelSet.ModelSetId}");

            // save the state for use in subsequent samples
            var stateFile = await fileManager.WriteJsonAsync(state);

            ColourConsole.WriteSuccess($"Sample state written to {stateFile.FullName}");
        }
    }
}

