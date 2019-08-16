using MCCommon;
using MCSample;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class PrintLastQueryLinesCommand : CommandBase
    {
        public override string Display => "Print last index query to console";

        public override uint Order => 3;

        public override async Task DoInput()
        {
            Me.State = await SampleFileManager.LoadSavedState<LastQueryState>();
        }

        public override async Task RunCommand()
        {
            LastQueryState state = Me.State;

            if (state == null)
            {
                throw new InvalidOperationException("No cached query not found!");
            }

            var resFile = new FileInfo(state.ResultPath);

            if (!resFile.Exists)
            {
                throw new InvalidOperationException("Cached query result file not found!");
            }

            int maxLines = Console.WindowHeight - 1;

            var lines = new List<string>(maxLines);

            int count = 0;

            using (var fin = resFile.OpenRead())
            using (var gzip = new GZipStream(fin, CompressionMode.Decompress))
            using (var sr = new StreamReader(gzip, Encoding.UTF8))
            {
                while (true)
                {
                    lines.Clear();

                    for (int i = 0; i < maxLines; i++)
                    {
                        var line = await sr.ReadLineAsync();

                        if (line == null)
                        {
                            break;
                        }
                        else
                        {
                            lines.Add(line);

                            count++;
                        }
                    }

                    lines.ForEach(l => Console.WriteLine(l));

                    if (lines.Count == maxLines)
                    {
                        Console.Write($"---------- {count} lines, q to quit ----------");

                        var key = Console.ReadKey();

                        if (key.KeyChar.Equals('q') || key.KeyChar.Equals('Q'))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
