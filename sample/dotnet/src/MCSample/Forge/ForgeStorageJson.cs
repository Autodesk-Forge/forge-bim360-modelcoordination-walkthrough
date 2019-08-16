using Autodesk.Forge.Model;

namespace MCSample.Forge
{
    public static class ForgeStorageJson
    {
        public static CreateStorage CreateStorage(string fileName, string folderId)
        {
            return new CreateStorage
            (
                new JsonApiVersionJsonapi
                (
                    JsonApiVersionJsonapi.VersionEnum._0
                ),
                new CreateStorageData
                (
                    CreateStorageData.TypeEnum.Objects,
                    new CreateStorageDataAttributes
                    (
                        fileName,
                        new BaseAttributesExtensionObject
                        (
                            "items:autodesk.core:File",
                            "1.0",
                            null,
                            null
                        )
                    ),
                    new CreateStorageDataRelationships
                    (
                        new CreateStorageDataRelationshipsTarget
                        (
                            new StorageRelationshipsTargetData
                            (
                                StorageRelationshipsTargetData.TypeEnum.Folders,
                                folderId
                            )
                        )
                    )
                )
            );
        }
    }
}
