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
using Autodesk.Forge.Bim360.ModelCoordination.Index;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sample.Forge.Coordination
{
    internal static class IndexQueryExtensions
    {
        public static string GetThumbprint(this IndexQuery query)
        {
            using (var hashFunction = MD5.Create())
            {
                var hash = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(query.Statement.ToUpperInvariant()));

                var sb = new StringBuilder(2 * hash.Length);

                foreach (byte b in hash)
                {
                    sb.AppendFormat("{0:X2}", b);
                }

                return sb.ToString();
            }
        }
    }
}
