using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MCCommon
{
    public static class ConsoleCommandHost
    {
        public static void Run(string banner, Assembly[] parts = default)
        {
            try
            {
                RunAsync(banner, parts).Wait();
            }
            catch (Exception ex)
            {
                PrintError(ex);

                Console.ReadLine();
            }
        }

        private static async Task RunAsync(string banner, Assembly[] parts = default)
        {
            var config = new ContainerConfiguration();

            config.AddModelCoordinationCommon();

            if (parts != null)
            {
                foreach (var part in parts)
                {
                    config.WithAssembly(part);
                }
            }

            using (var ctx = config.CreateContainer())
            {
                while (true)
                {
                    Console.Clear();

                    var commands = ctx.GetExports<IConsoleCommand>().GroupBy(c => c.Group);

                    Console.WriteLine(new string('-', banner.Length + 8));
                    Console.WriteLine($"    {banner}");
                    Console.WriteLine(new string('-', banner.Length + 8));

                    int offset = 0;

                    var choices = new List<IConsoleCommand>();

                    foreach (var group in commands.OrderBy(g => g.Key))
                    {
                        Console.WriteLine();

                        var groupCommands = group.OrderBy(c => c.Order).ThenBy(c => c.Display).ToArray();

                        for (int i = 0; i < groupCommands.Length; i++)
                        {
                            choices.Add(groupCommands[i]);

                            Console.WriteLine($"  {i + offset + 1,2}. {groupCommands[i].Display}");
                        }

                        offset += groupCommands.Length;
                    }

                    Console.WriteLine();
                    Console.WriteLine($"  {offset + 1}. Exit");
                    Console.WriteLine();
                    Console.Write("Select : ");

                    var choiceTxt = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(choiceTxt) && int.TryParse(choiceTxt, out int choice))
                    {
                        var commandIndex = choice - 1;

                        if (commandIndex >= 0 && commandIndex < choices.Count)
                        {
                            Console.Clear();

                            try
                            {
                                await choices[commandIndex].DoInput();

                                if (choices[commandIndex].Continue())
                                {
                                    await choices[commandIndex].RunCommand();
                                }
                            }
                            catch (Exception ex)
                            {
                                PrintError(ex);
                            }
                        }
                        else if (choice == choices.Count + 1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void PrintError(Exception error)
        {
            if (error is AggregateException)
            {
                ((AggregateException)error).Handle(e =>
                {
                    Console.WriteLine(e.Message);

                    return true;
                });
            }
            else
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
