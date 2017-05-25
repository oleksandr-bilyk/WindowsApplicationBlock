using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ExtendedExecutionTaskAgrigation;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ReportGeneration
{
    /// <summary>
    /// Services report generation process for every organisation.
    /// </summary>
    internal sealed class ReportGenerationAgentcs : IReportGenerationAgent
    {
        private readonly ExtendedExecutionTaskAgrigation extendedExecutionTaskAgrigation;
        private readonly ConcurrentDictionary<Guid, ReportGenerationStateInfo> stateMap;

        public ReportGenerationAgentcs(
            ExtendedExecutionTaskAgrigation extendedExecutionTaskAgrigation
        )
        {
            this.extendedExecutionTaskAgrigation = extendedExecutionTaskAgrigation;
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
            var firstProgressSetCompleted = new TaskCompletionSource<bool>();
            var task = extendedExecutionTaskAgrigation.ExecuteAsync(
                async context =>
                {
                    var initialState = ReportGenerationStateInfo.NewRunning(
                        new ReportGenerationRunningInfo(progressValue: null, runningInExtendedExecution: context.InExtendedExecution)
                    );
                    bool startedByOrganisation = stateMap.TryAdd(organisationId, initialState);
                    firstProgressSetCompleted.SetResult(startedByOrganisation);

                    await ReportGenerationWork(organisationId, context);
                }
            );

            return await firstProgressSetCompleted.Task;
        }

        private async Task ReportGenerationWork(
            Guid organisationId,
            IExtendedExecutionTaskAgrigationArg executionContext
        )
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    await FooWork(organisationId, executionContext, cancellationTokenSource.Token);
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
            IExtendedExecutionTaskAgrigationArg executionContext,
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
                    stateMap[organisationId] = ReportGenerationStateInfo.NewRunning(
                        new ReportGenerationRunningInfo(
                            progressValue: completedPercent, runningInExtendedExecution: executionContext.InExtendedExecution
                        )
                    );
                    var extendedExecutionChanged = new TaskCompletionSource<bool>();
                    executionContext.InExtendedExecutionChanged += 
                        (bool inExtendedExecution) => extendedExecutionChanged.SetResult(inExtendedExecution);
                    await Task.WhenAny(
                        Task.Delay(TimeSpan.FromSeconds(2), cancellationToken),
                        extendedExecutionChanged.Task
                    );
                }
                else break;
            }
        }
    }
}
