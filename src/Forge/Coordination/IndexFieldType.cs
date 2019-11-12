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
using System.Collections.Generic;
using System.Text;

namespace Sample.Forge.Coordination
{
    public enum IndexFieldType : int
    {
        Unknown = 0,
        Boolean = 1,
        Integer = 2,
        Double = 3,
        BLOB = 10,
        DbKey = 11,
        String = 20,
        LocalisableString = 21,
        DateTime = 22,
        GeoLocation = 23,
        Position = 24
    }
}
