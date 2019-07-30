using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FSM.Web.FSMConstant;
using System.ComponentModel;

namespace FSM.Web.Areas.Customer.ViewModels
{
    public class CustomerResidenceDetailViewModel
    {
        [Key]
        public Guid ResidenceDetailId { get; set; }
        public Guid SiteDetailId { get; set; }
        [DisplayName("Type of Residence")]
        public Nullable<Constant.ResidenceType> TypeOfResidence { get; set; }
        public string DisplayResidenceType { get; set; }
        [StringLength(20)]
        public string Unit { get; set; }
        [StringLength(20)]
        [DisplayName("No Bldgs")]
        public string NoBldgs { get; set; }
        public Constant.ResidenceHeight Height { get; set; }
        public string DisplayResidenceHeight { get; set; }
        public Constant.RoofPitch Pitch { get; set; }
        public string DisplayRoofPitch { get; set; }
        [DisplayName("Roof Type")]
        public Constant.RoofType RoofType { get; set; }
        public string DisplayRoofType { get; set; }
        [DisplayName("Not Wet")]
        public bool NotWet { get; set; }
        [DisplayName("Multiple person")]
        public bool NeedTwoPPL { get; set; }
        [DisplayName("SRAS Installed")]
        public Constant.SRAS SRASinstalled { get; set; }
        [DisplayName("Gutter Guard")]
        public Constant.GutarGuard GutterGaurd { get; set; }
        public string DisplayGutterGaurd { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}