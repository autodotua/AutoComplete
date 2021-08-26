using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfCodes.Device;
using static AutoCompletion.Settings;

namespace AutoCompletion
{
    public class AutoCompletionHelper
    {
        KeyboardHook hook = new KeyboardHook("AutoCompletion");

        WpfCodes.Program.TrayIcon tray;
        public bool IsPause
        {
            get => hook.IsPaused;
            set
            {
                hook.IsPaused = value;
                if (value)
                {
                    tray.ContextMenu.MenuItems[0].Text = "启动";
                }
                else
                {
                    tray.ContextMenu.MenuItems[0].Text = "暂停";
                }

                if (App.Current.MainWindow.Visibility != Visibility.Visible || App.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    ShowTrayMessage("自动补全已" + (value ? "暂停" : "启动"));
                }
            }
        }

        public void ShowTrayMessage(string message)
        {
            tray.ShowMessage(message);
        }

        public AutoCompletionHelper()
        {
            tray = new WpfCodes.Program.TrayIcon(Properties.Resources.icon_tray, "自动补全");
            tray.MouseLeftClick += (p1, p2) =>
            {
                if (App.Current.MainWindow.Visibility == Visibility.Collapsed)
                {
                    App.Current.MainWindow.Show();
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                    //Topmost = true;
                    //Topmost = false;
                    App.Current.MainWindow.Activate();

                }
                else
                {
                    App.Current.MainWindow.WindowState = WindowState.Minimized;
                    App.Current.MainWindow.Visibility = Visibility.Collapsed;
                }
            };

            tray.AddContextMenu("暂停", () => IsPause = !IsPause);
            tray.AddContextMenu("退出", () => Application.Current.Shutdown());
            tray.Show();
            hook.KeyUp += KeyUp;
            hook.KeyDown += KeyDown;
        }

        readonly List<char> inputedChars = new List<char>();
        private void KeyDown(object sender, KeyboardHook.KeyboardHookEventArgs e)
        {
            Debug.WriteLine(KeyboardHelper.GetLocalizedKeyString(e.Key));
        }
        private void KeyUp(object sender, KeyboardHook.KeyboardHookEventArgs e)
        {
            if (modifierKeys.Any(p => e.Key == p))
            {
                return;
            }
            string inputed = KeyboardHelper.GetLocalizedKeyString(e.Key);
            if (inputed.Length == 1)
            {
                inputedChars.Add(inputed[0]);
            }
            else
            {
                inputedChars.Clear();
            }

            Check();
        }

        private readonly Key[] modifierKeys =
        {
            Key.RightCtrl,
            Key.LeftCtrl,
            Key.RightAlt,
            Key.LeftAlt,
            Key.LeftShift,
            Key.RightShift,
        };

        private void Check()
        {
            int count = inputedChars.Count;
            foreach (var item in DefaultSetting.Infos.Where(p => p.Enable))
            {
                if (item.Input == null || item.Output == null)
                {
                    continue;
                }
                int length = item.Input.Length;
                if (length <= count)
                {
                    bool ok = true;
                    for (int i = 0; i < length; i++)
                    {
                        if (item.Input[i] != inputedChars[count - length + i])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok && CheckModifierKeys(item))
                    {
                        Complete(item);
                        break;
                    }
                }
            }
        }

        private bool CheckModifierKeys(Info info)
        {
            if (info.Alt && !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)
                || info.Ctrl && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
                || info.Shift && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                return false;
            }
            return true;
        }

        private async void Complete(Info info)
        {
            hook.IsPaused = true;
            await Task.Delay(100);
            KeyboardHelper.SendString("{BS " + info.Input.Length.ToString() + "}");

            //  KeyboardHelper.SendString("+");
            //  KeyboardHelper.SendString("^");
            //  KeyboardHelper.SendString("%");

            await Task.Delay(100);

            KeyboardHelper.SendString(info.Output);
            hook.IsPaused = false;
        }

        ~AutoCompletionHelper()
        {
            tray.Dispose();
        }
    }
}
