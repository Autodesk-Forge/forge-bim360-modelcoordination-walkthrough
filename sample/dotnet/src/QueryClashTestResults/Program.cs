using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryClashTestResults
{
    class Program
    {
        static void Main(string[] args)
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
            QueryClashTestResultsState clashState = new QueryClashTestResultsState();
            CreateModelSetState modelSetState = null;

            using (var ctx = ForgeAppContext.Create())
            {
                var mcClient = ctx.ExportService<IModelSetClient>();
                var clashClient = ctx.ExportService<IClashClient>();

                /*
                 * Load the cached model set state from the CreateModelSet.dll console app
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    modelSetState = await SampleFileManager.LoadSavedState<CreateModelSetState>();

                    Assert.NotNull(modelSetState);
                },
                $"LOAD CreateModelSetState");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    clashState.Container = modelSetState.ModelSet.ContainerId;

                    var tests = await clashClient.GetModelSetClashTests(modelSetState.ModelSet.ContainerId, modelSetState.ModelSet.ModelSetId);

                    Console.WriteLine();

                    if (tests.Tests.Count > 0)
                    {
                        foreach (var test in tests.Tests.OrderBy(t => t.ModelSetVersion))
                        {
                            Console.WriteLine($"  {test.ModelSetId}:{test.ModelSetVersion:00}, {test.Status} : {test.CompletedOn?.ToString("u")} ");
                        }

                        clashState.Latest = tests.Tests.OrderBy(t => t.ModelSetVersion).Last();
                    }
                    else
                    {
                        Console.WriteLine($"  No test found for {modelSetState.ModelSet.ModelSetId}, try again later.");
                    }
                },
                $"GET Clash tests for model set {modelSetState.ModelSet.ModelSetId}");

                if (clashState.HasLatest)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        var clashTest = await clashClient.GetModelSetClashTest(modelSetState.ModelSet.ContainerId, clashState.Latest.Id);

                        Assert.NotNull(clashTest);
                    },
                    $"GET Clash test {clashState.Latest.Id}");
                }
                else
                {
                    Console.Write($"GET Clash test {clashState.Latest.Id}");
                    ConsoleExt.WriteRight("SKIPPED", ConsoleColor.Yellow);
                    Console.WriteLine();
                }

                if (clashState.HasLatest)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        clashState.ResourceCollection = await clashClient.GetModelSetClashTestResources(modelSetState.ModelSet.ContainerId, clashState.Latest.Id);

                        if (clashState.ResourceCollection?.Resources.Count > 0)
                        {
                            Console.WriteLine();

                            foreach (var res in clashState.ResourceCollection.Resources)
                            {
                                Console.WriteLine($"  {res.Type}");
                                Console.WriteLine($"  {res.Url}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"  No resources found for {clashState.Latest.Id}.");
                        }
                    },
                    $"GET Clash test {clashState.Latest.Id} resources");
                }
                else
                {
                    Console.Write($"GET Clash test {clashState.Latest.Id} resources");
                    ConsoleExt.WriteRight("SKIPPED", ConsoleColor.Yellow);
                    Console.WriteLine();
                }

                if (clashState.HasResources)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        Console.WriteLine();

                        foreach (var resource in clashState.ResourceCollection.Resources)
                        {
                            var name = new Uri(resource.Url).Segments.Last();

                            var fout = SampleFileManager.NewStatePath(name);

                            Console.WriteLine($"  Download {fout.Name}");

                            await clashClient.DownloadClashTestResource(resource, fout);

                            clashState.LocalResourcePaths[resource.Url] = fout;
                        }
                    },
                    $"DOWNLOAD clash resources for test {clashState.Latest.Id}");
                }

                if (clashState.HasResources)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        Console.WriteLine();

                        var documentIndexFile = clashState.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-document.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                        var documentIndexReader = new ClashResultReader<ClashDocument>(documentIndexFile, true);

                        var documentIndex = new Dictionary<int, string>();

                        await documentIndexReader.Read(doc =>
                        {
                            documentIndex[doc.Index] = doc.Urn;

                            return Task.FromResult(true);
                        });

                        var clashResultFile = clashState.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-clash.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                        var clashIndex = new Dictionary<int, Clash>();

                        var clashReader = new ClashResultReader<Clash>(clashResultFile, true);

                        await clashReader.Read(c =>
                        {
                            clashIndex[c.Id] = c;

                            return Task.FromResult(true);
                        });

                        // Show counts for the different clash statuses 
                        foreach (var group in clashIndex.Values.GroupBy(c => c.Status))
                        {
                            Console.WriteLine($"  Clash count for status {group.Key}: {group.Count()}");
                        }

                        // get the clsh instance details
                        var clashInstanceFile = clashState.LocalResourcePaths.Values.Single(f => f.Name.Equals("scope-version-clash-instance.2.0.0.json.gz", StringComparison.OrdinalIgnoreCase));

                        var clashInstanceReader = new ClashResultReader<ClashInstance>(clashInstanceFile, true);

                        var clashInstanceIndex = new Dictionary<int, ClashInstance>();

                        await clashInstanceReader.Read(ci =>
                        {
                            clashInstanceIndex[ci.ClashId] = ci;

                            return Task.FromResult(true);
                        });

                        // pick a random clash an view it
                        var rnd = new Random(Guid.NewGuid().GetHashCode());

                        var clash = clashIndex.Values.Skip(rnd.Next(0, clashIndex.Count - 1)).Take(1).Single();

                        Console.WriteLine($"  Clash : {clash.Id}");
                        Console.WriteLine($"  Left Document : {documentIndex[clashInstanceIndex[clash.Id].LeftDocumentIndex]}");
                        Console.WriteLine($"  Left Stable Object ID : {clashInstanceIndex[clash.Id].LeftStableObjectId}");
                        Console.WriteLine($"  Left LMV ID : {clashInstanceIndex[clash.Id].LeftLmvObjectId}");
                        Console.WriteLine($"  Right Document : {documentIndex[clashInstanceIndex[clash.Id].RightDocumentIndex]}");
                        Console.WriteLine($"  Right Stable Object ID : {clashInstanceIndex[clash.Id].RightStableObjectId}");
                        Console.WriteLine($"  Right LMV ID : {clashInstanceIndex[clash.Id].RightLmvObjectId}");

                    },
                    $"REPORT clashes for {clashState.Latest.Id}");
                }

                await SampleFileManager.SaveState(clashState);
            }
        }
    }
}
