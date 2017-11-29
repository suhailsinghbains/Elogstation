using ModernHttpClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.Content;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CoreFoundation.DispatchSource;
using System.Threading;

namespace Elogstation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Logs : TabbedPage
    {
        int Driving=0, Not_Driving=0, Sleeper=0, Off_Duty=0;
        public Logs(string parameter)
        {
            InitializeComponent();
            this.Title = parameter;
            Initial_API_CallAsync(this, null);
            StartTimer();
        }
        
        private void StartTimer()
        {
            Device.StartTimer(System.TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(Keep_Record);
                return true;
            });
        }

        private void Keep_Record()
        {
            Label Test = this.FindByName<Label>("Current_Status");
            if (Test.Text == "Current Status: Driving")
            {
                Driving++;
                SHOW_Driving.Text = Driving.ToString();
            }
            else if (Test.Text == "Current Status: Not Driving")
            {
                Not_Driving++;
                SHOW_Not_Driving.Text = Not_Driving.ToString();
            }
            else if(Test.Text == "Current Status: Sleeper")
            {
                Sleeper++;
                SHOW_Sleeper.Text =Sleeper.ToString(); 
            }
            else if(Test.Text == "Current Status: Off Duty")
            {
                Off_Duty++;
                SHOW_Off_Duty.Text = Off_Duty.ToString();
            }
            SHOW_Total_Time.Text = "Total Time: " + (Driving + Not_Driving + Off_Duty + Sleeper).ToString() + "s";
        }

        private async void Initial_API_CallAsync(object sender, EventArgs e)
        {
            await API_CallAsync();
        }

        private async Task API_CallAsync()
        {
            HttpClient client = new HttpClient(new NativeMessageHandler())
            {
                MaxResponseContentBufferSize = 256000
            };
            var RestUrl = "https://reqres.in/api/users/2";
            var response = await client.GetAsync(RestUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                JObject results = JObject.Parse(content);
                Id.Text ="ID: " + results["data"]["id"].ToString();
                Name.Text = "Name: " + results["data"]["first_name"].ToString() + results["data"]["last_name"].ToString();
                Current_Status.Text = "Current Status: " + "Off Duty";
                B_Off_Duty.BackgroundColor = Color.FromHex("#0d47a1");
                B_Sleeper.BackgroundColor = Color.FromHex("#3E88F2");
                B_Driving.BackgroundColor = Color.FromHex("#3E88F2");
                B_Not_Driving.BackgroundColor = Color.FromHex("#3E88F2");
            }
            else
            {
                Error.IsVisible = true;
                Error.Text = response.ToString();
            }
        }

        private void Off_Duty_Clicked(object sender, EventArgs e)
        {
            Current_Status.Text = "Current Status: " + "Off Duty";
            B_Off_Duty.BackgroundColor = Color.FromHex("#0d47a1");
            B_Sleeper.BackgroundColor = Color.FromHex("#3E88F2");
            B_Driving.BackgroundColor = Color.FromHex("#3E88F2");
            B_Not_Driving.BackgroundColor = Color.FromHex("#3E88F2");
        }
        private void Sleeper_Clicked(object sender, EventArgs e)
        {
            Current_Status.Text = "Current Status: " + "Sleeper";
            B_Sleeper.BackgroundColor = Color.FromHex("#0d47a1");
            B_Off_Duty.BackgroundColor = Color.FromHex("#3E88F2");
            B_Driving.BackgroundColor = Color.FromHex("#3E88F2");
            B_Not_Driving.BackgroundColor = Color.FromHex("#3E88F2");
        }

        private void Driving_Clicked(object sender, EventArgs e)
        {
            Current_Status.Text = "Current Status: " + "Driving";
            B_Driving.BackgroundColor = Color.FromHex("#0d47a1");
            B_Off_Duty.BackgroundColor = Color.FromHex("#3E88F2");
            B_Sleeper.BackgroundColor = Color.FromHex("#3E88F2");
            B_Not_Driving.BackgroundColor = Color.FromHex("#3E88F2");
        }

        private void Not_Driving_Clicked(object sender, EventArgs e)
        {
            Current_Status.Text = "Current Status: " + "Not Driving";
            B_Not_Driving.BackgroundColor = Color.FromHex("#0d47a1");
            B_Off_Duty.BackgroundColor = Color.FromHex("#3E88F2");
            B_Sleeper.BackgroundColor = Color.FromHex("#3E88F2");
            B_Driving.BackgroundColor = Color.FromHex("#3E88F2");
        }

        private void Certify_Logs_Clicked(object sender, EventArgs e)
        {
            Certify_Condition.IsVisible = true;
            Space_for_Certification.IsVisible = false;
        }

        private void Accept_Clicked(object sender, EventArgs e)
        {
            //Acceptance Api Call
        }

        private void Reject_Clicked(object sender, EventArgs e)
        {
            Certify_Condition.IsVisible = false;
            Space_for_Certification.IsVisible = true;
        }
    }
}