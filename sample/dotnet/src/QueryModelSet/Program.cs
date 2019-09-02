using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryModelSet
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
            CreateModelSetState state = null;

            using (var ctx = ForgeAppContext.Create())
            {
                var mcClient = ctx.ExportService<IForgeModelSetClient>();

                /*
                 * Load the cached model set state from the CreateModelSet.dll console app
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    state = await SampleFileManager.LoadSavedState<CreateModelSetState>();

                    Assert.NotNull(state);
                },
                $"LOAD CreateModelSetState");

                /*
                 * Make sure we can pull back the model set by ID
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    Assert.NotNull(state.ModelSet);
                    Assert.NotEqual(state.ModelSet.ContainerId, Guid.Empty);
                    Assert.NotEqual(state.ModelSet.ModelSetId, Guid.Empty);

                    state = await SampleFileManager.LoadSavedState<CreateModelSetState>();

                    var ms = await mcClient.GetModelSet(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);
                },
                $"GET test model set {state.ModelSet.ModelSetId}");

                ModelSetVersionSummaryCollection versions = null;

                /*
                 * Get the model set verison summarise. Remember 
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    versions = await mcClient.GetModelSetVersions(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);

                    Console.WriteLine();

                    if (versions.ModelSetVersions.Count > 0)
                    {

                        foreach (var version in versions.ModelSetVersions.OrderBy(v => v.Version))
                        {
                            Console.WriteLine($"  {version.Version:00} : {version.CreateTime.ToString("u")}, {version.Status}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  No model set verisons found for {state.ModelSet.ModelSetId}, try again later.");
                    }
                },
                $"LOAD first page of model set versions from {state.ModelSet.ModelSetId}");

                if (versions?.ModelSetVersions.Count > 0)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        var version = await mcClient.GetModelSetVersion(state.ModelSet.ContainerId, state.ModelSet.ModelSetId, 1U);

                        Console.WriteLine();

                        if (version == null)
                        {
                            Console.WriteLine($"  No model set verisons found for {state.ModelSet.ModelSetId}, try again later.");
                        }
                        else
                        {
                            Console.WriteLine($"  {version.Version:00} : {version.CreateTime.ToString("u")}, {version.Status}, documents: {version.DocumentVersions.Count}");
                        }
                    },
                    $"GET model set version 1  from {state.ModelSet.ModelSetId}");
                }

                if (versions?.ModelSetVersions.Count > 0)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        var latest = await mcClient.GetLatestModelSetVersion(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);

                        Console.WriteLine();

                        if (latest == null)
                        {
                            Console.WriteLine($"  No model set verisons found for {state.ModelSet.ModelSetId}, try again later.");
                        }
                        else
                        {
                            Console.WriteLine($"  {latest.Version:00} : {latest.CreateTime.ToString("u")}, {latest.Status}, documents: {latest.DocumentVersions.Count}");

                            Debug.Write(JsonConvert.SerializeObject(latest, Formatting.Indented));
                        }
                    },
                    $"GET latest model set version from {state.ModelSet.ModelSetId}");
                }
            }
        }
    }
}
