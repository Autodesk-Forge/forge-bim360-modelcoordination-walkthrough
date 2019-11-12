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
using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using Microsoft.Extensions.DependencyInjection;
using Sample.Forge.Auth;
using Sample.Forge.Coordination;
using Sample.Forge.Data;
using Sample.Forge.Issue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSampleForgeServices(this IServiceCollection serviceCollection, SampleConfiguration configuration = null)
        {
            serviceCollection.AddSingleton<ILocalFileManager, LocalFileManager>();

            if (configuration == null)
            {
                serviceCollection.AddSingleton(new SampleConfiguration());
            }
            else
            {
                serviceCollection.AddSingleton(configuration);
            }            

            serviceCollection.AddSingleton<IForgeDataClient, ForgeDataClient>();
            
            serviceCollection.AddSingleton<IForgeDerivativeClient, ForgeDerivativeClient>();
            
            serviceCollection.AddSingleton<IForgeIssueClient, ForgeIssueClient>();

            serviceCollection.AddTransient<SampleConfigurationDelegatingHandler>();

            serviceCollection.AddHttpClient<IIndexClient, IndexClient>(options => options.BaseAddress = configuration.ModelSetIndexApiBasePath)
                .AddSampleConfigurationDelegatingHandler();

            serviceCollection.AddHttpClient<IModelSetClient, ModelSetClient>(options => options.BaseAddress = configuration.ModelSetApiBasePath)
                .AddSampleConfigurationDelegatingHandler();

            serviceCollection.AddHttpClient<IClashClient, ClashClient>(options => options.BaseAddress = configuration.ModelSetClashApiBasePath)
                .AddSampleConfigurationDelegatingHandler();

            serviceCollection.AddSingleton<ITokenManager, TokenManager>();

            serviceCollection.AddSingleton<IIndexFieldCache, IndexFieldCache>();

            serviceCollection.AddSingleton<IModelSetIndex, ModelSetIndex>();

            return serviceCollection;
        }
    }
}
