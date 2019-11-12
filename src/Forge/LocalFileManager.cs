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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Forge
{
    internal sealed class LocalFileManager : ILocalFileManager
    {
        private readonly static Lazy<DirectoryInfo> _lazySampleFolder = new Lazy<DirectoryInfo>(() =>
            new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "..", "..", "models")));

        private readonly static Lazy<DirectoryInfo> _configDir = new Lazy<DirectoryInfo>(() =>
        {
            var dir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".adsk-forge"));

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        });
            

        public bool JsonPathExists<T>()
        {
            return GetJsonPath<T>().Exists;
        }

        public FileInfo GetPathToSampleModelFile(string sampleName, string modelFileName)
            => new FileInfo(Path.Combine(_lazySampleFolder.Value.FullName, $"{sampleName}", $"{modelFileName}"));

        public T ReadJson<T>() where T : class, new()
        {
            var task = ReadJsonAsync<T>();

            task.Wait();

            return task.Result;
        }

        public async Task<T> ReadJsonAsync<T>()
             where T : class, new()
        {
            var path = GetJsonPath<T>();

            T config;

            if (path.Exists)
            {
                config = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path.FullName, Encoding.UTF8));
            }
            else
            {
                config = new T();
            }

            return config;
        }

        public FileInfo WriteJson<T>(T configuration) where T : class, new()
        {
            var task = WriteJsonAsync<T>(configuration);

            task.Wait();

            return task.Result;
        }

        public async Task<FileInfo> WriteJsonAsync<T>(T jsonObject)
            where T : class, new()
        {
            var path = GetJsonPath<T>();

            await File.WriteAllTextAsync(path.FullName, JsonConvert.SerializeObject(jsonObject, Formatting.Indented), Encoding.UTF8);

            path.Refresh();

            return path;
        }

        public FileInfo GetJsonPath<T>() => new FileInfo(Path.Combine(_configDir.Value.FullName, $"{typeof(T).Name}.json"));

        public FileInfo NewPath(string fileName) => new FileInfo(Path.Combine(_configDir.Value.FullName, fileName));
    }
}
