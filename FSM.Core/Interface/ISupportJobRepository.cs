﻿using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface ISupportJobRepository: IGenericRepository<SupportJob>
    {
        IQueryable<SupportJobCoreViewModel> GetJobsForSupport();
        IQueryable<SupportJobCoreViewModel> GetSupportJobListBySearchKeyword(string keyword);
    }
}
