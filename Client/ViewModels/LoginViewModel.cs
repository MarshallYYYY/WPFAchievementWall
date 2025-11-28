using Client.Common;
using Client.Services;
using Models;
using System.Windows;

namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "个人成就记录墙";
        public LoginViewModel(UserService userService, IUserSession userSession)
        {
            LoginCommand = new DelegateCommand(Login, CanLogin);
            OpenRegisterCommand = new DelegateCommand(
                () => TransitionerSelectedIndex = 1);
            this.userService = userService;
            this.userSession = userSession;
        }

        #region 服务、会话、页面切换
        private readonly UserService userService;
        private readonly IUserSession userSession;

        private int transitionerSelectedIndex = 0;
        public int TransitionerSelectedIndex
        {
            get { return transitionerSelectedIndex; }
            set { SetProperty(ref transitionerSelectedIndex, value); }
        }

        public DelegateCommand OpenRegisterCommand { get; }
        #endregion

        #region 登录验证界面
        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set
            {
                // 告诉按钮重新判断
                SetProperty(ref userName, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }
        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand LoginCommand { get; }
        private async void Login()
        {
            User user = await userService.GetUserAsyncForLogin(userName, password);
            if (user is null)
                return;

            // 保存到全局 Session
            userSession.CurrentUser = user;
            // 登录成功
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName)
                && !string.IsNullOrWhiteSpace(Password);
        }
        #endregion

        #region IDialogAware
        /* 每次访问都会 new 一个新的 DialogCloseListener 对象。
         * 对于 DialogCloseListener（一般用于事件），用这个写法会有问题，
         * 因为监听者绑定的是 A 实例，而触发是 B 实例 → 不会触发展示关闭事件。*/
        // public DialogCloseListener RequestClose => new(); 

        // 只会在 ViewModel 创建时执行一次，整个生命周期中维护 同一个实例。
        public DialogCloseListener RequestClose { get; } = new();

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters) { } 
        #endregion
    }
}