using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueApp
{
    class Bridge
    {
        private List<Light> lights = new List<Light>();
        public string BridgeId { get; set; }
        public string IpAddress { get; set; }

        public List<Light> Lights
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
