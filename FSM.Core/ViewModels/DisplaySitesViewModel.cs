using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class DisplaySitesViewModel
    {
        public Guid SiteDetailId { get; set; }
        public string Name { get; set; }
        public string StrataManagerName { get; set; }
        public string StreetName { get; set; }
        public Nullable<int> StreetType { get; set; }
        public string StreeTypeText { get; set; }
        public string State { get; set; }
        public string Suburb { get; set; }
        public Nullable<int> TypeOfResidence { get; set; }
        public string TypeOfResidenceText { get; set; }
        public Nullable<int> RoofType { get; set; }
        public string RoofTypeText { get; set; }
        public Nullable<int> DownPipe { get; set; }
        public Nullable<int> Contracted { get; set; }
        public Nullable<Guid> ContactId { get; set; }
        public Nullable<Guid> StrataManagerId { get; set; }
        public string Unit { get; set; }
        public string SiteAddress { get; set; }
        public Nullable<int> PostalCode { get; set; }
        public string Street { get; set; }
        public string SiteFileName { get; set; }
        public string Notes { get; set;}
    }
}
