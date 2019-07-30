using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Admin.ViewModels
{
    public class AspNetRolesViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Name is required !")]
        public string Name { get; set; }
        public string PageSize { get; set; }
        public string grid_column { get; set; }
        public string grid_dir { get; set; }
        public string grid_page { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}