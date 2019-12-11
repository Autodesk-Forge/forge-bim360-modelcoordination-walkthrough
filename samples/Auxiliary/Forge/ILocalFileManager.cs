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
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge
{
    public interface ILocalFileManager
    {
        bool JsonPathExists<T>();

        FileInfo GetJsonPath<T>();

        Task<T> ReadJsonAsync<T>() where T : class, new();

        T ReadJson<T>() where T : class, new();

        Task<FileInfo> WriteJsonAsync<T>(T configuration) where T : class, new();

        FileInfo WriteJson<T>(T configuration) where T : class, new();

        FileInfo GetPathToSampleModelFile(string sampleName, string modelFileName);

        FileInfo NewPath(string fileName);
    }
}
