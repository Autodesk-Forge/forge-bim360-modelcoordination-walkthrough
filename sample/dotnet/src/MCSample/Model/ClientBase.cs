using MCSample.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MCSample.Model
{
    internal abstract class ClientBase
    {
        private readonly IModelCoordinationServiceCollectionFactory _serviceCollecitonFactory;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected ClientBase(IModelCoordinationServiceCollectionFactory serviceCollecitonFactory)
        {
            _serviceCollecitonFactory = serviceCollecitonFactory ?? throw new ArgumentNullException(nameof(serviceCollecitonFactory));
        }

        protected Task<ServiceProvider> CreateServiceProvider() => _serviceCollecitonFactory.CreateServiceProvider();

        public async Task<TJob> RunJob<TJob>(
            Func<Task<TJob>> startJob,
            Func<TJob, Task<TJob>> checkProgress,
            Func<TJob, bool> continueJob)
        {
            var start = await startJob();

            var status = await checkProgress(start);

            while (continueJob(status))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000));

                status = await checkProgress(start);
            }

            return status;
        }

        protected void ResetStopwatch() => _stopwatch.Reset();

        protected void StartStopwatch() => _stopwatch.Start();

        protected void StopStopwatch() => _stopwatch.Stop();

        protected long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}
