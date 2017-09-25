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
        Int32 i = 1;
		WifiManager wifi = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
        BluetoothManager bluetooth = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
        string Y;
        public Page1(string parameter,string y)
		{
			InitializeComponent();

			this.Title = "Hi" + " " + parameter;
			Send_Data_Defi.IsVisible = false;
			Map_View_Defi.IsVisible = false;
			Logs_Defi.IsVisible = true;
			D_Logs.BackgroundColor = Color.FromHex("#0d47a1");
			D_Map_View.BackgroundColor = Color.FromHex("#3E88F2");
            D_Send_Data.BackgroundColor = Color.FromHex("#3E88F2");
            var z = y.Length - 1;
            y = y.Remove(z, 1);
            y = y.Substring(11);
            //y = "eyJpYXQiOjE1MDU1ODYzOTYsImV4cCI6MTUwNjE5MTE5NiwiYWxnIjoiSFMyNTYifQ.eyJpZCI6Nn0.BCGPvSOtUY0HkcjOljZR6OCUj7jeygWIGWxDeoCwWVo";
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //GPS_Api_CallAsync(y.ToString(), 51.503364051, -0.1276250, 1, 1, 20);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (wifi.WifiState.ToString().Equals("Enabled"))
			{
				//Wifi Toggle ON
			}
            /*
            if (bluetooth.Adapter.IsEnabled)
            {
                //Bluetooth Toggle ON
            }
			*/
            Y = y;
            StartTimer();
        }

        private void StartTimer()
        {
            Device.StartTimer(System.TimeSpan.FromSeconds(10), () =>
            {
                Device.BeginInvokeOnMainThread(UpdateUserDataAsync);
                return true;
            });
        }

        private async void UpdateUserDataAsync()
        {
            await GPS_Api_CallAsync(Y.ToString(), 51.503364051, -0.1276250, 1, 1, 20);
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

        private async void D_Logs_Clicked(object sender, EventArgs e)
        {
		    Send_Data_Defi.IsVisible = false;
		    Map_View_Defi.IsVisible = false;
	    	Logs_Defi.IsVisible = true;
		    D_Logs.BackgroundColor = Color.FromHex("#0d47a1");
	    	D_Map_View.BackgroundColor = Color.FromHex("#3E88F2");
		    D_Send_Data.BackgroundColor = Color.FromHex("#3E88F2");
		    SentData.Text = "change gps co-ordinates to view value here";
            try
            {
                /*
                var locator = CrossGeolocator.Current;
			    locator.DesiredAccuracy = 50;
            	TimeSpan ts = new TimeSpan((Int64)10e12 + 3456789);
                var position = await locator.GetPositionAsync(timeout: ts);
                SentData.Text += position.Latitude + " ";
                */
                
                await StartListening();
            }

            catch (Exception excep)
            {
			    SentData.Text += excep.ToString();
            }
        }
        async Task StartListening()
	    {
		    await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true, new Plugin.Geolocator.Abstractions.ListenerSettings
		    {
			    ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
			    AllowBackgroundUpdates = true,
		    	DeferLocationUpdates = true,
		    	DeferralDistanceMeters = 1,
			    DeferralTime = TimeSpan.FromSeconds(1),
		    	ListenForSignificantChanges = true,
		    	PauseLocationUpdatesAutomatically = false
		    });
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
	    }

        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
		    Device.BeginInvokeOnMainThread(() =>
            {
                var test = e.Position;
                SentData.Text += "Full: Lat: " + test.Latitude.ToString() + " Long: " + test.Longitude.ToString();
            });
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
        private async Task GPS_Api_CallAsync(string T, double Latitude, double Longitude, int User_id, int Company_id, int Rpm)
        {
            var json = JsonConvert.SerializeObject(new
            {
                token = T,
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
            if (request.IsSuccessStatusCode)
            {
                CalledCounter.Text = "Api Called " + i + " times";
                TimeStamp.Text = "Last Called at " + DateTime.Now.ToLocalTime().ToString();
                i++;
            }
            else
            {
                CalledCounter.Text = "Something went wrong" + request.IsSuccessStatusCode.ToString();
            }
        }
    }
}