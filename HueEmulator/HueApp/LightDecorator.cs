using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueApp
{
    class LightDecorator : Light
    {
        private Light light;
        public LightDecorator(Light light)
        {
            this.light = light;
        }
        public new string Id
        {
            get
            {
                return light.Id;
            }
            set
            {
                light.Id = value;
            }
        }
        public new State State
        {
            get
            {
                return light.State;
            }
            set
            {
                light.State = value;
            }
        }
        public new string Type
        {
            get
            {
                return light.Type;
            }
            set
            {
                light.Type = value;
            }
        }
        public new string Name
        {
            get
            {
                return light.Name;
            }
            set
            {
                light.Name = value;
            }
        }
        public new string ModelId
        {
            get
            {
                return light.ModelId;
            }
            set
            {
                light.ModelId = value;
            }
        }
        public new string ProductId
        {
            get
            {
                return light.ProductId;
            }
            set
            {
                light.ProductId = value;
            }
        }
        public new string SwConfigId
        {
            get
            {
                return light.SwConfigId;
            }
            set
            {
                light.SwConfigId = value;
            }
        }
        public new string UniqueId
        {
            get
            {
                return light.UniqueId;
            }
            set
            {
                light.UniqueId = value;
            }
        }
        public new string LuminaireUniqueId
        {
            get
            {
                return light.LuminaireUniqueId;
            }
            set
            {
                light.LuminaireUniqueId = value;
            }
        }
        public new string ManufacturerName
        {
            get
            {
                return light.ManufacturerName;
            }
            set
            {
                light.ManufacturerName = value;
            }
        }
        public new string SoftwareVersion
        {
            get
            {
                return light.SoftwareVersion;
            }
            set
            {
                light.SoftwareVersion = value;
            }
        }

        public int HueTrackBarValue
        {
            get
            {
                return (int)(State.Hue.Value / 65535.0 * 10.0);
            }
            set
            {
                light.State.Hue = value;
            }
        }

        public int SaturationTrackBarValue
        {
            get
            {
                return (int)(light.State.Saturation.Value / 254.0 * 10.0);
            }
            set
            {
                light.State.Saturation = value;
            }
        }

        public int BrightnessTrackBarValue
        {
            get
            {
                return (int)(light.State.Brightness / 254.0 * 10.0);
            }
            set
            {
                light.State.Brightness = (byte)value;
            }
        }

        public override string ToString()
        {
            return State.Hue + " " + State.Saturation + " " + State.Brightness;
        }
    }
}
