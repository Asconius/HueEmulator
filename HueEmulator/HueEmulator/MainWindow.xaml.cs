using Newtonsoft.Json;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.HSB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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

namespace HueEmulator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel model = new ViewModel();
        private HttpListener httpListener = new HttpListener();
        private Thread thread;
        public MainWindow()
        {
            InitializeComponent();
            InitializeModel();
            DataContext = model;
            thread = new Thread(ResponseThread);
        }

        private void InitializeModel()
        {
            string[] colors = new string[] { "A80000", "FCE100", "00188F" };

            for (int i = 0; i < 3; i++)
            {
                RGBColor color = new RGBColor(colors[i]);

                LightCommand command = LightCommandExtensions.SetColor(new LightCommand(), color);

                State state = new State()
                {
                    Hue = command.Hue,
                    Saturation = command.Saturation,
                    Brightness = command.Brightness.Value
                };
                Light light = new Light()
                {
                    Id = "Id",
                    State = state
                };
                model.Lights.Add(light);
            }

            httpListener.Prefixes.Add("https://www.meethue.com/api/nupnp/");
            httpListener.Prefixes.Add("http://localhost:80/");
            httpListener.Prefixes.Add("http://127.0.0.1/");
        }

        private void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext ctx = httpListener.GetContext();
                ThreadPool.QueueUserWorkItem((_) =>
                {
                    string[] segments = ctx.Request.Url.Segments;
                    string lastSegment = segments.Last().Replace("/", "").ToLower();
                    switch (lastSegment)
                    {
                        // <bridge ip address>/api
                        case "api":
                            GetAPIHandler(ctx);
                            break;
                        // <bridge ip address>/api/<username>/lights
                        case "lights":
                            GetLightsHandler(ctx);
                            break;
                        // <bridge ip address>/api/<username>/lights/<id>
                        case "state":
                            string lightID = segments[segments.Length - 2].Replace("/", "").ToLower();
                            GetStateHandler(ctx, lightID);
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        public void GetAPIHandler(HttpListenerContext context)
        {
            Log("api");
            string data = @"[{'success':{'username': '83b7780291a6ceffbe0bd049104df'}}]";
            WriteResponse(context, data);
        }

        public void GetLightsHandler(HttpListenerContext context)
        {
            Log("lights");

            Dictionary<string, Light> lights = new Dictionary<string, Light>();
            for (int i = 0; i < model.Lights.Count; i++)
            {
                lights.Add("" + (i + 1), model.Lights.ElementAt(i));
            }

            WriteJSONResponse(context, lights);
        }

        public void GetStateHandler(HttpListenerContext context, String number)
        {
            Log("state " + context.Request.Url);

            if (context.Request.HasEntityBody)
            {
                using (System.IO.Stream body = context.Request.InputStream)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
                    {
                        String request = reader.ReadToEnd();

                        State state = JsonConvert.DeserializeObject<State>(request);

                        int index = int.Parse(number) - 1;
                        Light light = model.Lights[index];

                        light.State.Hue = state.Hue;
                        light.State.Brightness = state.Brightness;
                        light.State.Saturation = state.Saturation;
                        Dispatcher.Invoke(() =>
                        {
                            model.Lights[index] = null;
                            model.Lights[index] = light;
                        });
                        model.Lights = model.Lights;
                        WriteResponse(context, String.Empty);
                    }
                }
            }
        }

        private void WriteJSONResponse(HttpListenerContext context, Object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            WriteResponse(context, jsonData);
        }

        private void WriteResponse(HttpListenerContext context, string data)
        {
            byte[] _responseArray = Encoding.UTF8.GetBytes(data);
            context.Response.OutputStream.Write(_responseArray, 0, _responseArray.Length);
            context.Response.KeepAlive = false;
            context.Response.Close();
        }

        private void Log(string text)
        {
            model.Text = text + Environment.NewLine + model.Text;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Starting server...");
            try
            {
                httpListener.Start();
            }
            catch (HttpListenerException)
            {
                Log("Run this program as Administrator");
                return;
            }
            thread.Start();
        }

        private void StopMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Stopping server...");
            httpListener.Stop();
            thread.Abort();
        }

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            model.Text = String.Empty;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            httpListener.Stop();
            thread.Abort();
        }
    }
}
