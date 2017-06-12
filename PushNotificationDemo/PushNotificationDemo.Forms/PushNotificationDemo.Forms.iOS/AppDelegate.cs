using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreTelephony;

namespace PushNotificationDemo.Forms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            RegisterForPushNotificationsAsync();

            return base.FinishedLaunching(app, options);
        }

        private void RegisterForPushNotificationsAsync()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());
                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            NSUserDefaults.StandardUserDefaults.SetValueForKey(deviceToken, new NSString("ApnsToken"));
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != userInfo && userInfo.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

                string alert = string.Empty;

                //Extract the alert text
                // NOTE: If you're using the simple alert by just specifying
                // "  aps:{alert:"alert msg here"}  ", this will work fine.
                // But if you're using a complex alert with Localization keys, etc.,
                // your "alert" object from the aps dictionary will be another NSDictionary.
                // Basically the JSON gets dumped right into a NSDictionary,
                // so keep that in mind.
                if (aps.ContainsKey(new NSString("alert")))
                    alert = (aps[new NSString("alert")] as NSString).ToString();


                // Differ between the state of the current application, when it receives the push notification
                switch (application.ApplicationState)
                {
                    // App is running in the foreground and notification will get depressed
                    // We have to handle the notification on our own
                    case UIApplicationState.Active:
                        if (!string.IsNullOrEmpty(alert))
                        {
                            // Manually show an alert
                            var alertView = new UIAlertView() { Title = "Notification", Message = alert };
                            alertView.AddButton("Ok");
                            alertView.Show();                            
                        }
                        break;
                    case UIApplicationState.Inactive:                        
                    case UIApplicationState.Background:
                    default:
                        break;
                }
            }
        }
    }
}
