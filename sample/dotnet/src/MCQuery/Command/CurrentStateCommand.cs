using Autodesk.Nucleus.Clash.Entities.V3;
using MCCommon;
using MCSample;
using System;
using System.Threading.Tasks;

namespace MCQuery.Command
{
    internal abstract class CurrentStateCommand : CommandBase
    {
        private readonly Lazy<Task<QueryClashTestResultsState>> _lazyState = new Lazy<Task<QueryClashTestResultsState>>(() => SampleFileManager.LoadSavedState<QueryClashTestResultsState>());

        protected async Task<QueryClashTestResultsState> GetState() => await _lazyState.Value;

        protected async Task<ClashTestSummary> GetClashTest() => (await _lazyState.Value).Latest;


        protected async Task DoCurrentContainerClashTestInput()
        {
            var state = await GetState();

            GetSetExistingValue(
                () => state != null,
                () => state.Container,
                "Container",
                value => Me.Container = value,
                value =>
                {
                    if (Guid.TryParse(value, out var result))
                    {
                        Me.Container = result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Container GUID");
                    }
                });

            GetSetExistingValue(
                () => state != null && state.HasLatest,
                () => state.Latest.Id,
                "Clash Test",
                value => Me.ClashTestId = value,
                value =>
                {
                    if (Guid.TryParse(value, out var result))
                    {
                        Me.ClashTestId = result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Clash Test GUID");
                    }
                });
        }

        protected async Task DoCurrentContainerModelSetInput()
        {
            var state = await GetState();

            GetSetExistingValue(
                () => state != null,
                () => state.Container,
                "Container",
                value => Me.Container = value,
                value =>
                {
                    if (Guid.TryParse(value, out var result))
                    {
                        Me.Container = result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Container GUID");
                    }
                });

            GetSetExistingValue(
                () => state != null && state.HasLatest,
                () => state.Latest.ModelSetId,
                "Model Set",
                value => Me.ModelSetId = value,
                value =>
                {
                    if (Guid.TryParse(value, out var result))
                    {
                        Me.ModelSetId = result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Model Set GUID");
                    }
                });

            GetSetExistingValue(
                () => state != null && state.HasLatest,
                () => (uint)state.Latest.ModelSetVersion,
                "Model Set Version",
                value => Me.ModelSetVersion = (uint)value,
                value =>
                {
                    if (uint.TryParse(value, out var result))
                    {
                        Me.ModelSetVersion = result;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid Model Set Version");
                    }
                });
        }
    }
}
