using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HueApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LocalHost = "127.0.0.1";
        private ILocalHueClient client;
        private List<Bridge> bridges = new List<Bridge>();
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => FindBridge());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void FindBridge()
        {
            bool successful = false;

            while (!successful)
            {
                Task<bool> task = StartAsync();
                task.Wait();
                successful = task.Result;

                task = BridgeEntryAsync();
                task.Wait();
                successful = task.Result;
            }
        }

        public async Task<bool> StartAsync()
        {
            Console.WriteLine("Controller.StartAsync");
            IBridgeLocator locator = new HttpBridgeLocator();
            IEnumerable<LocatedBridge> bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

            foreach (LocatedBridge bridge in bridgeIPs)
            {
                Console.WriteLine(bridge);
            }
            return bridgeIPs.Count<LocatedBridge>() > 0;
        }

        public async Task<bool> BridgeEntryAsync()
        {
            client = new LocalHueClient(LocalHost);
            try
            {
                var appKey = await client.RegisterAsync("mypersonalappname", "mydevicename");

                Bridge bridge = new Bridge()
                {
                    IpAddress = LocalHost
                };
                bridges.Add(bridge);

                Dispatcher.Invoke(() =>
                {
                    BridgeListView.Items.Clear();
                    BridgeListView.Items.Add(bridge);
                });

                IEnumerable<Light> lights = await client.GetLightsAsync();
                foreach (Light light in lights)
                {
                    Console.WriteLine(light);
                    LightDecorator lightDecorator = new LightDecorator(light);
                    bridge.Lights.Add(lightDecorator);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        private void BridgeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bridge bridge = (Bridge)((ListBox)sender).SelectedItem;

            if (bridge == null)
            {
                return;
            }

            LightListView.Items.Clear();
            foreach (Light light in bridge.Lights)
            {
                LightListView.Items.Add(light);
            }
        }

        private void LightListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LightDecorator light = (LightDecorator)((ListBox)sender).SelectedItem;

            if (light == null)
            {
                return;
            }

            ColorStackPanel.Visibility = Visibility.Visible;

            HueSlider.Value = light.HueTrackBarValue;
            SaturationSlider.Value = light.SaturationTrackBarValue;
            BrightnessSlider.Value = light.BrightnessTrackBarValue;
        }

        private async void ColorButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            LightDecorator light = (LightDecorator)LightListView.SelectedItem;
            LightCommand command = new LightCommand()
            {
                Hue = (int)(HueSlider.Value * 65535.0 / 10.0),
                Saturation = (int)(SaturationSlider.Value * 254.0 / 10.0),
                Brightness = (byte)(BrightnessSlider.Value * 254.0 / 10.0)
            };
            await client.SendCommandAsync(command, new List<string> { light.Id });
        }
    }
}
