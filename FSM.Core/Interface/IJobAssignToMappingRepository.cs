using FSM.Core.Entities;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Interface
{
    public interface IJobAssignToMappingRepository: IGenericRepository<JobAssignToMapping>
    {
        IQueryable<GetSuperVisorCoreViewModel> GetSuperVisorList(Guid JobId);
        IQueryable<JobDataVM> GetJobDataByDate(DateTime DateBooked);
        IQueryable<JobDataViewModel> GetJobByDateBooked(DateTime dateBooked);
    }
}
