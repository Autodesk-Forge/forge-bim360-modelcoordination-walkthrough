# Introduction to `System.Composition`

In this example we are writing greeters which write greetings via a greeting writer and they are stitched together using examples. The interfaces look like this :-

```csharp
interface IExample
{
    Task Run();
}

interface IGreeter
{
    Task Greet();
}

interface IGreetingWriter
{
    Task Write(string greeting);
}
```    

Once we have some interfaces, the next step is to create some types which implement these interfaces, first some greeters...

Good morning,

```csharp
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
        return "Good morning! Time to get up.";
    }
}
```

Good evening

```csharp
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
        return "Good afternoon! Time for a nap?";
    }
}
```

and Good Night!

```csharp
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
        return "Good evening! Time for bed.";
    }
}
```

and now a `IGreetingWriter`

```csharp
[Shared]
[Export(typeof(IGreetingWriter))]
internal class ConsoleGreetingWriter : IGreetingWriter
{
    public Task Write(string greeting)
    {
        return Task.Run(() => Console.WriteLine(greeting));
    }
}
```

There are a few `System.Composition` elements at play here. First the `ExportAttribute`, e.g. `[Export(typeof(IGreeter))]` and `[Export("evening", typeof(IGreeter))]`. These tell the `System.Composition` infrastructure that these types are exportable (constructable) from the composition container. In the case of `[Export(typeof(IGreeter))]` this reads _this type can be exported as a instance of the `typeof(interface)` interface_. `[Export("evening", typeof(IGreeter))]` on the other hand changes the semantics subtly and states _this type can be exported as a instance of the `typeof(interface)` interface using the contract name provided_, in this example "evening".

The `ImportingConstructorAttribute`, ie. `[ImportingConstructor]` constructor attribute tells the `System.Composition` infrastructure that in order to construct an instance of this type it first needs to satisfy its _imports_. This necessitates looking in the composition container for _exports_ which match this constructor's required inputs, instating instances of these dependencies (and their dependencies) and _injecting_ them into the constructor.

Finally the `SharedAttribute`, `[Shared]` tells the `System.Composition` infrastructure to only ever instantiate one instance of this type and re-use it every time someone asks for and export which can be satisfied by this singleton.

When it comes to the examples there are two. The first uses another `System.Composition` injection feature, namely `ImportManyAttribute`, `[ImportMany]`. This is used in conjunction with `[ImportingConstructor]` to inject collections into type constructors, in this case a collection of `IEnumerable<IGreeter>`. This results in _automatic_ composition. The container automatically satisfies the type's imports as part of the export of the type i.e. in order to create an instance of the `ImportManyWithAutomaticCompositionExample` type which implements the `IExample` interface the composition container will first interrogate its parts catalogue (to use System.Composition's terminology) to find every part which implements `IGreeter`, instantiate these constituents (or get references to singleton instances, the `[Shared]` attribute) and pass them as an `IEnumerable` collection to `ImportManyWithAutomaticCompositionExample`'s constructor. `[ImportMany]` is only needed if you want to "Import Many", if you just have a single dependency type then no additional markup is needed other than  `[ImportingConstructor]`.

```csharp
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
```

The `NamedContractWithManualCompositionExample` type on the other hand, imports the actual composition container, `CompositionContext` and manually interrogates it for exports using the contact names we defined for greeters.

```csharp
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
```

To run the two examples we build a `CompositionContext` by creating a `ContainerConfiguration` and adding the Assembly which contains our exports. We are then able to export instances of our `IExample` types and execute them.

```csharp
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
```

This trivial sample only scratches the surface of `System.Composition` which is capable of supporting far more sophisticated dependency injection patterns. It does however cover the use of `System.Composition` in the BIM 360 Model Coordination samples accompanying this repo.

## Complete Code Listing

The complete sample can be found [here](../sample/dotnet/src/GreeterApp).

```powershell
 PS > cd .\sample\dotnet\src\GreeterApp
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run
```

## And Finally

Incidentally the `[Import]` attribute is also available and can be added to properties on a type, also this sample uses interfaces throughout this is NOT a requirement of `System.Composition`, it is more that happy to use concrete types. This sample only scratches the surface of the full capabilities of `System.Composition` and further reading is recommended.