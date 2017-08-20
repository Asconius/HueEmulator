using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueApp
{
    class ViewModel : INotifyPropertyChanged
    {
        private Bridge selectedBridge;
        private Light selectedLight;
        private ObservableCollection<Bridge> bridges = new ObservableCollection<Bridge>();
        private ObservableCollection<Light> lights = new ObservableCollection<Light>();
        private bool visible = false;

        public ObservableCollection<Bridge> Bridges
        {
            get
            {
                return bridges;
            }
            private set
            {
                bridges = value;
            }
        }

        public ObservableCollection<Light> Lights
        {
            get
            {
                return lights;
            }
            private set
            {
                lights = value;
            }
        }

        public Bridge SelectedBridge
        {
            get
            {
                return selectedBridge;
            }
            set
            {
                selectedBridge = value;
                Lights = selectedBridge.Lights;
                OnPropertyChanged("Lights");
            }
        }

        public Light SelectedLight
        {
            get
            {
                return selectedLight;
            }
            set
            {
                selectedLight = value;
                visible = true;
                OnPropertyChanged("Visible");
                OnPropertyChanged("HueSliderValue");
                OnPropertyChanged("SaturationSliderValue");
                OnPropertyChanged("BrightnessSliderValue");
            }
        }

        public int HueSliderValue
        {
            get
            {
                return (selectedLight == null) ? 0 : (int)(selectedLight.State.Hue.Value / 65535.0 * 10.0);
            }
            set
            {
                selectedLight.State.Hue = (int)(value * 65535.0 / 10.0);
            }
        }

        public int SaturationSliderValue
        {
            get
            {
                return (selectedLight == null) ? 0 : (int)(selectedLight.State.Saturation.Value / 254.0 * 10.0);
            }
            set
            {
                selectedLight.State.Saturation = (int)(value * 254.0 / 10.0);
            }
        }

        public byte BrightnessSliderValue
        {
            get
            {
                return (selectedLight == null) ? (byte) 0 : (byte)(selectedLight.State.Brightness / 254.0 * 10.0);
            }
            set
            {
                selectedLight.State.Brightness = (byte)(value * 254.0 / 10.0);
            }
        }

        public int Hue
        {
            get
            {
                return (selectedLight != null && selectedLight.State != null) ? selectedLight.State.Hue.Value : 0;
            }
            set
            {
                selectedLight.State.Hue = value;
            }
        }

        public int Saturation
        {
            get
            {
                return (selectedLight != null && selectedLight.State != null) ? selectedLight.State.Saturation.Value : 0;
            }
            set
            {
                selectedLight.State.Saturation = value;
            }
        }

        public byte Brightness
        {
            get
            {
                return (selectedLight != null && selectedLight.State != null) ? selectedLight.State.Brightness : (byte) 0;
            }
            set
            {
                selectedLight.State.Brightness = value;
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
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
