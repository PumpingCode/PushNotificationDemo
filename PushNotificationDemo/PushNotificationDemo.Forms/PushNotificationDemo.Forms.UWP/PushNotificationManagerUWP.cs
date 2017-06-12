using Microsoft.WindowsAzure.Messaging;
using PushNotificationDemo.Forms.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHubManagerUWP))]
namespace PushNotificationDemo.Forms.UWP
{
    public class NotificationHubManagerUWP : INotificationHubManager
    {
        private NotificationHub notificationHub;
        private ApplicationDataContainer localSettings;

        public NotificationHubManagerUWP()
        {
            notificationHub = new NotificationHub("PumpingCodeNotificationHub", "Endpoint=sb://pumpingcodedemo.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=+vU1I+8GGdPct3fANW5XbP03iC9Txe+/muGxUHe3e7g=");
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        }

        public async void RegisterDeviceForUser(string userId)
        {
            // Get WNS token from preferences            
            if (localSettings.Values.ContainsKey("WindowsNotificationChannelUri"))
            {
                var channelUri = (string)localSettings.Values["WindowsNotificationChannelUri"];

                // Unregister all previous users with this token
                await notificationHub.UnregisterAllAsync(channelUri);

                // Register user
                var registration = await notificationHub.RegisterNativeAsync(channelUri, new List<string> { "userId:" + userId });
            }
        }
    }
}
