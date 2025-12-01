using Client.Common;
using Client.Events;
using Client.Services;
using Client.Services.WebAPI;
using MaterialDesignThemes.Wpf;
using Models;

namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "个人成就记录墙";
        public LoginViewModel(
            UserService userService,
            IUserSession userSession,
            IEventAggregator eventAggregator,
            ILoadingService loadingService)
        {
            this.userService = userService;
            this.userSession = userSession;
            this.eventAggregator = eventAggregator;
            this.loadingService = loadingService;

            eventAggregator.GetEvent<LoadingOpenEvent>().Subscribe(LoadingSubscribe);

            LoginCommand = new DelegateCommand(Login, CanLogin);
            OpenRegisterCommand = new DelegateCommand(OpenRegister);

            RegisterCommand = new DelegateCommand(Register, CanRegister);
            OpenLoginCommand = new DelegateCommand(OpenLogin);

            MsgQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        }

        #region WebAPI用户服务、全局当前用户、页面切换、SnackbarMessageQueue
        private readonly UserService userService;
        private readonly IUserSession userSession;

        private int transitionerSelectedIndex = 0;
        public int TransitionerSelectedIndex
        {
            get { return transitionerSelectedIndex; }
            set { SetProperty(ref transitionerSelectedIndex, value); }
        }

        public SnackbarMessageQueue MsgQueue { get; }
        #endregion

        #region 加载窗口

        private readonly IEventAggregator eventAggregator;
        private readonly ILoadingService loadingService;

        private bool isOpenDialogContent = false;
        public bool IsOpenDialogContent
        {
            get { return isOpenDialogContent; }
            set { SetProperty(ref isOpenDialogContent, value); }
        }
        private void LoadingSubscribe((bool isOpen, bool isLogin) tuple)
        {
            // LoginViewModel 只监听自己的标识
            if (tuple.isLogin is true)
                IsOpenDialogContent = tuple.isOpen;
        }

        #endregion 加载窗口

        #region 登录验证界面
        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set
            {
                SetProperty(ref userName, value);
                // 告诉按钮重新判断
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
            await loadingService.RunWithLoadingAsync(async () =>
            {
                User? user = await userService.GetUserAsyncForLogin(userName, password);
                if (user is null)
                    return;

                // 保存到全局 Session
                userSession.CurrentUser = user;
                // 登录成功
                RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            }, true);
        }
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName)
                && !string.IsNullOrWhiteSpace(Password);
        }

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

        public DelegateCommand OpenRegisterCommand { get; }
        private void OpenRegister()
        {
            TransitionerSelectedIndex = 1;
            UserNameRegister = string.Empty;
            PasswordRegister = string.Empty;
            PasswordRegisterAgain = string.Empty;
        }
        #endregion

        #region 注册界面

        #region 数据、提示信息

        private string userNameRegister = string.Empty;
        public string UserNameRegister
        {
            get { return userNameRegister; }
            set
            {
                SetProperty(ref userNameRegister, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
        private string passwordRegister = string.Empty;
        public string PasswordRegister
        {
            get { return passwordRegister; }
            set
            {
                SetProperty(ref passwordRegister, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
        private string passwordRegisterAgain = string.Empty;
        public string PasswordRegisterAgain
        {
            get { return passwordRegisterAgain; }
            set
            {
                SetProperty(ref passwordRegisterAgain, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
        private string passwordboxAgainHelperText = string.Empty;
        public string PasswordboxAgainHelperText
        {
            get { return passwordboxAgainHelperText; }
            set { SetProperty(ref passwordboxAgainHelperText, value); }
        }
        #endregion

        public DelegateCommand RegisterCommand { get; }
        private async void Register()
        {
            User user = new()
            {
                UserName = userNameRegister,
                Password = passwordRegister,
            };

            await loadingService.RunWithLoadingAsync(async () =>
            {
                User newUser;
                try
                {
                    newUser = await userService.CreateUserAsync(user);
                }
                catch (Exception ex)
                {
                    MsgQueue.Enqueue(ex.Message);
                    return;
                }
                MsgQueue.Enqueue("注册成功！返回登录界面");
                OpenLogin();
                //OpenLoginCommand.Execute();
            }, true);
        }
        private bool CanRegister()
        {
            // 三个输入框都不为空
            bool result =
                !string.IsNullOrWhiteSpace(UserNameRegister)
                && !string.IsNullOrWhiteSpace(PasswordRegister)
                && !string.IsNullOrWhiteSpace(PasswordRegisterAgain);
            // 两次输入的密码需要一致
            result = result && IsSamePassword();
            return result;
        }
        private bool IsSamePassword()
        {
            if (passwordRegister != passwordRegisterAgain)
            {
                PasswordboxAgainHelperText = "两次密码输入不一致！";
                return false;
            }
            else
            {
                PasswordboxAgainHelperText = string.Empty;
                return true;
            }
        }
        public DelegateCommand OpenLoginCommand { get; }
        private void OpenLogin()
        {
            TransitionerSelectedIndex = 0;
            UserName = string.Empty;
            Password = string.Empty;
        }
        #endregion
    }
}