using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueApp
{
    public class Bridge
    {
        private ObservableCollection<Light> lights = new ObservableCollection<Light>();
        public string BridgeId { get; set; }
        public string IpAddress { get; set; }

        public ObservableCollection<Light> Lights
        {
            get
            {
                return lights;
            }
            set
            {
                lights = value;
            }
        }

        public override string ToString()
        {
            return IpAddress;
        }
    }
}
