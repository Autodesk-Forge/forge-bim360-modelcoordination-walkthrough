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

namespace CreateAndQueryViewsSample
{
    class Program
    {
        private static class SampleModelSetView
        {
            public const string Name = "Forge API Sample View";
            public const string Description = "This view was created by the Model Coordination Forge sample .NET core ModelSetViews app to demonstrate API view creation.";
        }

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

            var modelSetCreateSampleState = await fileManager.ReadJsonAsync<CreateModelSetSampleState>()
                ?? throw new InvalidOperationException("Could not load CreateModelSetSampleState, have you run CreateModelSetSample?");

            // get the first page of model set views
            var modelSetClient = serviceProvider.GetRequiredService<IModelSetClient>();

            ModelSetView sampleView = null;

            bool createSampleView = true;

            var views = await modelSetClient.GetModelSetViewsAsync(
                containerId: modelSetCreateSampleState.ModelSet.ContainerId, 
                modelSetId: modelSetCreateSampleState.ModelSet.ModelSetId, 
                pageLimit: null,
                continuationToken: null,
                createdBy: null,
                modifiedBy: null,
                after: null,
                before: null,
                isPrivate: null, 
                sortBy: null,
                sortDirection: null);

            if (views?.ModelSetViews.Count > 0)
            {
                foreach (var view in views.ModelSetViews)
                {
                    if (view.Name.Equals(SampleModelSetView.Name))
                    {
                        createSampleView = false;

                        sampleView = view;

                        ColourConsole.WriteInfo($"Found sample view, {view.Name}, is private:{view.IsPrivate}");
                    }
                    else
                    {
                        ColourConsole.WriteInfo($"Found, {view.Name}, is private:{view.IsPrivate}");
                    }
                }
            }
            else
            {
                ColourConsole.WriteInfo($"No views found for model set {modelSetCreateSampleState.ModelSet.ModelSetId}, create sample...");
            }

            // if the sample view does not exist create it using the lineages we
            // lave on the current tip verison of the model set
            ModelSetVersion modelSetTipVersion = await modelSetClient.GetModelSetVersionLatestAsync(
                modelSetCreateSampleState.ModelSet.ContainerId, 
                modelSetCreateSampleState.ModelSet.ModelSetId);

            if (createSampleView)
            {
                if (modelSetTipVersion != null && modelSetTipVersion?.DocumentVersions.Count > 1)
                {
                    var lineages = modelSetTipVersion.DocumentVersions
                        .Where(v => v.DocumentStatus == ModelSetDocumentStatus.Succeeded)
                        .Take(2)
                        .Select(d => d.DocumentLineage).ToList();

                    var modelSetView = new NewModelSetView
                    {
                        Name = SampleModelSetView.Name,
                        Description = SampleModelSetView.Description,
                        IsPrivate = true,
                        Definition = lineages
                            .Select(
                                l => new LineageViewable
                                {
                                    LineageUrn = l.LineageUrn
                                })
                            .ToList()
                    };

                    // this extension method wraps the job polling
                    sampleView = await modelSetClient.CreateModelSetView(
                        modelSetCreateSampleState.ModelSet.ContainerId, 
                        modelSetCreateSampleState.ModelSet.ModelSetId, 
                        modelSetView);

                    ColourConsole.WriteSuccess($"Created sample view: {sampleView.Name}, is private: {sampleView.IsPrivate}");
                }
                else
                {
                    ColourConsole.WriteWarning($"No model set version with >= 2 lineages in {modelSetCreateSampleState.ModelSet.ModelSetId}, skip sample view create!");
                }
            }

            // if we have a sample view toggle its privacy flag. If the view was created above ^^
            // then this should make the sample view public, otherwise it will become private
            if (sampleView != null)
            {
                // this extension method wraps the job polling
                sampleView = await modelSetClient.UpdateModelSetView(
                    modelSetCreateSampleState.ModelSet.ContainerId,
                    modelSetCreateSampleState.ModelSet.ModelSetId,
                    sampleView.ViewId,
                    new UpdateModelSetView
                    {
                        OldIsPrivate = sampleView.IsPrivate,
                        NewIsPrivate = !sampleView.IsPrivate
                    });

                ColourConsole.WriteSuccess($"Toggled sample view is private: {sampleView.IsPrivate}");
            }

            // finally if the sample view exists, instance it against the tip
            if (sampleView != null)
            {
                var viewVersion = await modelSetClient.GetModelSetViewVersionAsync(
                    modelSetCreateSampleState.ModelSet.ContainerId,
                    modelSetTipVersion.ModelSetId,
                    modelSetTipVersion.Version,
                    sampleView.ViewId);

                foreach (var doc in viewVersion.DocumentVersions)
                {
                    ColourConsole.WriteSuccess($"View member {doc.DisplayName} => {doc.VersionUrn}");
                }
            }
        }
    }
}
