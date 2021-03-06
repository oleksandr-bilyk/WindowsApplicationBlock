﻿using System;
using Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.ViewDataModel;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic.MainPage
{
    public sealed class OrganisationDetailsViewModel : ViewModelBase
    {
        private bool requireExtendedExecution;

        public OrganisationDetailsViewModel(
            Guid organisationId,
            IWindowFrameControllerAgent viewAgent,
            IViewModelDataProvider viewModelDataProvider
        )
        {
            ReportGeneration = new OrganisationReportGenerationViewModel(
                organisationId, viewAgent, viewModelDataProvider.ReportGeneration, () => requireExtendedExecution
            );
        }

        public string Address => "Some Address";

        public bool LogoIsAvaialble => Logo != null;

        public byte[] Logo { get; set; }
        public bool RequireExtendedExecution
        {
            get { return requireExtendedExecution; }
            set
            {
                if (requireExtendedExecution == value) return;
                requireExtendedExecution = value;
                OnCallerPropertyChanged();
                OnRequireExtendedExecution();
            }
        }
        public event Action RequireExtendedExecutionChanged;
        private void OnRequireExtendedExecution() => RequireExtendedExecutionChanged?.Invoke();

        public OrganisationReportGenerationViewModel ReportGeneration { get; }
    }
}
