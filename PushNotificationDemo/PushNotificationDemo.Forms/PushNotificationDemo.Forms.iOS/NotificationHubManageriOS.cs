using Foundation;
using PushNotificationDemo.Forms.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindowsAzure.Messaging;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHubManageriOS))]
namespace PushNotificationDemo.Forms.iOS
{
    public class NotificationHubManageriOS : INotificationHubManager
    {
        private SBNotificationHub notificationHub;

        public NotificationHubManageriOS()
        {
            notificationHub = new SBNotificationHub("Endpoint=sb://pumpingcodedemo.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=+vU1I+8GGdPct3fANW5XbP03iC9Txe+/muGxUHe3e7g=", "PumpingCodeNotificationHub");
        }

        public async Task RegisterDeviceForUserAsync(string userId)
        {
            // Get APNS Token from preferences
            var token = (NSData)NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("ApnsToken"));
            if (token != null)
            {
                // Unregister all previous users with this token
                await notificationHub.UnregisterAllAsyncAsync(token);

                // Register user
                await notificationHub.RegisterNativeAsyncAsync(token, new NSSet("userId:" + userId));
            }            
        }    
    }
}
