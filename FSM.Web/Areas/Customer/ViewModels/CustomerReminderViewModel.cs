﻿using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerReminderViewModel
    {
        [Key]
        public Guid ReminderId { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public List<Nullable<Guid>> JobId2 { get; set; }
        public Nullable<int> JobNo { get; set; }
        [DisplayName("Log Date")]
        public string EnteredBy { get; set; }
        [DisplayName("Message")]
        public string Note { get; set; }
        [DisplayName("From Email")]
        public Constant.FromEmail? FromEmail { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<int> ViewJobid { get; set; }
        public List<System.Web.Mvc.SelectListItem> JobList { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> JobListIds { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> FinalJobList { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> ContactList { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> ContactListIds { get; set; }
        [DisplayName("Reminder Date")]
        public Nullable<DateTime> ReminderDate { get; set; }
        [DisplayName("Message Type")]
        public Nullable<Constant.MessageType> MessageTypeId { get; set; }
        [DisplayName("Template Message")]
        public Nullable<Constant.DashboardTemplateMessage> TemplateMessageId { get; set; }
        [Display(Name ="SMS")]
        public bool HasSMS { get; set; }
        [Display(Name = "Email")]
        public bool HasEmail { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }
        public string CustomerId { get; set; }
        public Nullable<DateTime> ReContactDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}