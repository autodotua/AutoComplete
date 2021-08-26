using FzLib;
using FzLib.Device;
using FzLib.Program.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static AutoCompletion.Settings;

namespace AutoCompletion
{
    public class AutoCompletionHelper : INotifyPropertyChanged
    {
        private KeyboardHook hook = new KeyboardHook(FzLib.Program.App.ProgramName);

        public bool IsPause
        {
            get => hook.IsPaused;
            set
            {
                hook.IsPaused = value;
                this.Notify(nameof(IsPause));
            }
        }

        public AutoCompletionHelper()
        {
            hook.KeyDown += (s, e) =>
            {
                if (DefaultSetting.KeyEvent == 0)
                {
                    KeyChanged(e);
                }
            };
            hook.KeyUp += (s, e) =>
            {
                if (DefaultSetting.KeyEvent == 1)
                {
                    KeyChanged(e);
                }
            };
        }

        private readonly List<char> inputedChars = new List<char>();

        private void KeyChanged(KeyboardHook.KeyboardHookEventArgs e)
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

        public event PropertyChangedEventHandler PropertyChanged;

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
            await Task.Delay(DefaultSetting.DelayBeforeBackspace);
            KeyboardHelper.SendString("{BS " + info.Input.Length.ToString() + "}");

            await Task.Delay(DefaultSetting.DelayBeforeSend);

            KeyboardHelper.SendString(info.Output);
            hook.IsPaused = false;
        }
    }
}