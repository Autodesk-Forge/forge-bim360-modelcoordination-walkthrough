using Autodesk.Forge.Bim360.ModelCoordination.ModelSet;
using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestEnvironmentSetup
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
            using (var ctx = ForgeAppContext.Create())
            {
                var configManager = ctx.ExportService<IForgeAppConfigurationManager>();

                ConsoleExt.DoConsoleAction(() =>
                {
                    Assert.True(configManager.ConfigDirectory.Exists, $"Could not find sample config folder {configManager.ConfigDirectory.FullName}");
                },
                $"GET local user ForgeApp config folder");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var token = await configManager.GetCachedToken() ?? throw new InvalidOperationException("Could not get a cached OAuth token! Have you run MCAuth or MCConfig?");
                },
                $"GET cached user OAuth token");

                var forgeClient = ctx.ExportService<IForgeDataClient>();

                ConsoleExt.DoConsoleAction(() =>
                {
                    var config = forgeClient.Configuration ?? throw new InvalidOperationException("Could not determine default ForgeApp configuration! Have you run MCConfig?");

                },
                "GET Current default configuraiton");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var token = await forgeClient.GetToken() ?? throw new InvalidOperationException("Could not get a cached token! Have you run MCConfig or MCAuth?");

                },
                "GET Current cached token");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var project = await forgeClient.GetProject() ?? throw new InvalidOperationException("Could access a test Account/Project! Have you run MCConfig?");
                },
                $"GET Configured hub ({forgeClient.Configuration.ForgeBimHubId}) project ({forgeClient.Configuration.ForgeBimProjectId})");

                ConsoleExt.DoConsoleAction(() =>
                {
                    Assert.True(SampleFileManager.StateDirectory.Exists, $"Could not find tmp state folder {SampleFileManager.StateDirectory.FullName}");
                },
                $"GET Tmp state folder");

                ConsoleExt.DoConsoleAction(() =>
                {
                    Assert.True(SampleFileManager.SampleDirectory.Exists, $"Could not find sample file folder {SampleFileManager.SampleDirectory.FullName}");
                },
                $"GET Sample file folder");

                var msClient = ctx.ExportService<IForgeModelSetClient>();

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    try
                    {
                        var container = await msClient.GetContainer(forgeClient.Configuration.Project);
                    }
                    catch (ModelSetException ex)
                    {
                        if (ex.StatusCode == 401)
                        {
                            Console.WriteLine();
                            Console.WriteLine("  - No model coordination container found. Use the web UI to make a dummy coordination space!");
                        }
                        else
                        {
                            throw;
                        }
                    }
                },
                $"GET model coordinaiton container {forgeClient.Configuration.Project}");
            }
        }
    }
}
