using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tampleworks.WindowsApplicationBlock.ViewModel;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    /// <summary>
    /// Services report generation process for every organisation.
    /// </summary>
    internal sealed class ReportGenerationAgentcs : IReportGenerationAgent
    {
        private readonly IExtendedExecutionSessionFactory extendedExecutionSessionFactory;
        private readonly ConcurrentDictionary<Guid, ReportGenerationStateInfo> stateMap;

        public ReportGenerationAgentcs(
            IExtendedExecutionSessionFactory extendedExecutionSessionFactory
        )
        {
            this.extendedExecutionSessionFactory = extendedExecutionSessionFactory;
            stateMap = new ConcurrentDictionary<Guid, ReportGenerationStateInfo>();
        }

        public ReportGenerationStateInfo GetStateAsync(Guid organisationId)
        {
            ReportGenerationStateInfo result;
            if (stateMap.TryGetValue(organisationId, out result)) return result;
            else return null;
        }

        public async Task<bool> StartGenerationAsync(Guid organisationId)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var extendedExectuionSession = await extendedExecutionSessionFactory.TryRequestAsync(
                "Rendering", 
                revokedBySystemPolicy: () => cancellationTokenSource.Cancel()
            );

            var initialState = ReportGenerationStateInfo.NewRunning(
                new ReportGenerationRunningInfo(progressValue: null, runningInExtendedExecution: extendedExectuionSession != null));
            if (!stateMap.TryAdd(organisationId, initialState))
                return false;

            var task = ReportGenerationWork(
                organisationId, extendedExectuionSession, cancellationTokenSource
            );

            return true;
        }

        private async Task ReportGenerationWork(
            Guid organisationId, 
            IDisposable extendedExectuionSession,
            CancellationTokenSource cancellationTokenSource
        )
        {
            using (cancellationTokenSource)
            using (extendedExectuionSession)
            {
                try
                {
                    await FooWork(organisationId, extendedExectuionSession, cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    stateMap[organisationId] = ReportGenerationStateInfo.NewCancelled();
                }
                catch (UserMessageException exc)
                {
                    stateMap[organisationId] = ReportGenerationStateInfo.NewFailed(new ReportGenerationFaultInfo(exc.Message));
                }
                catch (Exception)
                {
                    stateMap[organisationId] = ReportGenerationStateInfo.NewFailed(new ReportGenerationFaultInfo("Unexpected Error."));
                }
            }
        }

        private async Task FooWork(
            Guid organisationId,
            IDisposable extendedExectuionSession,
            CancellationToken cancellationToken
        )
        {
            DateTime startedAt = DateTime.Now;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(120);
            while (true)
            {
                TimeSpan currentDuration = DateTime.Now - startedAt;
                if (currentDuration < expectedDuration)
                {
                    if (currentDuration > expectedDuration) { currentDuration = expectedDuration; }
                    double completedPercent = currentDuration.TotalMilliseconds / expectedDuration.TotalMilliseconds * 100.0;
                    stateMap[organisationId] = ReportGenerationStateInfo.NewRunning(new ReportGenerationRunningInfo(progressValue: completedPercent, runningInExtendedExecution: extendedExectuionSession != null));
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                }
                else break;
            }
        }
    }
}
