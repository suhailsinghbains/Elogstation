using Android.Net.Wifi;
using CoreLocation;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;
using Android.Content;
using Android.Bluetooth;

namespace Elogstation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        /*private readonly GeocoordinateSatelliteData _dataManager ;
        private Location location;
        public LocationDetail()
        {
            _dataManager = new GeocoordinateSatelliteData();
        }*/
        WifiManager wifi = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
        BluetoothManager bluetooth = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
        public Page1(string parameter)
        {
            InitializeComponent();
            this.Title = "Hi" + " " + parameter;
            Send_Data_Defi.IsVisible = false;
            Map_View_Defi.IsVisible = false;
            Logs_Defi.IsVisible = true;
            D_Logs.BackgroundColor = Color.FromHex("#0d47a1");
            D_Map_View.BackgroundColor = Color.FromHex("#3E88F2");
            D_Send_Data.BackgroundColor = Color.FromHex("#3E88F2");
            if(wifi.WifiState.ToString().Equals("Enabled"))
            {
                //Wifi Toggle ON
            }
            if (bluetooth.Adapter.IsEnabled)
            {
                //Bluetooth Toggle ON
            }
            string T = "eyJleHAiOjE1MDI2MDAwNDIsImlhdCI6MTUwMTk5NTI0MiwiYWxnIjoiSFMyNTYifQ.eyJpZCI6Nn0.-s1xGqei94vva2A79CyMRA8mQA-ZCcfgLvlleRlHeLE";
            //GPS_Api_CallAsync(T, 51.50336401, -0.1276250, 1, 1, 20);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            GPS_Location();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            wifi.SetWifiEnabled(e.Value);
        }
        
        private void Switch_Toggled_1(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                bluetooth.Adapter.Enable();
            }
            else
            {
                bluetooth.Adapter.Disable();
            }
        }

        private void D_Logs_Clicked(object sender, EventArgs e)
        {
            Send_Data_Defi.IsVisible = false;
            Map_View_Defi.IsVisible = false;
            Logs_Defi.IsVisible = true;
            D_Logs.BackgroundColor = Color.FromHex("#0d47a1");
            D_Map_View.BackgroundColor = Color.FromHex("#3E88F2");
            D_Send_Data.BackgroundColor = Color.FromHex("#3E88F2");
        }

        private void D_Send_Data_Clicked(object sender, EventArgs e)
        {
            Map_View_Defi.IsVisible = false;
            Logs_Defi.IsVisible = false;
            Send_Data_Defi.IsVisible = true;
            D_Send_Data.BackgroundColor = Color.FromHex("#0d47a1");
            D_Map_View.BackgroundColor = Color.FromHex("#3E88F2");
            D_Logs.BackgroundColor = Color.FromHex("#3E88F2");
        }

        private void D_Map_View_Clicked(object sender, EventArgs e)
        {
            Send_Data_Defi.IsVisible = false;
            Logs_Defi.IsVisible = false;
            Map_View_Defi.IsVisible = true;
            D_Map_View.BackgroundColor = Color.FromHex("#0d47a1");
            D_Logs.BackgroundColor = Color.FromHex("#3E88F2");
            D_Send_Data.BackgroundColor = Color.FromHex("#3E88F2");
        }
        private async Task GPS_Api_CallAsync(string Token, double Latitude, double Longitude, int User_id, int Company_id, int Rpm)
        {
            var json = JsonConvert.SerializeObject(new
            {
                token = Token,
                user_id = User_id,
                company_id = Company_id,
                latitude = Latitude,
                longitude = Longitude,
                rpm = Rpm
            });
            string data = json.ToString();
            HttpClient client = new HttpClient(new NativeMessageHandler())
            {
                MaxResponseContentBufferSize = 256000
            };
            var RestUrl = "http://54.208.49.250/api/eld";
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(RestUrl, content);
            var response = await request.Content.ReadAsStringAsync();
            SentData.Text = response;
        }

        private async Task GPS_Location()
        {
            Test1.Text += "This is a test";
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
            Test1.Text += position;
        }
    }
}