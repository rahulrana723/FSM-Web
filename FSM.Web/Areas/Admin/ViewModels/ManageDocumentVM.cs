using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class ManageDocumentVM
    {
        public List<ImportantDocumentVM> ImportantDocumentList { get; set; }
        public HttpPostedFileBase FilePosted { get; set; }
        [MaxLength(300, ErrorMessage = "Description max length is 300")]
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool IsTermAndConditionDoc { get; set; }
    }
}