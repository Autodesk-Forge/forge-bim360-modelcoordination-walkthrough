using System;
using System.Threading.Tasks;

namespace MCSample.Forge
{
    public interface IForgeIssueClient : IForgeClient
    {
        Task<dynamic> GetIssue(Guid containerId, Guid issueId);
    }
}
