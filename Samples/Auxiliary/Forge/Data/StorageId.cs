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
using System;

namespace Sample.Forge.Data
{
    public class StorageId
    {
        public string Bucket { get; set; }

        public string Key { get; set; }

        public static StorageId Parse(string ossUrn)
        {
            var parts = ossUrn.Split(new char[] { '/' }, 2);

            var resourceParts = parts[0].Split(new[] { ':' }, 4);

            return new StorageId
            {
                Bucket = resourceParts[3],
                Key = parts[1]
            };
        }
    }
}
