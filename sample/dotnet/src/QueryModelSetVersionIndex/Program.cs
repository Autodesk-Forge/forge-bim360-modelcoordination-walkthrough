using Autodesk.Forge.Bim360.ModelCoordination.Index;
using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryModelSetVersionIndex
{
    class Program
    {
        static void Main()
        {
            try
            {
                RunAsync().Wait();
            }
            catch (Exception ex)
            {
                ex.LogToConsole();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            QueryClashTestResultsState state = null;

            using (var ctx = ForgeAppContext.Create())
            {
                var msClient = ctx.ExportService<IForgeModelSetClient>();

                var msIndex = ctx.ExportService<IForgeIndexClient>();

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    state = await SampleFileManager.LoadSavedState<QueryClashTestResultsState>();

                    Assert.NotNull(state);
                },
                $"LOAD QueryClashTestResultsState");

                ModelSetVersion modelSetVersion = null;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    modelSetVersion = await msClient.GetModelSetVersion(state.Container, state.Latest.ModelSetId, (uint)state.Latest.ModelSetVersion);

                    Assert.NotNull(modelSetVersion);
                },
                $"GET model set version {state.Latest.ModelSetId}:{state.Latest.ModelSetVersion}");

                IndexManifest indexManifest;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    indexManifest = await msIndex.GetIndexManifest(state.Container, state.Latest.ModelSetId, (uint)state.Latest.ModelSetVersion);

                    Assert.NotNull(indexManifest);
                },
                $"LOAD index manifest for model set version {state.Latest.ModelSetId}:{state.Latest.ModelSetVersion}");


                IReadOnlyDictionary<string, IndexField> fields = null;

                IndexField nameField = null;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    fields = await msIndex.GetFields(state.Container, state.Latest.ModelSetId, (uint)state.Latest.ModelSetVersion);

                    Assert.NotNull(fields);

                    // {"key":"p153cb174","category":"__name__","type":20,"name":"name","uom":null}
                    nameField = fields.Values.SingleOrDefault(f => f.Name.Equals("name", StringComparison.OrdinalIgnoreCase) &&
                                                                   f.Category.Equals("__name__", StringComparison.OrdinalIgnoreCase));

                    Assert.NotNull(nameField);
                },
                "FIND index field with category '__name__' and name, 'name'");

                FileInfo queryResults = null;

                string query = $"select s.file, s.db, s.docs, s.id, s.{nameField.Key} from s3object s where s.{nameField.Key} is not missing";

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    queryResults = await msIndex.QueryIndex(state.Container, state.Latest.ModelSetId, (uint)state.Latest.ModelSetVersion, query);

                    queryResults.Refresh();

                    Assert.True(queryResults.Exists);
                },
                $"RUN index query '{query}'");



                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var reader = new IndexResultReader(queryResults, null);

                    Console.WriteLine();

                    int count = 0;

                    await reader.ReadToEndAsync(
                        obj =>
                        {
                            count++;
                            return Task.FromResult(true);
                        }, false);

                    Console.WriteLine($"  processed {count.ToString("N0")} rows");

                },
                $"PROCESS query results with no field substitution");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var reader = new IndexResultReader(queryResults, fields);

                    Console.WriteLine();

                    int count = 0;

                    await reader.ReadToEndAsync(
                        obj =>
                        {
                            count++;
                            return Task.FromResult(true);
                        }, true);

                    Console.WriteLine($"  processed {count.ToString("N0")} rows");
                },
                $"PROCESS query results with field substitution");
            }
        }
    }
}
