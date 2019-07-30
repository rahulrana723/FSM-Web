using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
   public class GetLatitudeLongitudeCoreviewModel
    {
        public Guid Employeeid { get; set; }
        public string UserName { get; set; }

        public string EID { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
    }
}
