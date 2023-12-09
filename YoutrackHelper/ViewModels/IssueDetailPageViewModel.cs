using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YoutrackHelper.Models;

namespace YoutrackHelper.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueDetailPageViewModel : BindableBase, IDialogAware
    {
        private Connector connector;
        private IssueWrapper issueWrapper;
        private TimeCounter timeCounter;
        private TimerWrapper timerWrapper;

        public event Action<IDialogResult> RequestClose;

        public IssueWrapper IssueWrapper { get => issueWrapper; set => SetProperty(ref issueWrapper, value); }

        public string Title => string.Empty;

        public DelegateCommand CloseCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public DelegateCommand ToggleStatusCommand => new (() =>
        {
            _ = IssueWrapper.ToggleStatus(connector, timeCounter);
        });

        public DelegateCommand CompleteCommand => new (() =>
        {
            _ = IssueWrapper.Complete(connector, timeCounter);
        });

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            connector = parameters.GetValue<Connector>(nameof(Connector));
            IssueWrapper = parameters.GetValue<IssueWrapper>(nameof(IssueWrapper));
            timeCounter = parameters.GetValue<TimeCounter>(nameof(TimeCounter));

            if (timerWrapper != null)
            {
                return;
            }

            timerWrapper = new TimerWrapper((_, _) =>
            {
                IssueWrapper.UpdateWorkingDuration(DateTime.Now);
            });

            timerWrapper.Interval = 1000;
            timerWrapper.Start();
        }
    }
}