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
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge;
using System;

namespace MCConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new SampleConfiguration();

            new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build()
                .Bind(config);

            var serviceProvider = new ServiceCollection()
                .AddSampleForgeServices(config)
                .BuildServiceProvider();

            var fileManager = serviceProvider.GetRequiredService<ILocalFileManager>();

            var configFile = fileManager.WriteJson(config);

            ColourConsole.WriteSuccess($"Configuraiton file {configFile.FullName}");
        }
    }
}
