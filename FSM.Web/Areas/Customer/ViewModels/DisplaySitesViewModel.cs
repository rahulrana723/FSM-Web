using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FSM.Web.FSMConstant;
using System.ComponentModel;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class DisplaySitesViewModel
    {
        public Guid SiteDetailId { get; set; }
        [DisplayName("Contact Name")]
        public string Name { get; set; }
        public string StrataManagerName { get; set; }
        public string StreetName { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public Nullable<Constant.RoofType> RoofType { get; set; }
        public Nullable<Constant.Frequency> Contracted { get; set; }
        public Nullable<Constant.HomeAddressStreetType> StreetType { get; set; }
        public Nullable<Constant.DownPipe> DownPipe { get; set; }
        public Nullable<Constant.ResidenceType> TypeOfResidence { get; set; }
        public string DisplayResidenceType { get; set; }
        public string DisplayDownPipe { get; set; }
        public string DisplayRoofType { get; set; }
        public string IsContracted { get; set; }
        public string DisplayStreetType { get; set; }
        public string SiteFileName { get; set; }
        public string Notes { get; set; }

    }
}