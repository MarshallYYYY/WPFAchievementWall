using Client.Common;
using MaterialDesignThemes.Wpf;

namespace Client.ViewModels.DialogContent
{
    public class MessageBoxViewModel : BindableBase
    {
        public MessageBoxViewModel()
        {
            OkCommand = new DelegateCommand(OkFun, () => canOk);
            CancelCommand = new DelegateCommand(Cancel, () => canCancel);
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

        private bool canOk = true;
        private bool canCancel = true;
        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private void OkFun()
        {
            DialogHost.Close(AppConstants.MessageBoxDialog, ButtonResult.OK);
            canOk = false;
            OkCommand.RaiseCanExecuteChanged();
        }
        private void Cancel()
        {
            DialogHost.Close(AppConstants.MessageBoxDialog, ButtonResult.Cancel);
            // 确定和点击按钮可能被多次、快速点击，当第二次点击过来的时候，就会报错。
            // 因为此时关闭命令已经发送，弹窗已经关闭（逻辑上），但是弹窗UI有动画，
            // 此时按钮仍可被点击，就会进入该函数内部，触发错误。
            canCancel = false;
            // 经过测试，因为弹窗每次都是new出来的，所以下面这一行不加也可以。
            // 每次新生成时，canOk 和 canCancel 都会被设置为 true，不会影响可点击性。
            CancelCommand.RaiseCanExecuteChanged();
        }
    }
}