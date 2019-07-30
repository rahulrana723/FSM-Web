using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class MessageListViewModel
    {
        public IEnumerable<UserMessageViewModel> UserMessageForCoreViewModel { get; set; }
        public UserMessageViewModel UserMessageViewModel { get; set; }
        public IEnumerable<UserMessageCoreViewModel> UserMessageCoreViewModel { get; set; }
        public UserMessageThreadviewModel UserMessageThreadviewModel { get; set; }
    }
}