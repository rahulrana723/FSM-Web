using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
   public class CustomerSitesDocuments
    {
        [Key]
        public Guid DocumentId { get; set; }
        [ForeignKey("CustomerGeneralInfo"), Column(Order = 0)]
        public Guid CustomerGeneralInfoId { get; set; }

        [ForeignKey("CustomerSiteDetail"), Column(Order = 1)]
        public Guid SiteId { get; set; }
        public string DocumentName { get; set; }
        public string DocType { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public virtual CustomerGeneralInfo CustomerGeneralInfo { get; set; }
        public virtual CustomerSiteDetail CustomerSiteDetail { get; set; }
    }
}
