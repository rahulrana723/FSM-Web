using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class WebNotifications
    {
        [Key]
        public Guid Id { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationType { get; set; }
        public Nullable<Guid> NotificationTypeId { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}
