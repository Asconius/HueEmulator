using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ViewModel model = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = model;
            Task.Run(() => FindBridge());
        }

        public ObservableCollection<Bridge> Bridges
        {
            get
            {
                return model.Bridges;
            }
        }

        public ObservableCollection<Light> Lights
        {
            get
            {
                return model.Lights;
            }
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
                Dispatcher.Invoke(() =>
                {
                    model.Bridges.Add(bridge);
                });
                IEnumerable<Light> lights = await client.GetLightsAsync();
                foreach (Light light in lights)
                {
                    Console.WriteLine(light);
                    bridge.Lights.Add(light);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
        private async void ColorButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            Light light = (Light) LightListView.SelectedItem;
            LightCommand command = new LightCommand()
            {
                Hue = model.Hue,
                Saturation = model.Saturation,
                Brightness = model.Brightness
            };
            await client.SendCommandAsync(command, new List<string> { light.Id });
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
