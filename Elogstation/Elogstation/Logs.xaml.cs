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
using System.Threading;
using Java.Util;

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

        DateTime D_Start_Driving_Time = DateTime.Now, D_Start_Not_Driving_Time = DateTime.Now, D_Start_Sleeper_Time = DateTime.Now, D_Start_Off_Duty_Time = DateTime.Now;
        DateTime D_End_Driving_Time = DateTime.Now, D_End_Not_Driving_Time = DateTime.Now, D_End_Sleeper_Time = DateTime.Now, D_End_Off_Duty_Time = DateTime.Now;

        private void Keep_Record()
        {
            Label C_Status = this.FindByName<Label>("Current_Status");
            if (C_Status.Text == "Current Status: Driving")
            {
                if (Driving == 0)
                {
                    D_Start_Driving_Time = DateTime.Now;
                }
                B_Driving.IsEnabled = false;
                Driving=1;
                End_Other_Times("Driving");
                D_End_Driving_Time = DateTime.Now;
                SHOW_Driving.Text = D_Start_Driving_Time.ToString() + "\n" + D_End_Driving_Time;
            }
            else if (C_Status.Text == "Current Status: Not Driving")
            {
                if (Not_Driving == 0)
                {
                    D_Start_Not_Driving_Time = DateTime.Now;
                }
                B_Not_Driving.IsEnabled = false;
                Not_Driving=1;
                End_Other_Times("Not Driving");
                D_End_Not_Driving_Time = DateTime.Now;
                SHOW_Not_Driving.Text = D_Start_Not_Driving_Time.ToString() + "\n" + D_End_Not_Driving_Time;
            }
            else if(C_Status.Text == "Current Status: Sleeper")
            {
                if (Sleeper == 0)
                {
                    D_Start_Sleeper_Time = DateTime.Now;
                }
                B_Sleeper.IsEnabled = false;
                Sleeper=1;
                End_Other_Times("Sleeper");
                D_End_Sleeper_Time = DateTime.Now;
                SHOW_Sleeper.Text = D_Start_Sleeper_Time.ToString() + "\n" + D_End_Sleeper_Time;
            }
            else if(C_Status.Text == "Current Status: Off Duty")
            {
                if (Off_Duty == 0)
                {
                    D_Start_Off_Duty_Time = DateTime.Now;
                }
                B_Off_Duty.IsEnabled = false;
                Off_Duty=1;
                End_Other_Times("Off Duty");
                D_End_Off_Duty_Time = DateTime.Now;
                SHOW_Off_Duty.Text = D_Start_Off_Duty_Time + "\n" + D_End_Off_Duty_Time;
            }
            void End_Other_Times(string x)
            {
                if (Off_Duty == 1 && x != "Off Duty")
                {
                    Off_Duty = 0;
                    D_End_Off_Duty_Time = DateTime.Now;
                    SHOW_Off_Duty.Text = D_Start_Off_Duty_Time + "\n" + D_End_Off_Duty_Time;
                    B_Off_Duty.IsEnabled = true;
                }
                else if (Sleeper == 1 && x != "Sleeper")
                {
                    Sleeper = 1;
                    D_End_Sleeper_Time = DateTime.Now;
                    SHOW_Sleeper.Text = D_Start_Sleeper_Time.ToString() + "\n" + D_End_Sleeper_Time;
                    B_Sleeper.IsEnabled = true;
                }
                else if (Driving == 1 && x != "Driving")
                {
                    Driving = 0;
                    D_End_Driving_Time = DateTime.Now;
                    SHOW_Driving.Text = D_Start_Driving_Time.ToString() + "\n" + D_End_Driving_Time;
                    B_Driving.IsEnabled = true;
                }
                else if (Not_Driving == 1 && x != "Not Driving")
                {
                    Not_Driving = 0;
                    D_End_Not_Driving_Time = DateTime.Now;
                    SHOW_Not_Driving.Text = D_Start_Not_Driving_Time.ToString() + "\n" + D_End_Not_Driving_Time;
                    B_Not_Driving.IsEnabled = true;
                }
                else
                {
                    //Nothing;
                }
            }
            var x1 = D_End_Driving_Time - D_Start_Driving_Time;
            var x2 = D_End_Not_Driving_Time - D_Start_Not_Driving_Time;
            var x3 = D_End_Off_Duty_Time - D_Start_Off_Duty_Time;
            var x4 = D_End_Sleeper_Time - D_Start_Sleeper_Time;
            SHOW_Total_Time.Text = "Total Time: " + (x1+x2+x3+x4) + "s";
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