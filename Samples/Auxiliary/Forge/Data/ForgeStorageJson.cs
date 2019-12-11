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

namespace Sample.Forge.Data
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
