using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class CustomerReminderRepository:GenericRepository<FsmContext, CustomerReminder>, ICustomerReminderRepository
    {

    }
}
