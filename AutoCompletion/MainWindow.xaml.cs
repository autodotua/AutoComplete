using FzLib.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static AutoCompletion.Settings;
using HotKey = FzLib.Device.HotKey;
using FzLib.Program;
using FzLib.Program.Runtime;

namespace AutoCompletion
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly AutoCompletionHelper helper = new AutoCompletionHelper();
        private TrayIcon tray;
        private readonly HotKey hotkey = new HotKey();

        public MainWindow()
        {
            helper = new AutoCompletionHelper();

            InitializeComponent();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Infos)));
            StartupButtonContent = Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist ? "取消开机自启" : "注册开机自启";
            helper.PropertyChanged += Helper_PropertyChanged;
            ShowTrayIcon();
        }

        private void Helper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AutoCompletionHelper.IsPause))
            {
                if (helper.IsPause)
                {
                    tray.ContextMenuStrip.Items[0].Text = "启动";
                }
                else
                {
                    tray.ContextMenuStrip.Items[0].Text = "暂停";
                }
                if (Visibility != Visibility.Visible || WindowState == WindowState.Minimized)
                {
                    tray.ShowMessage("自动补全已" + (helper.IsPause ? "暂停" : "启动"));
                }
            }
        }

        private void ShowTrayIcon()
        {
            tray = new TrayIcon(Resource.icon_tray, "自动补全");
            tray.MouseLeftClick += (p1, p2) =>
            {
                if (Visibility == Visibility.Collapsed)
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Topmost = true;
                    Topmost = false;
                    Activate();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                    Visibility = Visibility.Collapsed;
                }
            };

            tray.AddContextMenuStripItem("暂停", () => helper.IsPause = !helper.IsPause);
            tray.AddContextMenuStripItem("退出", () => Application.Current.Shutdown());
            tray.ReShowWhenDisplayChanged = true;
            tray.Show();
        }

        public ObservableCollection<Info> Infos => DefaultSetting.Infos;

        public event PropertyChangedEventHandler PropertyChanged;

        private void ShutdownButtonClick(object sender, RoutedEventArgs e)
        {
            helper.IsPause = true;
            Application.Current.Shutdown();
        }

        private string startupButtonContent;

        public string StartupButtonContent
        {
            get => startupButtonContent;
            set
            {
                startupButtonContent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartupButtonContent)));
            }
        }

        private void StartupButtonClick(object sender, RoutedEventArgs e)
        {
            if (Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist)
            {
                Startup.DeleteRegistryKey();
            }
            else
            {
                Startup.CreateRegistryKey("startup");
            }

            StartupButtonContent = Startup.IsRegistryKeyExist() == FzLib.IO.ShortcutStatus.Exist ? "取消开机自启" : "注册开机自启";
        }

        private async void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            DefaultSetting.Save();
            Button btn = sender as Button;
            btn.IsEnabled = false;
            await Task.Delay(400);
            btn.IsEnabled = true;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Collapsed;
        }

        protected override void OnClosed(EventArgs e)
        {
            tray?.Dispose();
            base.OnClosed(e);
        }
    }
}