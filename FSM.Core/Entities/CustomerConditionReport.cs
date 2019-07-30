using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class CustomerConditionReport
    {
        [Key]
        public Guid ConditionReportId { get; set; }
        [ForeignKey("CustomerSiteDetail")]
        public Guid SiteDetailId { get; set; }
        public Nullable<int> RoofTilesSheets { get; set; }
        public Nullable<int> BargeCappings { get; set; }
        public Nullable<int> RidgeCappings { get; set; }
        public Nullable<int> Valleys { get; set; }
        public Nullable<int> Flashings { get; set; }
        public Nullable<int> Gutters { get; set; }
        public Nullable<int> DownPipes { get; set; }
        public string ConditionNote { get; set; }
        public Nullable<int> ConditionRoof { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual CustomerSiteDetail CustomerSiteDetail { get; set; }
        public Nullable<int> ConditionOfRoof { get; set; }
    }

}
