using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class EmailViewModels
    {
        public string To { get; set; }
        public string From { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }

    }

    public class MessageViewModel
    {
        public string message { get; set; }
        public string sendTo_name { get; set; }
        public string sendTo_number { get; set; }
        public string sentFrom_number { get; set; }

    }

    public class FileInformation
    {
        public string filename { get; set; }
        public string filepath { get; set; }
    }
}