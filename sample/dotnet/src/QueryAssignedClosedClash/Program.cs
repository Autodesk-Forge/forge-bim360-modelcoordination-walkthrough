using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QueryAssignedClosedClash
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
            QueryClashTestResultsState clashState = null;

            using (var ctx = ForgeAppContext.Create())
            {
                var clashClient = ctx.ExportService<IForgeClashClient>();

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    clashState = await SampleFileManager.LoadSavedState<QueryClashTestResultsState>();

                    Assert.NotNull(clashState);

                    Assert.True(clashState.HasLatest);
                },
                $"LOAD Clash test state");

                Guid issueContainer = Guid.Empty;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var dataClient = ctx.ExportService<IForgeDataClient>();

                    dynamic obj = await dataClient.GetProjectAsJObject();

                    issueContainer = obj.data.relationships.issues.data.id;

                    Console.WriteLine();

                    Console.WriteLine($"  Issue container GUID : {issueContainer}");
                },
                $"GET project issue container");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    Console.WriteLine();

                    var issueClient = ctx.ExportService<IForgeIssueClient>();

                    var assignedClashGroups = await clashClient.GetAssignedClashGroups(clashState.Container, clashState.Latest.Id);

                    var assignedClashGroupDetails = await clashClient.GetAssignedClashGroupDetailBatch(
                        clashState.Container,
                        clashState.Latest.Id,
                        assignedClashGroups.Groups.Select(cg => cg.Id));

                    foreach (var cg in assignedClashGroups.Groups)
                    {
                        var detail = assignedClashGroupDetails.Single(i => i.Id == cg.Id);

                        // https://forge.autodesk.com/en/docs/bim360/v1/reference/http/field-issues-:id-GET/
                        dynamic issue = await issueClient.GetIssue(issueContainer, detail.IssueId);

                        Console.WriteLine($"  Issue : {detail.IssueId}");
                        Console.WriteLine($"  Title : {issue.data.attributes.title}");
                        Console.WriteLine($"  Status : {issue.data.attributes.status}");
                        Console.WriteLine($"  Created on : {issue.data.attributes.created_at}");
                        Console.WriteLine($"  Clash count : {detail.ClashData.Clashes.Count}");
                        Console.WriteLine($"  Clash document count : {detail.ClashData.Documents.Count}");
                        Console.WriteLine($"  Clash instance count : {detail.ClashData.ClashInstances.Count}");
                        Console.WriteLine($"  Existing clash count : {cg?.Existing.Count}");
                        Console.WriteLine($"  Resolved clash count : {cg?.Resolved.Count}");

                        if (assignedClashGroups.Groups.Count > 1)
                        {
                            Console.WriteLine();
                        }
                    }
                },
                $"GET Assigned clash groups and associated issues");

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    Console.WriteLine();

                    var closedClashGroups = await clashClient.GetClosedClashGroups(clashState.Container, clashState.Latest.Id);

                    var closedClashGroupDetails = await clashClient.GetClosedClashGroupDetailBatch(
                        clashState.Container,
                        clashState.Latest.Id,
                        closedClashGroups.Groups.Select(cg => cg.Id));

                    foreach (var cg in closedClashGroups.Groups)
                    {
                        var detail = closedClashGroupDetails.Single(i => i.Id == cg.Id);

                        Console.WriteLine($"  Title : {detail.Title}");
                        Console.WriteLine($"  Reason : {detail.Reason}");
                        Console.WriteLine($"  Created on : {detail.CreatedOn}");
                        Console.WriteLine($"  Clash count : {detail.ClashData.Clashes.Count}");
                        Console.WriteLine($"  Clash document count : {detail.ClashData.Documents.Count}");
                        Console.WriteLine($"  Clash instance count : {detail.ClashData.ClashInstances.Count}");
                        Console.WriteLine($"  Existing clash count : {cg?.Existing.Count}");
                        Console.WriteLine($"  Resolved clash count : {cg?.Resolved.Count}");

                        if (detail?.ScreenShots.Count > 0)
                        {
                            var screenShotId = detail.ScreenShots.First();

                            var screenShot = SampleFileManager.NewStatePath($"{screenShotId}.png");

                            using (var ss = await clashClient.GetScreenShotAsync(clashState.Container, clashState.Latest.ModelSetId, screenShotId))
                            using (var fout = screenShot.Open(FileMode.Create))
                            {
                                await ss.Stream.CopyToAsync(fout);
                            }

                            Console.WriteLine($"  First screenshot : {screenShot.FullName}");
                        }

                        if (closedClashGroups.Groups.Count > 1)
                        {
                            Console.WriteLine();
                        }
                    }
                },
                $"GET closed clash groups");
            }
        }
    }
}
