using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Firebase.Iid;
using System.Threading.Tasks;
using Android.Gms.Common;
using Android.Content;
using Firebase.Messaging;
using Android.Preferences;

namespace PushNotificationDemo.Forms.Droid
{
    [Activity(Label = "PushNotificationDemo.Forms", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
		protected override void OnCreate(Bundle bundle)
		{
		    TabLayoutResource = Resource.Layout.Tabbar;
		    ToolbarResource = Resource.Layout.Toolbar;

		    base.OnCreate(bundle);

			// Check if app got activated with extras
			if (Intent.Extras != null)
			{
				var message = (RemoteMessage)Intent.Extras.Get("message");
				if (message != null)
				{
					// App got activated by notification
					// Do your stuff...
				}
			}

            // Check, if Google Play Services are available, that are mandatory for Google Firebase Push
            if (GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this) != ConnectionResult.Success)
	            System.Diagnostics.Debug.WriteLine($"Google Play Services are not available for this devie");

			// Initialize Xamarin.Forms
		    global::Xamarin.Forms.Forms.Init(this, bundle);
		    LoadApplication(new App());
		}
    }
}