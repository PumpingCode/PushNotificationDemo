using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationDemo.Forms
{
    public interface INotificationHubManager
    {
        void RegisterDeviceForUser(string userId);
    }
}
