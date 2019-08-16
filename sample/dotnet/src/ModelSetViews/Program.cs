using Autodesk.Nucleus.Scopes.Entities.V3;
using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ModelSetViews
{
    class Program
    {
        private static class SampleModelSetView
        {
            public const string Name = "Forge API Sample View";
            public const string Description = "This view was created by the Model Coordination Forge sample .NET core ModelSetViews app to demonstrate API view creation.";
        }

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
            CreateModelSetState modelSetState = null;

            using (var ctx = ForgeAppContext.Create())
            {
                var msClient = ctx.ExportService<IModelSetClient>();

                /*
                 * Load the cached model set state from the CreateModelSet.dll console app
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    modelSetState = await SampleFileManager.LoadSavedState<CreateModelSetState>();

                    Assert.NotNull(modelSetState);
                },
                $"LOAD CreateModelSetState");

                ModelSetView sampleView = null;

                bool createSampleView = true;

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var views = await msClient.GetModelSetViews(modelSetState.ModelSet.ContainerId, modelSetState.ModelSet.ModelSetId);

                    Console.WriteLine();

                    if (views?.ModelSetViews.Count > 0)
                    {
                        foreach (var view in views.ModelSetViews)
                        {
                            if (view.Name.Equals(SampleModelSetView.Name))
                            {
                                createSampleView = false;

                                sampleView = view;
                            }

                            Console.WriteLine($"  - {view.Name}, is private: {view.IsPrivate}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  No views found for model set {modelSetState.ModelSet.ModelSetId}, create sample...");
                    }
                },
                $"GET Views for model set {modelSetState.ModelSet.ModelSetId}");

                ModelSetVersion modelSetTipVersion = await msClient.GetLatestModelSetVersion(modelSetState.ModelSet.ContainerId, modelSetState.ModelSet.ModelSetId);

                if (createSampleView)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        Console.WriteLine();

                        if (modelSetTipVersion != null && modelSetTipVersion?.DocumentVersions.Count > 1)
                        {
                            var lineages = new ModelSetViewLineageUrns();
                            lineages.AddRange(modelSetTipVersion.DocumentVersions.Take(2).Select(d => d.DocumentLineage.LineageUrn));

                            var modelSetView = new NewModelSetView
                            {
                                Name = SampleModelSetView.Name,
                                Description = SampleModelSetView.Description,
                                IsPrivate = true,
                                DocumentLineageUrns = lineages
                            };

                            sampleView = await msClient.CreateModelSetView(modelSetState.ModelSet.ContainerId, modelSetState.ModelSet.ModelSetId, modelSetView);

                            Console.WriteLine($"  - {sampleView.Name}, is private: {sampleView.IsPrivate}");
                        }
                        else
                        {
                            Console.WriteLine($"  No model set version with >= 2 lineages in {modelSetState.ModelSet.ModelSetId}, skip view create!");
                        }
                    },
                    $"CREATE sample view for model set {modelSetState.ModelSet.ModelSetId}");
                }

                if (sampleView != null && sampleView.IsPrivate)
                {
                    Console.WriteLine();

                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        sampleView = await msClient.UpdateModelSetView(
                            modelSetState.ModelSet.ContainerId,
                            modelSetState.ModelSet.ModelSetId,
                            sampleView.ViewId,
                            new UpdateModelSetView
                            {
                                OldIsPrivate = true,
                                NewIsPrivate = false
                            });

                        Console.WriteLine($"  - {sampleView.ViewId}, is private: {sampleView.IsPrivate}");
                    },
                    $"SET model set view {sampleView.ViewId} public");
                }

                if (sampleView != null)
                {
                    Console.WriteLine();

                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        var viewVersion = await msClient.GetModelSetViewVersion(
                            modelSetState.ModelSet.ContainerId,
                            modelSetTipVersion.ModelSetId,
                            (uint)modelSetTipVersion.Version,
                            sampleView.ViewId);

                        Console.WriteLine($"  - Resolve {sampleView.ViewId}, for model set version {modelSetTipVersion.Version}");

                        foreach (var doc in viewVersion.DocumentVersions)
                        {
                            Console.WriteLine($"    - {doc.DisplayName} => {doc.VersionUrn}");
                        }

                    },
                    $"GET model set view {sampleView.ViewId} document instances");
                }
            }
        }
    }
}
