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
using System.Collections.Generic;

namespace Sample.Forge.Data
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
