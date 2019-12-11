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
using Autodesk.Forge.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Forge
{
    internal abstract class ForgeClientBase : IForgeClient
    {
        protected ForgeClientBase(SampleConfiguration configuraiton) => Configuration = configuraiton;

        public SampleConfiguration Configuration { get; }

        public T NewForgeApi<T>()
        {
            var apiType = typeof(T);

            var instance = apiType.GetConstructor(new[] { typeof(string) }).Invoke(new object[] { Configuration.BasePath.AbsoluteUri });

            var configProp = apiType.GetProperty("Configuration");

            var config = configProp.GetValue(instance);

            var atProp = config.GetType().GetProperty("AccessToken");

            atProp.SetValue(config, Configuration.AuthToken);

            return (T)instance;
        }

        protected async Task<T> CallService<T>(Func<Task<DynamicJsonResponse>> serviceCall)
        {
            DynamicJsonResponse res = await serviceCall();

            return res.ToObject<T>();
        }

        protected IDictionary<string, string> CreateDefaultHttpRequestHeaders(bool contentTypeJsonApi = false)
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Configuration.AuthToken}" }
            };

            if (contentTypeJsonApi)
            {
                headers.Add("Content-Type", "application/vnd.api+json");
            }

            return headers;
        }
    }
}
