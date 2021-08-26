using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompletion
{
    public class Info : INotifyPropertyChanged
    {

        public bool Enable { get; set; }

        public string Input
        {
            get => input;
            set
            {
                if (value.Any(p => p < 33 || p > 126))
                {
                    WpfControls.Dialog.DialogHelper.ShowError("输入的内容超出范围！");
                }
                else
                {
                    if (value.Any(p => p >= 'a' && p <= 'z'))
                    {
                        input = value.ToUpper();
                    }
                    else
                    {
                        input = value;
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Input)));

                }

            }
        }
        private string input;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Output { get; set; }


        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public bool Ctrl { get; set; }
    }
}
