using Client.Common;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.DialogContent
{
    public class MessageBoxViewModel : BindableBase
    {
        public MessageBoxViewModel()
        {
            OkCommand = new DelegateCommand(OkFun);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private void OkFun()
        {
            DialogHost.Close(AppConstants.MessageBoxDialog, ButtonResult.OK);

        }
        private void Cancel()
        {
            DialogHost.Close(AppConstants.MessageBoxDialog, ButtonResult.Cancel);
        }
    }
}
