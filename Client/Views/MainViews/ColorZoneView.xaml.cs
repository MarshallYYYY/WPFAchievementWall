using System.Windows;
using System.Windows.Controls;

namespace Client.Views.MainViews
{
    public partial class ColorZoneView : UserControl
    {
        public ColorZoneView()
        {
            InitializeComponent();
        }

        // 获取父窗口
        private Window GetParentWindow()
        {
            return Window.GetWindow(this);
        }

        // 最小化按钮点击
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow();
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        // 最大化/还原按钮点击
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow();
            if (window != null)
            {
                ToggleMaximizeState(window);
            }
        }

        // 关闭按钮点击
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = GetParentWindow();
            window?.Close();
        }

        // 切换最大化状态
        public void ToggleMaximizeState(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                // 从最大化还原到正常
                window.WindowState = WindowState.Normal;
                MaximizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }
            else
            {
                // 从正常最大化
                window.WindowState = WindowState.Maximized;
                MaximizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
            }
        }

        // 监听窗口状态变化，更新按钮图标
        public void UpdateMaximizeButton()
        {
            var window = GetParentWindow();
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    MaximizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
                }
                else
                {
                    MaximizeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
                }
            }
        }
    }
}