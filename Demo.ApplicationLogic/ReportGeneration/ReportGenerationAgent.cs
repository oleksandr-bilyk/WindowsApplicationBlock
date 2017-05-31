using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<Guid, ReportGenerationByOrganisationData> stateMap;

        public ReportGenerationAgentcs(
            ExtendedExecutionTaskAgrigation extendedExecutionTaskAgrigation
        )
        {
            this.extendedExecutionTaskAgrigation = extendedExecutionTaskAgrigation;
            stateMap = new ConcurrentDictionary<Guid, ReportGenerationByOrganisationData>();
        }

        public ReportGenerationStateInfo GetStateAsync(Guid organisationId)
        {
            ReportGenerationByOrganisationData result;
            if (stateMap.TryGetValue(organisationId, out result)) return result.CurrentState;
            else return null;
        }

        /// <summary>
        /// Resets completed task exectuions.
        /// </summary>
        /// <remarks>
        /// After task execution the information about last exectuion will not be removed till this method will be called.
        /// This method should be called before running execution by key again.
        /// </remarks>
        public void ResetCompleted(Guid organisationId)
        {
            ReportGenerationByOrganisationData value;
            if (stateMap.TryGetValue(organisationId, out value))
            {
                switch (value.CurrentState.State)
                {
                    case ReportGenerationState.Running: throw new InvalidOperationException("This method may be called only after completion.");
                    case ReportGenerationState.Cancelled:
                    case ReportGenerationState.Failed:
                    case ReportGenerationState.Completed:
                        stateMap.TryRemove(organisationId, out value);
                        if (value != null) { value.Dispose(); }
                        break;
                    default: throw new NotSupportedException();
                }
            }
            else
            {
                throw new InvalidOperationException("Unknown organisation operaiton.");
            }
        }

        public void CancelByUser(Guid organisationId)
        {
            ReportGenerationByOrganisationData value;
            if (stateMap.TryGetValue(organisationId, out value))
            {
                value.CancelByUse();
            }
            else
            {
                throw new InvalidOperationException("Unknown organisation operaiton.");
            }
        }

        public async Task<ReportGenerationStartResult> StartGenerationAsync(Guid organisationId, bool requireNonbreakingBehavior)
        {
            var firstProgressSetCompleted = new TaskCompletionSource<ReportGenerationStartResult>();
            var task = extendedExecutionTaskAgrigation.ExecuteAsync(
                async extendedExecutionContext =>
                {
                    if (requireNonbreakingBehavior && !extendedExecutionContext.InExtendedExecution)
                    {
                        Func<Exception> exceptionFunc = () => new UserMessageException("Operation cannot be launched because required extended execution is not available.");
                        firstProgressSetCompleted.SetException(exceptionFunc());
                        throw exceptionFunc();
                    }

                    var data = new ReportGenerationByOrganisationData(organisationId, requireNonbreakingBehavior, extendedExecutionContext)
                    {
                        CurrentState = ReportGenerationStateInfo.NewRunning(
                            new ReportGenerationRunningInfo(progressValue: null, runningInExtendedExecution: extendedExecutionContext.InExtendedExecution)
                        )
                    };
                    var added = stateMap.TryAdd(organisationId, data);
                    firstProgressSetCompleted.SetResult(added ? ReportGenerationStartResult.Started : ReportGenerationStartResult.PreviousOperationCompleted);

                    await ReportGenerationWork(data, extendedExecutionContext);
                }
            );

            return await firstProgressSetCompleted.Task;
        }

        private async Task ReportGenerationWork(
            ReportGenerationByOrganisationData data,
            IExtendedExecutionTaskAgrigationArg executionContext
        )
        {
            try
            {
                await FooWork(
                    data.OrganisationId,
                    updateProgress: (completedPercent) =>
                    {
                        data.CurrentState = ReportGenerationStateInfo.NewRunning(
                            new ReportGenerationRunningInfo(
                                progressValue: completedPercent, runningInExtendedExecution: executionContext.InExtendedExecution
                            )
                        );
                    },
                    cancellationToken: data.SharedCancellationToken
                );
                data.CurrentState = ReportGenerationStateInfo.NewCompleted(new ReportGenerationCompletionInfo());
            }
            catch (OperationCanceledException)
            {
                data.CurrentState = ReportGenerationStateInfo.NewCancelled();
            }
            catch (UserMessageException exc)
            {
                data.CurrentState = ReportGenerationStateInfo.NewFailed(new ReportGenerationFaultInfo(exc.Message));
            }
            catch (Exception)
            {
                data.CurrentState = ReportGenerationStateInfo.NewFailed(new ReportGenerationFaultInfo("Unexpected Error."));
            }
        }

        private async Task FooWork(
            Guid organisationId,
            Action<double?> updateProgress,
            CancellationToken cancellationToken
        )
        {
            DateTime startedAt = DateTime.Now;
            TimeSpan expectedDuration = TimeSpan.FromSeconds(30);
            while (true)
            {
                TimeSpan currentDuration = DateTime.Now - startedAt;
                if (currentDuration < expectedDuration)
                {
                    if (currentDuration > expectedDuration) { currentDuration = expectedDuration; }
                    double completedPercent = currentDuration.TotalMilliseconds / expectedDuration.TotalMilliseconds * 100.0;
                    updateProgress(completedPercent);
                    var extendedExecutionChanged = new TaskCompletionSource<bool>();
                    await Task.WhenAny(
                        Task.Delay(TimeSpan.FromSeconds(2), cancellationToken),
                        extendedExecutionChanged.Task
                    );
                }
                else break;
            }
        }

        private sealed class ReportGenerationByOrganisationData : IDisposable
        {
            internal readonly Guid OrganisationId;
            private readonly CancellationTokenSource cancelByUser = new CancellationTokenSource();
            private CancellationTokenSource cancelByExtendedExecutionRevoke;
            private CancellationTokenSource linkedCancellation;
            internal readonly CancellationToken SharedCancellationToken;
            private readonly IExtendedExecutionTaskAgrigationArg extendedExecutionAgregator;

            public ReportGenerationByOrganisationData(
                Guid organisationId,
                bool requireExtendedExecutionSession,
                IExtendedExecutionTaskAgrigationArg extendedExecutionAgregator
            )
            {
                this.OrganisationId = organisationId;
                this.extendedExecutionAgregator = extendedExecutionAgregator;

                if (requireExtendedExecutionSession)
                {
                    cancelByExtendedExecutionRevoke = new CancellationTokenSource();
                    extendedExecutionAgregator.InExtendedExecutionChanged += OnInExtendedExecutionChanged;
                }

                var tokenList = new List<CancellationToken> { cancelByUser.Token };
                if (cancelByExtendedExecutionRevoke != null)
                {
                    tokenList.Add(cancelByExtendedExecutionRevoke.Token);
                }

                if (tokenList.Count > 1)
                {
                    linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(tokenList.ToArray());
                    SharedCancellationToken = linkedCancellation.Token;
                }
                else
                {
                    SharedCancellationToken = tokenList.Single();
                }
            }

            internal ReportGenerationStateInfo CurrentState { get; set; }

            public void Dispose()
            {
                extendedExecutionAgregator.InExtendedExecutionChanged -= OnInExtendedExecutionChanged;
                cancelByUser.Dispose();
                if (cancelByExtendedExecutionRevoke != null) cancelByExtendedExecutionRevoke.Dispose();
                if (linkedCancellation != null) linkedCancellation.Dispose();
            }

            private void OnInExtendedExecutionChanged(bool inExtendedExecution)
            {
                if (!inExtendedExecution && cancelByExtendedExecutionRevoke != null)
                {
                    cancelByExtendedExecutionRevoke.Cancel();
                }
            }

            internal void CancelByUse() => cancelByUser.Cancel();
        }
    }
}
