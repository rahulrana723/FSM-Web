using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerSearchViewModel
    {
        public string CustomerLastName { get; set; }
        [DisplayName("Customer Type")]
        public Constant.CustomerSearchType CustomerType { get; set; }
        public List<string> GetUserRoles { get; set; }
        public int PageSize { get; set; }
    }
}