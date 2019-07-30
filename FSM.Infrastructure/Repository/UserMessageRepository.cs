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
    public class UserMessageRepository : GenericRepository<FsmContext, UserMessage>, IUserMessageRepository
    {
        public IQueryable<UserMessageCoreViewModel> GetMessageList(string threadId)
        {
            string sql = @"
                            update  UserMessage set IsMessageRead=1 where MessageThreadID='" + threadId + @"'

                            select um.Message,um.IsMessageRead,emp.UserName,um.From_Id,um.CreatedDate
                            from UserMessage um
                            left join dbo.AspNetUsers emp on emp.Id=um.From_Id
                            where um.MessageThreadID='" + threadId + "'";

            var CustomerSiteList = Context.Database.SqlQuery<UserMessageCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
    }
}
