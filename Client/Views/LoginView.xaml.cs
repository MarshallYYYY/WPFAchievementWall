using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 只响应左键
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // 获取弹出这个 UserControl 的 Window
                Window? window = Window.GetWindow(this);
                if (window != null)
                {
                    try
                    {
                        window.DragMove(); // 拖动窗口
                    }
                    catch (InvalidOperationException)
                    {
                        // DragMove 在鼠标松开前可能抛异常，可以忽略
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window? window = Window.GetWindow(this);
            window.Close();
        }
    }
}