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
    public static class ForgeEntityExtensions
    {
        public static StorageId ToStorageId(this ForgeEntity entity)
        {
            if (entity.Type != ForgeEntityType.StorageObject)
            {
                throw new InvalidOperationException($"{entity.Type} is an not a StorageObject");
            }

            return StorageId.Parse(entity.Id);
        }
    }
}
