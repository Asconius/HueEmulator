using Q42.HueApi.ColorConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HueEmulator
{
    class RGBExtensions
    {
        public static Color ToColor(RGBColor color)
        {
            return Color.FromRgb(ToByte(color.R), ToByte(color.G), ToByte(color.B));
        }

        private static byte ToByte(double value)
        {
            return (byte)(255.0 * value);
        }
    }
}
