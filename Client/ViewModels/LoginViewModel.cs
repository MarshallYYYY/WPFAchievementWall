using Client.Services;
using Models;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "个人成就记录墙";
        public LoginViewModel(UserService userService)
        {
            LoginCommand = new DelegateCommand(Login);
            OpenRegisterCommand = new DelegateCommand(
                () => TransitionerSelectedIndex = 1);
            this.userService = userService;
        }
        private int transitionerSelectedIndex = 0;
        private readonly UserService userService;

        public int TransitionerSelectedIndex
        {
            get { return transitionerSelectedIndex; }
            set { SetProperty(ref transitionerSelectedIndex, value); }
        }
        private string userName = string.Empty;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }
        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        public DelegateCommand LoginCommand { get; }
        public DelegateCommand OpenRegisterCommand { get; }
        private async void Login()
        {
            if (ValidateUser(out string errorMessage) is false)
            {
                MessageBox.Show(errorMessage, "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User user = await userService.GetUserAsyncForLogin(userName, password);
            if (user is null)
                return;
            MessageBox.Show(user.UserName);
            // 登录成功
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }
        private bool ValidateUser(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                errorMessage = "用户名不能为空！";
                return false;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "密码不能为空！";
                return false;
            }
            // 全部通过
            errorMessage = string.Empty;
            return true;
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