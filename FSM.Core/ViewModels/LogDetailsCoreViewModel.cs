﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.ViewModels
{
    public class LogDetailsCoreViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Thread { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        
    }
}
