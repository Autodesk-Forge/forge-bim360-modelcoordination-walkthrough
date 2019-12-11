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
using System.IO;
using System.Threading.Tasks;

namespace Sample.Forge.Data
{
    public interface IForgeDataClient : IForgeClient
    {
        Task<dynamic> GetProjectAsJObject();

        Task<Project> GetProject();

        Task<ForgeEntity> FindTopFolderByName(string name);

        Task<ForgeEntity> FindFolderByName(string parentFolderId, string folderName);

        Task<ForgeEntity> CreateFolder(string parentFolderId, string folderName);

        Task<ForgeEntity> CreateOssStorage(string folderId, string storageName);

        Task<UploadResult> Upload(FileInfo file, ForgeEntity storage);

        Task CreateItem(string folderId, string storageObject, string itemName);
    }
}
