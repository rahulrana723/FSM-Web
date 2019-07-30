﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FSM.Web.Areas.Employee.ViewModels
{
    public class ViewEmployeeRatesListViewModel
    {
        public IEnumerable<ViewEmployeeRatesViewModel> Rateslist { get; set; }
        public ViewEmployeeRatesSearchViewModel RatesListsearchmodel { get; set; }

    }
}