using FzLib.Program.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
            UnhandledException.RegistAll();
            UnhandledException.UnhandledExceptionCatched += UnhandledException_UnhandledExceptionCatched;

            if (await s.CheckAndOpenWindow<MainWindow>(this))
            {
                Shutdown();
                return;
            }
            if (e.Args.Length == 1 && e.Args[0] == "startup")
            {
                FzLib.Program.App.SetWorkingDirectoryToAppPath();
            }
            MainWindow = new MainWindow();
            if (!(e.Args.Length == 1 && e.Args[0] == "startup"))
            {
                MainWindow.Show();
            }
        }

        private void UnhandledException_UnhandledExceptionCatched(object sender, FzLib.Program.Runtime.UnhandledExceptionEventArgs e)
        {
            try
            {
                File.AppendAllLines("errors", new string[]
                {
                    DateTime.Now.ToString(),
                    e.Exception.ToString()
                });
                MessageBox.Show(e.Exception.Message, "程序发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
            }
            finally
            {
                Environment.Exit(-1);
            }
        }
    }
}