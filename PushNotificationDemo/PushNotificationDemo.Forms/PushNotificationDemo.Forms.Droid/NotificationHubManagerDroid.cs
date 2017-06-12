using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WindowsAzure.Messaging;
using Android.Preferences;
using PushNotificationDemo.Forms.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHubManagerDroid))]
namespace PushNotificationDemo.Forms.Droid
{
    public class NotificationHubManagerDroid : INotificationHubManager
    {
        private NotificationHub notificationHub;
        private ISharedPreferences sharedPreferences;

        public NotificationHubManagerDroid()
        {
            notificationHub = new NotificationHub("PumpingCodeNotificationHub", "Endpoint=sb://pumpingcodedemo.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=+vU1I+8GGdPct3fANW5XbP03iC9Txe+/muGxUHe3e7g=", Application.Context);
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

        }

        public void RegisterDeviceForUser(string userId)
        {
            // Get Firebase Token from preferences
            var token = sharedPreferences.GetString("FirebaseInstanceToken", null);
            if (token != null)
            {
                // Unregister all previous users with this token
                notificationHub.UnregisterAll(token);

                // Register user
                var registration = notificationHub.Register(token, "userId:" + userId);
            }
        }
    }
}