using Autodesk.Forge.Model;
using System.Collections.Generic;

namespace MCSample.Forge
{
    public static class ForgeItemJson
    {
        public static CreateItem CreateFileItem(string storageId, string parentFolderId, string displayName)
        {
            return new CreateItem
            (
                new JsonApiVersionJsonapi
                (
                    JsonApiVersionJsonapi.VersionEnum._0
                ),
                new CreateItemData
                (
                    CreateItemData.TypeEnum.Items,
                    new CreateItemDataAttributes
                    (
                        DisplayName: displayName,
                        new BaseAttributesExtensionObject
                        (
                            Type: "items:autodesk.bim360:File",
                            Version: "1.0"
                        )
                    ),
                    new CreateItemDataRelationships
                    (
                        new CreateItemDataRelationshipsTip
                        (
                            new CreateItemDataRelationshipsTipData
                            (
                                CreateItemDataRelationshipsTipData.TypeEnum.Versions,
                                CreateItemDataRelationshipsTipData.IdEnum._1
                            )
                        ),
                        new CreateStorageDataRelationshipsTarget
                        (
                            new StorageRelationshipsTargetData
                            (
                                StorageRelationshipsTargetData.TypeEnum.Folders,
                                Id: parentFolderId
                            )
                        )
                    )
                ),
                new List<CreateItemIncluded>
                {
                    new CreateItemIncluded
                    (
                        CreateItemIncluded.TypeEnum.Versions,
                        CreateItemIncluded.IdEnum._1,
                        new CreateStorageDataAttributes
                        (
                            displayName,
                            new BaseAttributesExtensionObject
                            (
                                Type:"versions:autodesk.bim360:File",
                                Version:"1.0"
                            )
                        ),
                        new CreateItemRelationships(
                            new CreateItemRelationshipsStorage
                            (
                                new CreateItemRelationshipsStorageData
                                (
                                    CreateItemRelationshipsStorageData.TypeEnum.Objects,
                                    storageId
                                )
                            )
                        )
                    )
                }
            );
        }
    }
}
