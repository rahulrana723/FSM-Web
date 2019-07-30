using FSM.WebAPI.Controllers;
using FSM.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.WebAPI.Common
{
    public class CommonFunctions
    {
        public string GetCalculatedTime(DateTime? StartTime, DateTime? endtime)
        {
            // Difference in days, hours, and minutes.
            string ActualTime = string.Empty;
            TimeSpan timeSpentOnJob = Convert.ToDateTime(endtime) - Convert.ToDateTime(StartTime);
            string hours = Convert.ToString(timeSpentOnJob.Hours);
            string minutes = Convert.ToString(timeSpentOnJob.Minutes);
            string seconds = Convert.ToString(timeSpentOnJob.Seconds);
            string hourText = " Hour:";
            string minuteText = " Minute:";
            string SecondText = " Second";

            if (hours != "1")
            {
                hourText = " Hours:";
            }
            if (minutes != "1")
            {
                minuteText = " Minutes:";
            }
            if (seconds != "1")
            {
                SecondText = " Seconds";
            }
            return ActualTime = hours + hourText + minutes + minuteText + seconds + SecondText;
        }

        public static ServiceResponse<UserInfoViewModel> GetUserInfoByToken()
        {
            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            AccountController account = new AccountController();
            response = account.GetUserInfo();
            return response;
        }

    }


}