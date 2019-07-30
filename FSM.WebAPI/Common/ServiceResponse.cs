using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Common
{
    public class ServiceResponse<T> where T : class
    {
        public List<T> ResponseList { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseErrorMessage { get; set; }
    }
}