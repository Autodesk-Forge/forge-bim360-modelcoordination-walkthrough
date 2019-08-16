using MCCommon;
using MCSample;
using MCSample.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class PrettyPrintLastQueryLinesCommand : CommandBase
    {
        private readonly IIndexFieldCache _fieldCache;

        private LastQueryState _lastQueryState;

        private IReadOnlyDictionary<string, IndexField> _fields;

        [ImportingConstructor]
        public PrettyPrintLastQueryLinesCommand(IIndexFieldCache fieldCache)
            : base()
        {
            _fieldCache = fieldCache ?? throw new ArgumentNullException(nameof(fieldCache));
        }

        public override string Display => "Pretty print last index query to console";

        public override uint Order => 4;

        public override async Task DoInput()
        {
            _lastQueryState = (await SampleFileManager.LoadSavedState<LastQueryState>()) ??
                throw new InvalidOperationException("No cached query not found!");

            _fields = (await _fieldCache.Get(_lastQueryState.Container, _lastQueryState.ModelSet, _lastQueryState.Verison)) ??
                throw new InvalidOperationException("No fields found!"); ;
        }

        public override async Task RunCommand()
        {
            var resFile = new FileInfo(_lastQueryState.ResultPath);

            if (!resFile.Exists)
            {
                throw new InvalidOperationException("Cached query result file not found!");
            }

            int maxLines = Console.WindowHeight - 1;

            var lines = new List<string>(maxLines);

            int count = 0;

            var lineQueue = new ConcurrentQueue<string>();

            var reader = new IndexResultReader(resFile, _fields);

            await reader.ReadToEndAsync(obj =>
            {
                if (lines.Count == maxLines)
                {
                    lines.ForEach(l => Console.WriteLine(l));
                    lines.Clear();

                    Console.Write($"---------- {count} lines, q to quit ----------");

                    var key = Console.ReadKey();

                    if (key.KeyChar.Equals('q') || key.KeyChar.Equals('Q'))
                    {
                        return Task.FromResult(false);
                    }
                }
                else
                {
                    lines.Add(obj.ToString(Formatting.None));

                    count++;
                }

                return Task.FromResult(true);
            });

            if (lines.Count > 0)
            {
                lines.ForEach(l => Console.WriteLine(l));
            }
        }
    }
}
