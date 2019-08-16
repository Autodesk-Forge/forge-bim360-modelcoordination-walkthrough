using Autodesk.Forge.Model;
using System;
using System.Collections.Generic;

namespace MCSample.Forge
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
