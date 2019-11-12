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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge.Coordination
{
    public static class ModelSetClientExtensions
    {
        public static async Task<ModelSet> CreateModelSet(this IModelSetClient client, Guid containerId, string name, ModelSetFolder[] folders)
        {
            var status = await client.CreateModelSetAsync(
                containerId,
                new NewModelSet
                {
                    ModelSetId = Guid.NewGuid(),
                    Name = name,
                    Description = $"This model set has been created to demonstrate the Model Coordination API and encapsulates {name}",
                    Folders = folders.ToList()
                }).CompleteJob(
                    job => client.GetModelSetJobAsync(containerId, job.ModelSetId, job.JobId),
                    job => job.Status == ModelSetJobStatus.Running);

            if (status.Status != ModelSetJobStatus.Succeeded)
            {
                throw new InvalidOperationException(JsonConvert.SerializeObject(status));
            }

            return await client.GetModelSetAsync(containerId, status.ModelSetId);
        }
    }
}
