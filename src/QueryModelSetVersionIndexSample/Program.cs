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
using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using Sample.Forge.Coordination;
using Sample.Forge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryModelSetVersionIndexSample
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

            // load the state from GetClashResultsSample this includes the latest
            // model set version that was associated with the clash test. It means
            // the results of this query relate to the clashing objects in the 
            // proceeding sample, GetClashresultsSample
            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            var clashResultSampleState = await fileManager.ReadJsonAsync<GetClashResultsSampleState>()
                ?? throw new InvalidOperationException("Could not load GetClashResultsSampleState.json, have you run GetClashResultsSample?");

            // load the model set verison from the GetClashresultsSample
            var modelSetClient = serviceProvider.GetRequiredService<IModelSetClient>();

            var modelSetVersion = await modelSetClient.GetModelSetVersionAsync(
                clashResultSampleState.Container, clashResultSampleState.Latest.ModelSetId, clashResultSampleState.Latest.ModelSetVersion)
                ?? throw new InvalidOperationException($"Error could not load latest model set version for model set {clashResultSampleState.Latest.ModelSetId}, have you run GetClashResultsSample?");

            // next load the index manifest for the model set verison
            // this object contains the look-up values for the seed files
            // documents and databases which were used to build the index. You
            // will need these valuse to display query results in the LMV if
            // they are viewable in 3D
            var modelSetIndex = serviceProvider.GetRequiredService<IModelSetIndex>();

            var indexManifest = await modelSetIndex.GetManifest(
                clashResultSampleState.Container,
                clashResultSampleState.Latest.ModelSetId,
                clashResultSampleState.Latest.ModelSetVersion)
                ?? throw new InvalidOperationException($"No indfex manifest found for {clashResultSampleState.Latest.ModelSetVersion}:{clashResultSampleState.Latest.ModelSetVersion}");

            ColourConsole.WriteSuccess($"Manifest contains {indexManifest.SeedFiles.Count} seed files.");

            // load the fields for the index and go after the field with a name == "name" and category == "__name__"
            var fields = await modelSetIndex.GetFields(clashResultSampleState.Container, clashResultSampleState.Latest.ModelSetId, clashResultSampleState.Latest.ModelSetVersion);

            ColourConsole.WriteSuccess($"Loaded {fields.Count} unique index fields");

            var nameField = fields.Values.SingleOrDefault(
                f => f.Name.Equals("name", StringComparison.OrdinalIgnoreCase) &&
                f.Category.Equals("__name__", StringComparison.OrdinalIgnoreCase));

            // run a query to find all the objects which have a name as defined by (name == "name" && category == "__name__")
            // the not missing S3 select keywords assert that this projection is in the data
            string query = $"select s.file, s.db, s.docs, s.id, s.{nameField.Key} from s3object s where s.{nameField.Key} is not missing";

            ColourConsole.WriteInfo(query);

            var queryResults = await modelSetIndex.Query(
                clashResultSampleState.Container, 
                clashResultSampleState.Latest.ModelSetId, 
                clashResultSampleState.Latest.ModelSetVersion, query);

            ColourConsole.WriteSuccess($"Query results downloaded to {queryResults.FullName}");

            // itterate over the results and count the number of rows returned
            // substituteFieldkeys == true below will pull out the unique set of
            // fields in this data
            var reader = new IndexResultReader(queryResults, fields);

            var summary = await reader.ReadToEndAsync(null, true);

            ColourConsole.WriteSuccess($"Query returned {summary.RowCount} objects with {summary.Fields.Count} unique fields.");
        }
    }
}
