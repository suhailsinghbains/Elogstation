using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Elogstation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        public async Task SaveTodoItemAsync(string Username, string Password)
        {
            var json = JsonConvert.SerializeObject(new { username = Username, password = Password });
            string data = json.ToString();
            HttpClient client = new HttpClient(new NativeMessageHandler())
            {
                MaxResponseContentBufferSize = 256000
            };
            var RestUrl = "http://54.208.49.250/api/auth";
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(RestUrl, content);
            request.Headers.Add("postman-token", "5d07c886-04b0-6880-16ed-7b025bdef6ad");
            var response = await request.Content.ReadAsStringAsync();
            if (request.IsSuccessStatusCode)
            {
                Test.Text = response;
                await Task.Delay(1000);
                await Navigation.PushAsync(new Page1(Username, response.ToString()));
            }
            else
            {
                Test.Text += "LOGIN Failed with the following error\n";
                Test.Text = response;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page1(Username.ToString(), Password.ToString()));
            //await SaveTodoItemAsync(Username.Text, Password.Text);
        }
    }
}