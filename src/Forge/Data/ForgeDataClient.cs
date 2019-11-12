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
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sample.Forge.Data
{
    internal sealed class ForgeDataClient : ForgeClientBase, IForgeDataClient
    {
        public ForgeDataClient(SampleConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<dynamic> GetProjectAsJObject()
        {
            var api = NewForgeApi<ProjectsApi>();

            if (string.IsNullOrWhiteSpace(Configuration.ForgeBimHubId))
            {
                throw new ArgumentNullException("The BIM 360 hub (account) GUID has not been set via MConfig!");
            }

            if (string.IsNullOrWhiteSpace(Configuration.ForgeBimProjectId))
            {
                throw new ArgumentNullException("The BIM 360 project GUID has not been set via MConfig!");
            }

            var project = await api.GetProjectAsync(Configuration.ForgeBimHubId, Configuration.ForgeBimProjectId);

            return JObject.Parse(JsonConvert.SerializeObject(project));
        }

        public async Task<Project> GetProject()
        {
            var api = NewForgeApi<ProjectsApi>();

            if (string.IsNullOrWhiteSpace(Configuration.ForgeBimHubId))
            {
                throw new ArgumentNullException("The BIM 360 hub (account) GUID has not been set via MConfig!");
            }

            if (string.IsNullOrWhiteSpace(Configuration.ForgeBimProjectId))
            {
                throw new ArgumentNullException("The BIM 360 project GUID has not been set via MConfig!");
            }

            return await CallService<Project>(async () => await api.GetProjectAsync(Configuration.ForgeBimHubId, Configuration.ForgeBimProjectId));
        }

        public async Task<ForgeEntity> FindTopFolderByName(string name)
        {
            var projectApi = NewForgeApi<ProjectsApi>();

            var folders = await CallService<TopFolders>(
                async () => await projectApi.GetProjectTopFoldersAsync(
                    Configuration.ForgeBimHubId,
                    Configuration.ForgeBimProjectId));

            var folder = folders.Data.Single(f => f.Attributes.Name.Equals(name));

            return new ForgeEntity
            {
                Id = folder.Id,
                Name = name,
                Type = ForgeEntityType.Folder
            };
        }

        public async Task<ForgeEntity> FindFolderByName(string parentFolderId, string folderName)
        {
            ForgeEntity entity = null;

            var foldersApi = NewForgeApi<FoldersApi>();

            dynamic folders = await foldersApi.GetFolderContentsAsync(Configuration.ForgeBimProjectId, parentFolderId, new string[] { "folders" }.ToList());

            (string name, string id) folder = ForgeFolderJson.SearchFolders(folders, folderName);

            if (!string.IsNullOrWhiteSpace(folder.id))
            {
                entity = new ForgeEntity
                {
                    Type = ForgeEntityType.Folder,
                    Id = folder.id,
                    Name = folder.name
                };
            }

            return entity;
        }

        public async Task<ForgeEntity> CreateFolder(string parentFolderId, string folderName)
        {
            ForgeEntity entity = null;

            var foldersApi = NewForgeApi<FoldersApi>();

            var folder = ForgeFolderJson.CreateFolder(folderName, parentFolderId);

            var response = await foldersApi.PostFolderAsyncWithHttpInfo(Configuration.ForgeBimProjectId, folder);

            if (response.StatusCode == 201)
            {
                var newFolderLocation = new Uri(response.LocationHeader());

                entity = new ForgeEntity
                {
                    Type = ForgeEntityType.Folder,
                    Id = newFolderLocation.Segments.Last(),
                    Name = folderName
                };
            }

            return entity;
        }

        public async Task<ForgeEntity> CreateOssStorage(string folderId, string storageName)
        {
            ForgeEntity entity = null;

            var projectApi = NewForgeApi<ProjectsApi>();

            var storageObject = ForgeStorageJson.CreateStorage(storageName, folderId);

            var resp = await projectApi.PostStorageAsyncWithHttpInfo(Configuration.ForgeBimProjectId, storageObject);

            if (resp.StatusCode == 201)
            {
                entity = new ForgeEntity
                {
                    Type = ForgeEntityType.StorageObject,
                    Id = resp.Data.data.id,
                    Name = storageName
                };
            }

            return entity;
        }

        public async Task<UploadResult> Upload(FileInfo file, ForgeEntity storage)
        {
            var objectApi = NewForgeApi<ObjectsApi>();

            var storageId = storage.ToStorageId();

            var bucketKey = HttpUtility.UrlEncode(storageId.Bucket);
            var objectName = HttpUtility.UrlEncode(storageId.Key);
            var contentDisposition = file.GetFileContentDisposition().ToString();
            var sessionId = Guid.NewGuid().ToString();

            var chunks = await file.GetContentChunks();

            var uploadTasks = new List<Task<ApiResponse<dynamic>>>();

            try
            {
                foreach (var chunk in chunks)
                {
                    uploadTasks.Add(Task.Run(() =>
                    {
                        Debug.Write($"Upload chunk, {chunk.Range}");

                        var contentLength = (int)chunk.Content.Length;
                        var contentRange = chunk.Range;
                        var body = chunk.Content;

                        return objectApi.UploadChunkAsyncWithHttpInfo(bucketKey, objectName, contentLength, contentRange, sessionId, body, contentDisposition);
                    }));
                }

                Task.WaitAll(uploadTasks.ToArray());

                var successTask = uploadTasks.Single(t => t.Result.StatusCode == 200);

                return JsonConvert.DeserializeObject<UploadResult>(JsonConvert.SerializeObject(successTask.Result.Data));
            }
            finally
            {
                foreach (var chunk in chunks)
                {
                    chunk.Dispose();
                }
            }
        }

        public async Task CreateItem(string folderId, string storageObject, string itemName)
        {
            var itemsApi = NewForgeApi<ItemsApi>();

            var item = ForgeItemJson.CreateFileItem(storageObject, folderId, itemName);

            var response = await itemsApi.PostItemAsyncWithHttpInfo(Configuration.ForgeBimProjectId, item);
        }
    }
}
