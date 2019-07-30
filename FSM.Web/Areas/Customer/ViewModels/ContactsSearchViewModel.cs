using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class ContactsSearchViewModel
    {
        public Guid CustomerGeneralInfoId { get; set; }
        public FSMConstant.Constant.ContactsType ContactsType{get;set;}
        public Guid SiteId { get; set; }
        public List<SelectListItem> SiteSearch { get; set; }
        public string Name { get; set; }
        public int PageSize { get; set; }
    }
}