using MahApps.Metro.Controls;
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
using WpfControls.Win10Style;
using static AutoCompletion.Settings;
using static WpfCodes.Program.Startup;
using static WpfControls.Dialog.DialogHelper;

namespace AutoCompletion
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow, INotifyPropertyChanged
    {
        AutoCompletionHelper helper;

        WpfCodes.Device.HotKey hotkey;


        public MainWindow()
        {
            helper = new AutoCompletionHelper();

            hotkey = new WpfCodes.Device.HotKey();
            SwitchHotKey = new HotKey(DefaultSetting.SwitchHotKey.Key, DefaultSetting.SwitchHotKey.Modifiers);

            InitializeComponent();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Infos)));
            StartupButtonContent = WillRunWhenStartup("AutoCompletion") ? "取消开机自启" : "注册开机自启";

            Icon =
          Imaging.CreateBitmapSourceFromHBitmap(
          Properties.Resources.icon.ToBitmap().GetHbitmap(),
          IntPtr.Zero,
          Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());
        }

        public ObservableCollection<Info> Infos => DefaultSetting.Infos;

        public event PropertyChangedEventHandler PropertyChanged;



        private void ShutdownButtonClick(object sender, RoutedEventArgs e)
        {
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

        public HotKey SwitchHotKey
        {
            get => switchHotKey;
            set
            {

                try
                {
                    hotkey.UnregisterAll();
                    hotkey.Register(value.Key, value.ModifierKeys); switchHotKey = value;

                    DefaultSetting.SwitchHotKey = new WpfCodes.Device.HotKey.HotKeyInfo(value.Key, value.ModifierKeys);
                }
                catch (Exception ex)
                {
                    helper.ShowTrayMessage("注册热键失败" + Environment.NewLine + ex.Message);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SwitchHotKey)));

            }
        }

        private HotKey switchHotKey;



        private void StartupButtonClick(object sender, RoutedEventArgs e)
        {
            if (WillRunWhenStartup("AutoCompletion") ? CancelRunWhenStartup("AutoCompletion") : SetRunWhenStartup("AutoCompletion", "startup"))
            {
                StartupButtonContent = WillRunWhenStartup("AutoCompletion") ? "取消开机自启" : "注册开机自启";
            }
            else
            {
                ShowError("失败");
            }
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
    }
}
