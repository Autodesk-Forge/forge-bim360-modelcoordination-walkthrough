using MCCommon;
using MCSample;
using MCSample.Forge;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MCQuery.Command
{
    [Export(typeof(IConsoleCommand))]
    internal sealed class DownloadModelDerivativeCommand : CommandBase
    {
        private readonly static Regex IsBase64Encoded = new Regex(@"^[a-zA-Z0-9\+/]+={0,3}$", RegexOptions.Compiled);

        private readonly IForgeDerivativeClient _forgeClient;

        [ImportingConstructor]
        public DownloadModelDerivativeCommand(IForgeDerivativeClient forgeClient) => _forgeClient = forgeClient ?? throw new ArgumentNullException(nameof(forgeClient));

        public override string Display => "Download model derivative";

        public override uint Group => 1U;

        public override uint Order => 2;

        public override async Task DoInput()
        {
            Console.Write("Manifest urn : ");
            string urn = Console.ReadLine().Trim();

            if (IsBase64Encoded.IsMatch(urn))
            {
                Me.Urn = urn;
            }
            else
            {
                Me.Urn = Convert.ToBase64String(Encoding.UTF8.GetBytes(urn));
            }

            var manifest = await _forgeClient.GetDerivativeManifest(Me.Urn);

            IDictionary<string, JToken> derivatives = manifest.derivatives;

            string file = $"urn:adsk.viewing:fs.file:{manifest.urn}";

            var urns = new List<string>();

            if (derivatives != null && derivatives.Count > 0)
            {
                for (int i = 0; i < derivatives.Count; i++)
                {
                    FindUrns(derivatives[i.ToString()], x => urns.Add(x));
                }
            }

            Console.WriteLine();

            for (int i = 0; i < urns.Count; i++)
            {
                Console.WriteLine($"{i}. {urns[i].Replace(file, string.Empty)}");
            }

            Console.WriteLine();
            Console.Write("Select : ");

            int index = int.Parse(Console.ReadLine().Trim());

            Me.DerivativeUrn = urns[index];
            Me.OutputPath = SampleFileManager.NewStatePath(urns[index].Substring(urns[index].LastIndexOf('/') + 1));

            Console.WriteLine();
            Console.Write($"Output path ({Me.OutputPath.FullName}) : ");
            var path = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(path))
            {
                Me.OutputPath = new FileInfo(path);
            }
        }

        public override async Task RunCommand()
        {
            string urn = Me.Urn;
            string encoded = HttpUtility.UrlEncode(Me.DerivativeUrn);

            Console.WriteLine(urn);
            Console.WriteLine(encoded);

            using (var fout = Me.OutputPath.Open(FileMode.Create))
            {
                await _forgeClient.GetDerivative(urn, encoded, async stream => await stream.CopyToAsync(fout));
            }
        }

        private void FindUrns(dynamic node, Action<string> addUrn)
        {
            string urn = node.urn;

            if (urn != null)
            {
                addUrn(urn);
            }

            IDictionary<string, JToken> children = node.children;

            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    FindUrns(node.children[i.ToString()], addUrn);
                }
            }
        }
    }
}
