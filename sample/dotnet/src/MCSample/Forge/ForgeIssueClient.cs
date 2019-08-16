using MCCommon;
using System;
using System.Composition;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    [Export(typeof(IForgeIssueClient))]
    internal sealed class ForgeIssueClient : ForgeClientBase, IForgeIssueClient
    {
        [ImportingConstructor]
        public ForgeIssueClient(IForgeAppConfigurationManager configurationManager)
            : base(configurationManager)
        {
        }

        public async Task<dynamic> GetIssue(Guid containerId, Guid issueId)
        {
            var uri = Configuration.IssueManagementPath($"{containerId}/quality-issues/{issueId}");

            return await uri.HttpGetDynamicJson(await CreateDefaultHttpRequestHeaders(true));
        }
    }
}
