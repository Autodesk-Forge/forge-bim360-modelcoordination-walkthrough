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

namespace Sample.Forge.Data
{
    public class ForgeFolderJson
    {
        public static IEnumerable<(string name, string id)> GetFolders(dynamic folders)
        {
            for (int i = 0; i < folders.data.Count; i++)
            {
                if (folders.data[i].type.Equals("folders", StringComparison.OrdinalIgnoreCase))
                {
                    string id = folders.data[i].id;
                    string name = folders.data[i].attributes.name;

                    yield return (name, id);
                }
            }
        }

        public static (string name, string id) SearchFolders(dynamic folders, string name = default, string id = default)
        {
            (string name, string id) res = default;

            foreach ((string fname, string fid) in (IEnumerable<(string, string)>)GetFolders(folders))
            {
                if (!string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(id))
                {
                    if (name.Equals(fname, StringComparison.OrdinalIgnoreCase))
                    {
                        res = (fname, fid);

                        break;
                    }
                }
                else if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(id))
                {
                    if (id.Equals(fid, StringComparison.OrdinalIgnoreCase))
                    {
                        res = (fname, fid);

                        break;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(id))
                {
                    if (name.Equals(fname, StringComparison.OrdinalIgnoreCase) && id.Equals(fid, StringComparison.OrdinalIgnoreCase))
                    {
                        res = (fname, fid);

                        break;
                    }
                }
                else
                {
                    throw new InvalidOperationException("No name or id search parameters supplied!");
                }
            }

            return res;
        }

        public static CreateFolder CreateFolder(string folderName, string parentId)
        {
            return new CreateFolder
            (
                new JsonApiVersionJsonapi
                (
                    JsonApiVersionJsonapi.VersionEnum._0
                ),
                new CreateFolderData
                (
                    new CreateFolderDataAttributes
                    (
                        folderName,
                        new BaseAttributesExtensionObjectWithoutSchemaLink
                        (
                            Type: "folders:autodesk.bim360:Folder",
                            Version: "1.0"
                        )
                    ),
                    new CreateFolderDataRelationships
                    (
                        new CreateFolderDataRelationshipsParent
                        (
                            new CreateFolderDataRelationshipsParentData
                            (
                                Type: "folders",
                                Id: parentId
                            )
                        )
                    )
                )
            );
        }
    }
}
