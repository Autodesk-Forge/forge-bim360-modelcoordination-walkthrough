using Autodesk.Nucleus.Scopes.Entities.V3;
using MCCommon;
using MCSample;
using MCSample.Forge;
using MCSample.Model;
using System;
using System.Threading.Tasks;

namespace CreateModelSet
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
                var forge = ctx.ExportService<IForgeDataClient>();

                var state = new CreateModelSetState();

                /*
                 * Find the Plans folder using the hub and project that have been configured using MCConfig
                 */

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    state.PlansFolder = await forge.FindTopFolderByName("Plans") ?? throw new InvalidOperationException("Could not find Plans folder!");
                },
                $"FIND plans folder ID for project {forge.Configuration.ForgeBimProjectId}");

                /*
                 * This sample code creates a folder in plans under which all of the subsequent sample code does its
                 * work. If this folder does not exist in Plans it is created.
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    state.TestFolderRoot = await forge.FindFolderByName(state.PlansFolder.Id, SampleConstants.MC_SAMPLE_FOLDER_NAME);
                },
                $"FIND {SampleConstants.MC_SAMPLE_FOLDER_NAME} root folder in Plans ({state.PlansFolder.Id})");

                /*
                 * If we did not find the MC sample root folder in the call above then create it.
                 */
                if (state.TestFolderRoot == null)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        state.TestFolderRoot = await forge.CreateFolder(state.PlansFolder.Id, SampleConstants.MC_SAMPLE_FOLDER_NAME) ?? throw new InvalidOperationException($"Create {SampleConstants.MC_SAMPLE_FOLDER_NAME} failed!");
                    },
                    $"CREATE {SampleConstants.MC_SAMPLE_FOLDER_NAME} root folder in Plans ({state.PlansFolder.Id})");
                }

                /*
                 * Now that the MC sample root folder has been created make a new folder beneath this folder for this run
                 * of this console application. This is just a MC_{UTD-DATA-NOW} folder. This is not a convention for
                 * Model Coordination in general, it is just how this sample code is laid out.
                 */
                var testFolderName = $"MC_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";

                await ConsoleExt.DoConsoleAction(async () =>
                {
                    state.TestFolder = await forge.CreateFolder(state.TestFolderRoot.Id, testFolderName) ?? throw new InvalidOperationException($"Create {testFolderName} failed!");
                },
                $"CREATE {testFolderName} in {SampleConstants.MC_SAMPLE_FOLDER_NAME} ({state.TestFolderRoot.Id})");

                /*
                 * Find the two models that this sample code uploads into the test folder created above. Thsi will trigger the 
                 * BIM 360 Docs extraction workflow for these files. The upload process has two phases, (1) sending the bytes
                 * to OSS (Autodesk cloud bucket storage) and (2) registerig these OSS resources with Forge-DM to make them
                 * accessable via BIM 360 Docs.
                 */
                state.Uploads.Add(new ForgeUpload
                {
                    File = SampleFileManager.GetFile(SampleFileManager.Sample.Audubon.V1, "Audubon_Architecture.rvt")
                });

                state.Uploads.Add(new ForgeUpload
                {
                    File = SampleFileManager.GetFile(SampleFileManager.Sample.Audubon.V1, "Audubon_Structure.rvt")
                });

                state.Uploads.Add(new ForgeUpload
                {
                    File = SampleFileManager.GetFile(SampleFileManager.Sample.Audubon.V1, "Audubon_Mechanical.rvt")
                });

                /*
                 * For each file :-
                 * 
                 *   - Create an OSS storage location
                 *   - Uplaod the files to OSS in chunks
                 *   - Create the Item (and Version) representing the bytes via Forge-DM
                 */
                foreach (var upload in state.Uploads)
                {
                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        upload.Storage = await forge.CreateOssStorage(state.TestFolder.Id, upload.File.Name) ?? throw new InvalidOperationException($"Crete OSS storage object for {upload.File.Name} failed!");
                    },
                    $"CREATE OSS storage locations for {upload.File.Name}");

                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        upload.Result = await forge.Upload(upload.File, upload.Storage);
                    },
                    $"UPLOAD {upload.File.Name} to {upload.File.Name}");

                    await ConsoleExt.DoConsoleAction(async () =>
                    {
                        await forge.CreateItem(state.TestFolder.Id, upload.Storage.Id, upload.File.Name);
                    },
                    $"CREATE Item for {upload.File.Name}");
                }

                /*
                 * Finally create a Model Set which targets the test folder created above.
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    var modelSetClient = ctx.ExportService<IModelSetClient>();

                    state.ModelSet = await modelSetClient.CreateModelSet(
                        forge.Configuration.Project,
                        state.TestFolder.Name,
                        new ModelSetFolder[]
                        {
                            new ModelSetFolder
                            {
                                FolderUrn = state.TestFolder.Id
                            }
                        });
                },
                $"CREATE Model set for {state.TestFolder.Name}");

                /*
                 * The other sample consoles in this solution will make use of the Model Set
                 * created by this applicaiton. To do this the state we have been creatig 
                 * is saved to Environment.SpecialFolder.UserProfile\.nucleus\T where T is
                 * typeof(CreateModelSetState) in this case.
                 */
                await ConsoleExt.DoConsoleAction(async () =>
                {
                    await SampleFileManager.SaveState(state);
                },
                $"SAVE console state");
            }
        }
    }
}
