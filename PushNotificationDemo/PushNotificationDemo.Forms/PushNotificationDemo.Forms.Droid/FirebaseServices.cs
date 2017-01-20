using Android.App;
using Firebase.Iid;
using Firebase.Messaging;
using Android.Content;
using PushNotificationDemo.Forms.Droid;
using WindowsAzure.Messaging;
using Android.Preferences;
using System;
using System.Diagnostics;

namespace FirebaseServices
{
	[Service]
	[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class MyFirebaseIIDService : FirebaseInstanceIdService
	{
		public override void OnTokenRefresh()
		{
			var hub = new NotificationHub("PumpingCodeNotificationHub", "Endpoint=sb://pumpingcodedemo.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=+vU1I+8GGdPct3fANW5XbP03iC9Txe+/muGxUHe3e7g=", this);
			var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

			// Get Firebase Instance Token
			var refreshedToken = FirebaseInstanceId.Instance.Token;

			// Check, if a Firebase Instance Token has been registered before and unregister it
			var oldToken = sharedPreferences.GetString("FirebaseInstanceToken", null);
			if (oldToken != null)
				hub.UnregisterAll(oldToken);

			// Save the Firebase Instance Token locally
			var sharedPreferencesEditor = sharedPreferences.Edit();
			sharedPreferencesEditor.PutString("FirebaseInstanceToken", refreshedToken);
			sharedPreferencesEditor.Commit();

			// Register token at Azure Notification Hub
			var result = hub.Register(refreshedToken);
			if (result != null)
			{
				Debug.WriteLine($"Registration successful: {result.RegistrationId}");
			}
		}
	}

	[Service]
	[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class MyFirebaseMessagingService : FirebaseMessagingService
	{
		public override void OnMessageReceived(RemoteMessage message)
		{
			// Create an intent that gets started when the user taps the notification
			var intent = new Intent(this, typeof(MainActivity));
			intent.AddFlags(ActivityFlags.ClearTop);
			intent.PutExtra("message", message);
			var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

			// Get notification message
			string messageBody = null;
			if (message.GetNotification()?.Body != null)
				// Message has a Notification part from wich to take the body from
				messageBody = message.GetNotification().Body;
			else if (message.Data.ContainsKey("message"))
				// Message has a data part from wich to take the body from
				messageBody = message.Data["message"];

			// Create notification
			var notificationBuilder = new Notification.Builder(this)
				.SetContentTitle("Push Notification Demo")
			    .SetContentText(messageBody)
				//.SetAutoCancel(true)
			    .SetSmallIcon(Resource.Drawable.icon)
				.SetDefaults(NotificationDefaults.All)
				.SetPriority((int)NotificationPriority.High)
				.SetContentIntent(pendingIntent);

			// Show notification
			var notificationManager = NotificationManager.FromContext(this);
			notificationManager.Notify(0, notificationBuilder.Build());
		}
	}
}