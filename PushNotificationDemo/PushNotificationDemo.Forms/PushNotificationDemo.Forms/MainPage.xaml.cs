using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PushNotificationDemo.Forms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            var pushNotificationManager = DependencyService.Get<INotificationHubManager>();
            if (pushNotificationManager != null)
            {

                await pushNotificationManager.RegisterDeviceForUserAsync(UserIdEntry.Text);
            }
        }
    }
}
