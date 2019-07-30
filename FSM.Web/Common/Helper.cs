using FSM.Core.Interface;
using FSM.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSM.Core.Entities;
using System.Threading.Tasks;

namespace FSM.Web.Common
{
    public class Helper
    {
        public IUserDeviceTokenMapping UserDeviceTokenRepo { get; set; }
        public IEmployeeDetailRepository EmployeeDetailRepo { get; set; }
        public INotificationRepository NotificationRepo { get; set; }
        public Helper()
        {
            UserDeviceTokenRepo = new UserDeviceTokenRepository();
            EmployeeDetailRepo = new EmployeeDetailRepository();
            NotificationRepo = new NotificationRepository();
        }

        // save and send notifications
        public void SaveAndSendNotification(Guid FromId, Guid ToId, Guid NotificationTypeId, string NotificationType)
        {
            Task.Run(() =>
            {
            PushN objPushN = new PushN();
            // getting list of device token associated with user
            var lstUserDeviceTokenMapping = UserDeviceTokenRepo.FindBy(m => m.UserId == ToId).ToList();
            // getting employee record
            var employeeDetail = EmployeeDetailRepo.FindBy(m => m.EmployeeId == FromId).FirstOrDefault();

            // creating/saving notification
            Notifications Notifications = new Notifications();
            Notifications.Id = Guid.NewGuid();
            if (NotificationType == "Message")
            {
                Notifications.NotificationMessage = employeeDetail.FirstName + " " + employeeDetail.LastName + " sends you a message.";
            }
            else if (NotificationType == "Job")
            {
                Notifications.NotificationMessage = employeeDetail.FirstName + " " + employeeDetail.LastName + " assigned a new job to you.";
            }
            Notifications.NotificationType = NotificationType;
            Notifications.NotificationTypeId = NotificationTypeId;
            Notifications.UserId = ToId ;
            Notifications.CreatedBy = FromId;
            Notifications.CreatedDate = DateTime.Now;
            NotificationRepo.Add(Notifications);
            NotificationRepo.Save();

            // sending notifications
            foreach (UserDeviceTokenMapping obj in lstUserDeviceTokenMapping)
            {
                if (obj.DeviceToken != "0")
                {
                    objPushN.PushToIphone(obj.DeviceToken, employeeDetail.FirstName + " " + employeeDetail.LastName +
                    " sends you a message.", System.Configuration.ConfigurationManager.AppSettings["NotificationPassword"],
                    Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsProduction"]), System.Configuration.
                    ConfigurationManager.AppSettings["IphoneCertificateLocation"]);
                }
            }
            });
        }
    }
}