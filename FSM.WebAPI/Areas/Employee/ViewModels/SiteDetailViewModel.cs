using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Areas.Employee.ViewModels
{
    public class SiteDetailViewModel
    {
        public Guid SiteDetailId { get; set; }
        public Guid CustomerGeneralInfoId { get; set; }

        public string SiteFileName { get; set; }
      
        public string Unit { get; set; }
    
        public string Street { get; set; }
  
        public string StreetName { get; set; }
   
        public Nullable<int> StreetType { get; set; }
      
        public string Suburb { get; set; }
     
        public string State { get; set; }
       
        public Nullable<int> PostalCode { get; set; }
    
        public bool MarkAsPreferred { get; set; }
        public bool IsPrimaryAddress { get; set; }
       
        public string ScheduledPrice { get; set; }
  
        public bool BlackListed { get; set; }
        
        public string BlackListReason { get; set; }
        public Nullable<int> Contracted { get; set; }
      
        public Nullable<int> PrefTimeOfDay { get; set; }
        public string filecount { get; set; }
  
        public string StrataPlan { get; set; }
    }
}