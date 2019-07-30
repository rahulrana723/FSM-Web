using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSM.Core.Entities
{
    public class Notifications
    {
        [Key]
        public Guid Id { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationType { get; set; }
        public Nullable<Guid> NotificationTypeId { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
