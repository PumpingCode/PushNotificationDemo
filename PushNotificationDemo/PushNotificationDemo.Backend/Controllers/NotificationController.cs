using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PushNotificationDemo.Backend.Controllers
{
    [RoutePrefix("api/notification")]
    public class NotificationController : ApiController
    {
        private readonly NotificationHubClient notificationHub;

        public NotificationController()
        {
            //notificationHub = NotificationHubClient.CreateClientFromConnectionString("<CONNECTION_STRING>", "<HUB_NAME>");            

            notificationHub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://pumpingcodedemo.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=EoSsB6kgtYaPwyK8qp51RyKzrRb9sCiFUbwUrHKDmdU=", 
                "PumpingCodeNotificationHub");
        }

        [HttpPost]
        [Route("register/{registrationId}/{userId}")]        
        public async Task<HttpResponseMessage> Register(string registrationId, string userId)
        {
            // Get registration for the provided registrationId from the Notification Hub
            // It should find one as this gets called, after the device registered itself at the Notification Hub
            var registration = await notificationHub.GetRegistrationAsync<RegistrationDescription>(registrationId);
            if (registration == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            
            // Add userId to registration
            if (registration.Tags == null)
                registration.Tags = new HashSet<string>();

            registration.Tags.Add("userId:" + userId);

            // Update the registration at the Notification Hub
            await notificationHub.CreateOrUpdateRegistrationAsync(registration);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("sendalert/{userId}/{message}")]        
        public async Task<HttpResponseMessage> SendAlertNotification(string userId, string message)
        {
            // Get all registrations for this user
            var registrations = await notificationHub.GetRegistrationsByTagAsync("userId:" + userId, 100);

            // Interate trough all registrations, check for which platform they are and send the message in the appropriate format
            foreach (var registration in registrations)
            {
                string payload;
                switch (registration.GetType().Name)
                {
                    // APNS (iOS)
                    case nameof(AppleRegistrationDescription):
                        payload = "{\"aps\":{\"alert\":\"" + message + "\"}}";
                        await notificationHub.SendAppleNativeNotificationAsync(payload);
                        break;

                    // GCM (Android)
                    case nameof(GcmRegistrationDescription):
                        payload = "{ \"data\" : {\"message\":\"" + message + "\"}}";
                        await notificationHub.SendGcmNativeNotificationAsync(payload);
                        break;

                    // WNS (Windows)
                    case nameof(WindowsRegistrationDescription):
                        payload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + message + "</text></binding></visual></toast>";
                        await notificationHub.SendWindowsNativeNotificationAsync(payload);
                        break;

                    // ...                    
                }                   
            }

            // TODO: Make sure to check the outcome of the Push Notification send to handle errors!

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("sendraw/{userId}/{message}")]
        public async Task<HttpResponseMessage> SendRawNotification(string userId, string message)
        {
            // Create generic and platform independent payload, that every platform has to deal with by themself
            var payload = new Dictionary<string, string>
            {
                { "message", "Notification Hub test notification" },
                { "date", DateTime.Now.ToString() }
            };

            // Send this payload to every registration of the user
            await notificationHub.SendTemplateNotificationAsync(payload, "userId:" + userId);              

            // TODO: Make sure to check the outcome of the Push Notification send to handle errors!

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
