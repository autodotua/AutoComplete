using FzLib.DataStorage.Serialization;
using FzLib.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoCompletion
{
    internal class Settings : IJsonSerializable
    {
        private const string JsonPath = "AutoCompletionConfig.json";

        static Settings()
        {
            DefaultSetting = new Settings();
            DefaultSetting.LoadFromJsonFile(JsonPath);
            if (DefaultSetting.Infos == null)
            {
                DefaultSetting.Infos = new ObservableCollection<Info>();
            }
        }

        public static Settings DefaultSetting { get; }

        public ObservableCollection<Info> Infos { get; set; }

        public void Save()
        {
            this.Save(JsonPath);
        }

        ~Settings()
        {
            Save();
        }
    }
}