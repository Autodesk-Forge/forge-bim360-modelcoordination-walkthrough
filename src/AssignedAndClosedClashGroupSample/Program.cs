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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AssignedAndClosedClashGroupSample
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        public static async Task RunAsync()
        {
            var configuration = new SampleConfiguration();

            //-----------------------------------------------------------------------------------------------------
            // Sample Configuration
            //-----------------------------------------------------------------------------------------------------
            // Either add a .adsk-forge/SampleConfiguration.json file to the Environment.SpecialFolder.UserProfile
            // folder (this will be different on windows vs mac/linux) or pass the optional configuration
            // Dictionary<string, string> to set AuthToken, Account and Project values on SampleConfiguration
            //
            // configuration.Configure(new Dictionary<string, string>
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

            // get the issue container for this project
            var dataClient = serviceProvider.GetRequiredService<IForgeDataClient>();

            dynamic obj = await dataClient.GetProjectAsJObject()
                ?? throw new InvalidOperationException($"Could not load prject {configuration.ProjectId}");

            Guid issueContainer = obj.data.relationships.issues.data.id;

            ColourConsole.WriteSuccess($"Using issue container {issueContainer}");

            // get the first page of assigned clash groups AKA coordination issues
            // and display their details by calling the BIM 360 issues service
            var clashClient = serviceProvider.GetRequiredService<IClashClient>();

            var issueClient = serviceProvider.GetRequiredService<IForgeIssueClient>();

            var assignedClashGroups = await clashClient.GetClashTestAssignedClashGroupIntersectionAsync(clashResultSampleState.Container, clashResultSampleState.Latest.Id, null, null);

            var assignedClashGroupDetails = await clashClient.GetAssignedClashGroupBatchAsync(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.Id,
                false,
                assignedClashGroups.Groups.Select(cg => cg.Id));

            foreach (var cg in assignedClashGroups.Groups)
            {
                var detail = assignedClashGroupDetails.Single(i => i.Id == cg.Id);

                // https://forge.autodesk.com/en/docs/bim360/v1/reference/http/field-issues-:id-GET/
                dynamic issue = await issueClient.GetIssue(issueContainer, detail.IssueId);

                Console.WriteLine();
                ColourConsole.WriteSuccess($"Issue : {detail.IssueId}");
                ColourConsole.WriteSuccess($"Title : {issue.data.attributes.title}");
                ColourConsole.WriteSuccess($"Status : {issue.data.attributes.status}");
                ColourConsole.WriteSuccess($"Created on : {issue.data.attributes.created_at}");
                ColourConsole.WriteSuccess($"Clash count : {detail.ClashData.Clashes.Count}");
                ColourConsole.WriteSuccess($"Clash document count : {detail.ClashData.Documents.Count}");
                ColourConsole.WriteSuccess($"Clash instance count : {detail.ClashData.ClashInstances.Count}");
                ColourConsole.WriteSuccess($"Existing clash count : {cg?.Existing.Count}");
                ColourConsole.WriteSuccess($"Resolved clash count : {cg?.Resolved.Count}");
            }

            // do the samme for the closed (ignored) clash groups - instead of querying the issue
            // (one does not exist) download the screenshot associated with the closed clash group
            var closedClashGroups = await clashClient.GetClashTestClosedClashGroupIntersectionAsync(clashResultSampleState.Container, clashResultSampleState.Latest.Id, null, null);

            var closedClashGroupDetails = await clashClient.GetClosedClashGroupDataBatchAsync(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.Id,
                closedClashGroups.Groups.Select(cg => cg.Id));

            foreach (var cg in closedClashGroups.Groups)
            {
                var detail = closedClashGroupDetails.Single(i => i.Id == cg.Id);

                Console.WriteLine();
                ColourConsole.WriteSuccess($"Closed : {detail.Title}");
                ColourConsole.WriteSuccess($"Reason : {detail.Reason}");
                ColourConsole.WriteSuccess($"Created on : {detail.CreatedOn}");
                ColourConsole.WriteSuccess($"Clash count : {detail.ClashData.Clashes.Count}");
                ColourConsole.WriteSuccess($"Clash document count : {detail.ClashData.Documents.Count}");
                ColourConsole.WriteSuccess($"Clash instance count : {detail.ClashData.ClashInstances.Count}");
                ColourConsole.WriteSuccess($"Existing clash count : {cg?.Existing.Count}");
                ColourConsole.WriteSuccess($"Resolved clash count : {cg?.Resolved.Count}");

                if (detail?.ScreenShots.Count > 0)
                {
                    var screenShotId = detail.ScreenShots.First();

                    var screenShot = fileManager.NewPath($"{screenShotId}.png");

                    using (var ss = await clashClient.GetScreenShotAsync(clashResultSampleState.Container, clashResultSampleState.Latest.ModelSetId, screenShotId))
                    using (var fout = screenShot.Open(FileMode.Create))
                    {
                        await ss.Stream.CopyToAsync(fout);

                        ColourConsole.WriteSuccess($"First screenshot : {screenShot.FullName}");
                    }
                }
            }
        }
    }
}
