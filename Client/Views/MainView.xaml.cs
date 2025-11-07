using Client.Views.MainViews;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        // 标题栏双击
        private void ColorZone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                colorZoneControl.ToggleMaximizeState(window);
            }
        }

        // 标题栏拖拽
        private void ColorZone_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtonState.Pressed != e.LeftButton)
                return;
            var window = Window.GetWindow(this);
            if (window != null)
            {
                // 如果窗口是最大化状态，先还原再拖拽
                if (window.WindowState == WindowState.Maximized)
                {
                    // 计算还原后的位置
                    var point = e.GetPosition(this);
                    var screenPoint = PointToScreen(point);

                    // 先还原窗口
                    window.WindowState = WindowState.Normal;

                    // 设置窗口位置，使鼠标位置在标题栏中间
                    window.Left = screenPoint.X - (window.ActualWidth / 2);
                    // 稍微偏上，让鼠标在标题栏区域
                    window.Top = screenPoint.Y - 10; 
                }
                // 开始拖拽，让窗口移动
                this.DragMove();
            }
        }
        /// <summary>
        /// 在窗口状态变化之后执行的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChange(object sender, EventArgs e)
        {
            colorZoneControl.UpdateMaximizeButton();
        }
    }
}