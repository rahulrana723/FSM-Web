using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class ResidenceDetailViewModel
    {
        public Guid ResidenceDetailId { get; set; }
        public Guid SiteDetailId { get; set; }
        public Nullable<int> TypeOfResidence { get; set; }
        public string Unit { get; set; }
        public string NoBldgs { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Pitch { get; set; }
        public Nullable<int> RoofType { get; set; }
        public bool NotWet { get; set; }
        public bool NeedTwoPPL { get; set; }
        public Nullable<int> SRASinstalled { get; set; }
        public Nullable<int> GutterGaurd { get; set; }
    }
}