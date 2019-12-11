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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Sample.Forge
{
    public static class SampleConfigurationExtensions
    {
        public static Uri ModelDerivativePath(this SampleConfiguration config, string path) => new Uri(config.ModelDerivativeBasePath, path);

        public static Uri IssueManagementPath(this SampleConfiguration config, string path) => new Uri(config.IssueManagementBasePath, path);

        public static void Load(this SampleConfiguration configuration, IDictionary<string, string> settings = null)
        {
            var configPath = new LocalFileManager().GetJsonPath<SampleConfiguration>();

            if (!configPath.Exists && settings == null)
            {
                throw new InvalidOperationException($"Either add {configPath.FullName} or pass a Dictionary<string, string> with Account, Project and AuthToken set.");
            }

            if (configPath.Exists)
            {
                var target = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "configuration.json");

                configPath.CopyTo(target, true);
            }

            new ConfigurationBuilder()
                .AddJsonFile("configuration.json", true)
                .AddInMemoryCollection(settings)
                .Build()
                .Bind(configuration);
        }
    }
}
