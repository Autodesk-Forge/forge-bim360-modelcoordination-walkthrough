using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;
using System.Threading.Tasks;

namespace GreeterApp
{
    interface IGreetingWriter
    {
        Task Write(string greeting);
    }

    [Shared]
    [Export(typeof(IGreetingWriter))]
    internal class ConsoleGreetingWriter : IGreetingWriter
    {
        public Task Write(string greeting)
        {
            return Task.Run(() => Console.WriteLine(greeting));
        }
    }

    interface IGreeter
    {
        Task Greet();
    }

    internal abstract class Greeter : IGreeter
    {
        private readonly IGreetingWriter _writer = null;

        protected Greeter(IGreetingWriter writer)
        {
            this._writer = writer;
        }

        public async Task Greet()
        {
            await this._writer.Write(this.GetSalutation());
        }

        protected abstract string GetSalutation();
    }

    [Export(typeof(IGreeter))]
    [Export("morning", typeof(IGreeter))]
    internal class MorningGreeter : Greeter
    {
        [ImportingConstructor]
        public MorningGreeter(IGreetingWriter writer)
            : base(writer)
        {
        }

        protected override string GetSalutation()
        {
            return "  Good morning! Time to get up.";
        }
    }

    [Export(typeof(IGreeter))]
    [Export("afternoon", typeof(IGreeter))]
    internal class AfternoonGreeter : Greeter
    {
        [ImportingConstructor]
        public AfternoonGreeter(IGreetingWriter writer)
            : base(writer)
        {
        }

        protected override string GetSalutation()
        {
            return "  Good afternoon! Time for a nap?";
        }
    }

    [Export(typeof(IGreeter))]
    [Export("evening", typeof(IGreeter))]
    internal class EveningGreeter : Greeter
    {
        [ImportingConstructor]
        public EveningGreeter(IGreetingWriter writer)
            : base(writer)
        {
        }

        protected override string GetSalutation()
        {
            return "  Good evening! Time for bed.";
        }
    }

    interface IExample
    {
        Task Run();
    }

    [Export(typeof(IExample))]
    internal class ImportManyWithAutomaticCompositionExample : IExample
    {
        private readonly IEnumerable<IGreeter> _greeters;

        [ImportingConstructor]
        public ImportManyWithAutomaticCompositionExample([ImportMany]IEnumerable<IGreeter> greeters)
        {
            this._greeters = greeters;
        }

        public async Task Run()
        {
            Console.WriteLine($"Running {nameof(ImportManyWithAutomaticCompositionExample)}...");

            foreach (var greeter in this._greeters)
            {
                await greeter.Greet();
            }

            Console.WriteLine();
        }
    }

    [Export(typeof(IExample))]
    internal class NamedContractWithManualCompositionExample : IExample
    {
        private readonly CompositionContext _containerContext = null;

        [ImportingConstructor]
        public NamedContractWithManualCompositionExample(CompositionContext containerContext)
        {
            this._containerContext = containerContext;
        }

        public async Task Run()
        {
            Console.WriteLine($"Running {nameof(NamedContractWithManualCompositionExample)}...");

            var morning = this._containerContext.GetExport<IGreeter>("morning");

            await morning.Greet();

            var afternoon = this._containerContext.GetExport<IGreeter>("afternoon");

            await afternoon.Greet();

            var evening = this._containerContext.GetExport<IGreeter>("evening");

            await evening.Greet();

            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var config = new ContainerConfiguration();

            config.WithAssembly(Assembly.GetExecutingAssembly());

            var compositionContainer = config.CreateContainer();

            var examples = compositionContainer.GetExports<IExample>();

            foreach (var example in examples)
            {
                example.Run().Wait();
            }
        }
    }
}