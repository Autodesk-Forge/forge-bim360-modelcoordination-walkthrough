using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCSample.Model.Cosmo
{
    public interface ICosmoRevitClashClient
    {
        Task CreateContainerIfNotExists();

        Task PublishReports(IReadOnlyCollection<RevitClashReport> reports);
    }
}
