using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HueEmulator
{
    class ViewModel : INotifyPropertyChanged
    {
        private string text;
        private ObservableCollection<Light> lights = new ObservableCollection<Light>();

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        public ObservableCollection<Light> Lights
        {
            get
            {
                return lights;
            }
            set
            {
                lights = value;
                OnPropertyChanged("Lights");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
