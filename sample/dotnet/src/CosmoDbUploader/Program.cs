using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model.Cosmo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmoDbUploader
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
            var clashState = await SampleFileManager.LoadSavedState<QueryClashTestResultsState>();

            using (var ctx = ForgeAppContext.Create())
            {
                var reportBuilder = ctx.ExportService<IRevitClashReportBuilder>();

                IReadOnlyCollection<RevitClashReport> reports = null;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    reports = await reportBuilder.Build(clashState.Container, clashState.Latest.ModelSetId, (uint)clashState.Latest.ModelSetVersion, clashState.Latest.Id);
                },
                $"BUILD Revit clash report");

                if (reports?.Count > 0)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        var cosmoClient = ctx.ExportService<ICosmoRevitClashClient>();

                        await cosmoClient.PublishReports(reports);
                    },
                    $"PUBLISH Revit clash report to cosmo");
                }
            }
        }
    }
}
