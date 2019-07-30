using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class CustomerEmailSendViewModel
    {
        public string To { get; set; }
        public string From { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Docid { get; set; }
        public string BillingEmail { get; set; }
       
        public Nullable<Constant.EmailTemplates> EmailTemplates { get; set; }
        public Nullable<Constant.FromEmail> FromEmail { get; set; }
        public string BillingContact { get; set; }
        public List<JobDocumentsDetail> jobDocuments { get; set; }
        public List<BillingContactList> BillingContacts { get; set; }

        public string Message { get; set; }
        public List<Nullable<Guid>> CustomerIDs { get; set; }
        public List<SelectListItem> BillingContactList { get; set; }
        [AllowHtml]
        public string TemplateSelected { get; set; }

        public Guid? CustomerGeneralInfoId { get; set; }
    }
    public class JobDocumentsDetail
    {
        public Guid Id { get; set; }
        public string DocName { get; set; }

    }

    public class BillingContactList
    {
        public Guid Id { get; set; }
        public string Emailid { get; set; }

    }

    public class EmailSendViewModel
    {
        public string To { get; set; }
        public string From { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Docid { get; set; }
        public string BillingEmail { get; set; }
        public List<Nullable<Guid>> CustomerIDs { get; set; }

        public Nullable<Constant.EmailTemplates> EmailTemplates { get; set; }
        public Nullable<Constant.FromEmail> FromEmail { get; set; }
        public string BillingContact { get; set; }
        public List<JobDocumentsDetail> jobDocuments { get; set; }
        public List<BillingContactList> BillingContacts { get; set; }

        public string Message { get; set; }
        [AllowHtml]
        public string TemplateSelected { get; set; }
        public List<SelectListItem> BillingContactList { get; set; }


    }
}