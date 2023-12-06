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

        public event Action<IDialogResult> RequestClose;

        public IssueWrapper IssueWrapper { get => issueWrapper; set => SetProperty(ref issueWrapper, value); }

        public string Title => string.Empty;

        public DelegateCommand CloseCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult());
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
        }
    }
}