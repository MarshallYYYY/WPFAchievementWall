namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(Login);
        }
        public DelegateCommand LoginCommand { get; }
        private void Login()
        {
            // 登录成功
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        /* 每次访问都会 new 一个新的 DialogCloseListener 对象。
         * 对于 DialogCloseListener（一般用于事件），用这个写法会有问题，
         * 因为监听者绑定的是 A 实例，而触发是 B 实例 → 不会触发展示关闭事件。*/
        // public DialogCloseListener RequestClose => new(); 

        // 只会在 ViewModel 创建时执行一次，整个生命周期中维护 同一个实例。
        public DialogCloseListener RequestClose { get; } = new();

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters) { }
    }
}