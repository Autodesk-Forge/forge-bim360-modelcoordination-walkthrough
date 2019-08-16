using Autodesk.Forge.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MCSample.Forge
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
