using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class ContactLogSiteContactsMapping
    {
        public Guid Id { get; set; }
        public Nullable<Guid> ContactId { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public Nullable<Guid> ContactLogId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
    }
}
