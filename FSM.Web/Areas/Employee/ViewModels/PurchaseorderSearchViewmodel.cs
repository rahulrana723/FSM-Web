﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class PurchaseorderSearchViewmodel
    {
        public string SearchKeyword { get; set; }
        public int PageSize { get; set; }
    }
}