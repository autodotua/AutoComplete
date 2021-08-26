using FzLib.Program.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCompletion
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private SingleInstance s = new SingleInstance(FzLib.Program.App.ProgramName);

        private async void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if (await s.CheckAndOpenWindow<MainWindow>(this))
            {
                Shutdown();
                return;
            }
            MainWindow = new MainWindow();
            if (!(e.Args.Length == 1 && e.Args[0] == "startup"))
            {
                MainWindow.Show();
            }
        }
    }
}