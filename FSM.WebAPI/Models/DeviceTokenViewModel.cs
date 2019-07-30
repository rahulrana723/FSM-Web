using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Models
{
    public class DeviceTokenViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}