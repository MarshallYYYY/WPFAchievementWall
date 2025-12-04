using Client.Common;
using Client.Models;
using Client.Services;
using Client.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(
            IMessageBoxService messageBoxService,
            ILoadingService loadingService,
            IUserSession userSession,
            UserService userService,
            ISnackbarService snackbarService)
        {
            TabChangedCommand = new DelegateCommand<object>(TabSelectionChanged);
            this.messageBoxService = messageBoxService;
            this.loadingService = loadingService;
            this.userSession = userSession;
            this.userService = userService;
            this.snackbarService = snackbarService;

            SaveUserNameCommand = new DelegateCommand(SaveUserName);
            SavePasswordCommand = new DelegateCommand(SavePassword);
        }

        #region 服务、会话
        private readonly IMessageBoxService messageBoxService;
        private readonly ILoadingService loadingService;
        private readonly IUserSession userSession;
        private readonly UserService userService;
        private readonly ISnackbarService snackbarService;
        #endregion

        #region TabItem 切换
        private int tabControlSelectedIndex = 0;
        public int TabControlSelectedIndex
        {
            get { return tabControlSelectedIndex; }
            set { SetProperty(ref tabControlSelectedIndex, value); }
        }
        public DelegateCommand<object> TabChangedCommand { get; }

        private void TabSelectionChanged(object obj)
        {
            if (obj is TabItem tabItem)
            {
                switch (tabItem.Tag)
                {
                    case SettingsTabItemTags.About:
                        break;
                    case SettingsTabItemTags.ChangeAvatar:
                        ChangeAvatar();
                        break;
                    case SettingsTabItemTags.ChangeUserName:
                        ChangeUserName();
                        break;
                    case SettingsTabItemTags.ChangePassword:
                        ChangePassword();
                        break;
                    case SettingsTabItemTags.DeleteAccount:
                        DeleteAccount();
                        break;
                    case SettingsTabItemTags.LogOut:
                        LogOut();
                        break;
                }
            }
        }
        #endregion
        private void ChangeAvatar()
        {
        }

        #region 更换昵称
        private string newUserName = string.Empty;
        public string NewUserName
        {
            get { return newUserName; }
            set { SetProperty(ref newUserName, value); }
        }
        private void ChangeUserName()
        {
            NewUserName = string.Empty;
        }
        public DelegateCommand SaveUserNameCommand { get; }
        private void SaveUserName()
        {
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                snackbarService.SendMessage("昵称不能为空！");
                return;
            }

            userSession.CurrentUser.UserName = newUserName;

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                ApiResult apiResult = await userService.UpdateUserAsync(userSession.CurrentUser);
                if (apiResult.IsSuccess)
                {
                    snackbarService.SendMessage("更换昵称成功！");
                    // 清空输入框的内容
                    ChangeUserName();
                }
                else
                {
                    snackbarService.SendMessage(apiResult.ErrorMessage!);
                }
            });
        }
        #endregion

        #region 修改密码
        private string oldPassword = string.Empty;
        public string OldPassword
        {
            get { return oldPassword; }
            set { SetProperty(ref oldPassword, value); }
        }
        private string newPassword = string.Empty;
        public string NewPassword
        {
            get { return newPassword; }
            set { SetProperty(ref newPassword, value); }
        }
        private string newPasswordAgain = string.Empty;
        public string NewPasswordAgain
        {
            get { return newPasswordAgain; }
            set { SetProperty(ref newPasswordAgain, value); }
        }
        private void ChangePassword()
        {
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            NewPasswordAgain = string.Empty;
        }
        public DelegateCommand SavePasswordCommand { get; }
        private void SavePassword()
        {
            if (string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(newPasswordAgain))
            {
                snackbarService.SendMessage("密码不能为空！");
                return;
            }
            if (newPassword != newPasswordAgain)
            {
                snackbarService.SendMessage("两次新密码输入不一致！");
                return;
            }
            // TODO：向数据库请求数据来验证密码，而不是将原来的密码存储在客户端中。
            if (oldPassword != userSession.CurrentUser.Password)
            {
                snackbarService.SendMessage("密码不正确！");
                return;
            }

            userSession.CurrentUser.Password = newPassword;

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                ApiResult apiResult = await userService.UpdateUserAsync(userSession.CurrentUser);
                if (apiResult.IsSuccess)
                {
                    snackbarService.SendMessage("修改密码成功！");
                    // 清空三个密码框的内容
                    ChangePassword();
                }
                else
                {
                    snackbarService.SendMessage(apiResult.ErrorMessage!);
                }
            });
        }
        #endregion

        private async void DeleteAccount()
        {
            ButtonResult boxResult = await messageBoxService.ShowAsync(
                "警告", "确定注销吗？\n注销后与该账号关联的成就和目标数据都将被删除！");
            if (boxResult == ButtonResult.Cancel)
            {
                TabControlSelectedIndex = 0;
                return;
            }

            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                bool result = await userService.DeleteUserAsync(userSession.CurrentUser.Id);
                if (result is true)
                {
                    AppShutdown();
                }
            });
        }

        private async void LogOut()
        {
            string message = "确定要退出登录吗？";
            if (Debugger.IsAttached)
            {
                message += "\n重启后将断开与调试器的连接，若仍需调试需要手动附加调试器！";
            }
            ButtonResult result = await messageBoxService.ShowAsync("警告", message);
            if (result == ButtonResult.Cancel)
            {
                TabControlSelectedIndex = 0;
                return;
            }
            AppShutdown();
        }

        private static void AppShutdown()
        {
            // 获取当前应用程序的路径和文件名

            //旧版.NET Framework
            //string applicationPath = Process.GetCurrentProcess().MainModule!.FileName;
            string applicationPath = Environment.ProcessPath!;

            // 启动新的应用程序实例
            Process.Start(applicationPath);

            // 关闭当前应用程序
            Application.Current.Shutdown();
        }
    }
}