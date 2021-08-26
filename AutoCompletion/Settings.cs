using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfCodes.Program;
using static WpfCodes.Device.HotKey;

namespace AutoCompletion
{
    class Settings : SettingsBase
    {
        static Settings()
        {
            try
            {
                DefaultSetting = GetJsonSetting<Settings>("AutoCompletionConfig.json");
            }
            catch (System.Exception ex)
            {
                WpfControls.Dialog.DialogHelper.ShowException("加载配置文件失败", ex, true);

                DefaultSetting = SettingsBase.CreatJsonSetting<Settings>("AutoCompletionConfig.json");
            }
        }
        public static Settings DefaultSetting { get; }

        public ObservableCollection<Info> Infos { get; set; }

        public HotKeyInfo SwitchHotKey { get; set; } = new HotKeyInfo(Key.OemQuestion, ModifierKeys.Control | ModifierKeys.Shift);

        ~Settings()
        {
            Save();
        }
    }
}
