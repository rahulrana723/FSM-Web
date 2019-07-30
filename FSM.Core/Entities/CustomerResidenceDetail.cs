using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class CustomerResidenceDetail
    {
        [Key]
        public Guid ResidenceDetailId { get; set; }
        [ForeignKey("CustomerSiteDetail")]
        public Guid SiteDetailId { get; set; }
        public Nullable<int> TypeOfResidence { get; set; }
        public string Unit { get; set; }
        public string NoBldgs { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Pitch { get; set; }
        public Nullable<int> RoofType { get; set; }
        public Nullable<int> GutterGaurd { get; set; }
        public Nullable<bool> NotWet { get; set; }
        public Nullable<bool> NeedTwoPPL { get; set; }
        public int SRASinstalled { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerSiteDetail CustomerSiteDetail { get; set; }
    }


}
