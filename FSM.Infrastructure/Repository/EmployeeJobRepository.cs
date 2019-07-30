using FSM.Core.Entities;
using FSM.Core.Interface;
using FSM.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
    public class EmployeeJobRepository : GenericRepository<FsmContext, Jobs>, IEmployeeJobRepository
    {
        public int GetMaxJobID()
        {

            int count = Context.Jobs.Count();
            string sql = "";
            if (count > 0)
            {
                sql = "SELECT IDENT_CURRENT ('jobs')+1 AS Current_Identity";
            }
            else
            {
                sql = "SELECT IDENT_CURRENT ('jobs') AS Current_Identity";
            }
            return Convert.ToInt32(Context.Database.SqlQuery<decimal>(sql).FirstOrDefault());
        }

        public int GetMaxJobNo()
        {
            string sql = "SELECT COALESCE(MAX(JobNo),0) FROM dbo.Jobs";
            return (Convert.ToInt32(Context.Database.SqlQuery<int>(sql).FirstOrDefault()) + 1);
        }

        public IQueryable<EmployeeJobVieweModel> GetEmployeeCustomerJobs(Guid custGeneralInfoId)
        {

            string sql = @"Select * from(
select distinct jb.id
               ,jb.WorkType
               ,jb.JobNo
               ,jb.JobId
               ,jb.Status
               ,( SELECT InvoiceNo = 
                   STUFF((SELECT DISTINCT ' / ' + CONVERT(NVARCHAR(100),InvoiceNo) +' '+ CONVERT(NVARCHAR(100), InvoiceType) 
                          FROM invoice b 
                          WHERE b.EmployeeJobId = a.EmployeeJobId 
                         FOR XML PATH('')), 1, 2, '')
                            FROM Invoice a
                        WHERE a.EmployeeJobId = jb.id 
                         GROUP BY EmployeeJobId) As InvoiceDetails
                ,(SELECT MAX(UserTimeSheet.JobDate) FROM UserTimeSheet
                INNER JOIN Jobs ON Jobs.Id = UserTimeSheet.JobId
                WHERE Jobs.Id = jb.id  AND Jobs.Status = 15) AS CompletionDate
               ,jb.CustomerGeneralInfoId
               ,jb.SiteId
               ,jb.JobType
               ,jb.CreatedDate
               ,custdetail.Contracted,custdetail.StreetName
               ,ANU.UserName AssignUser
                ,(cc.FirstName +' '+cc.LastName) as ContactName
                ,cust.CustomerLastName CustomerName
                ,cust.StrataPlan
                ,cust.StrataNumber
                ,users.UserName BookedByName
                ,jb.datebooked
                ,ISNULL(NULLIF(custdetail.Street, '')+ '  ', '') + ISNULL(NULLIF(custdetail.StreetName, '') + ', ', '')+ISNULL(NULLIF(custdetail.Suburb, '')+'  ', '')+ISNULL(NULLIF(custdetail.state, ''), '')+ CONVERT(varchar(10), custdetail.PostalCode) as SiteAddress
                from Jobs jb 
                Left join AspNetUsers users on users.id=jb.BookedBy
                Left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                Left join CustomerSiteDetail custdetail on jb.SiteId=custdetail.SiteDetailId
                left join CustomerContacts cc on custdetail.ContactId=cc.ContactId
                left join CustomerResidenceDetail CRD on CRD.SiteDetailId = custdetail.SiteDetailId
                left join JobAssignToMapping Jap on jb.id=Jap.JobId
                left join AspNetUsers ANU on jap.AssignTo=ANU.Id 
                WHERE CRD.NeedTwoPPL = 0 OR CRD.NeedTwoPPL is NULL
                AND jb.IsDelete=0
                
               UNION 
                
                select distinct jb.id
               ,jb.WorkType
               ,jb.JobNo
               ,jb.JobId
               ,jb.Status
               ,( SELECT InvoiceNo = 
                   STUFF((SELECT DISTINCT ' / ' + CONVERT(NVARCHAR(100),InvoiceNo) +' '+ CONVERT(NVARCHAR(100), InvoiceType) 
                          FROM invoice b 
                          WHERE b.EmployeeJobId = a.EmployeeJobId 
                         FOR XML PATH('')), 1, 2, '')
                            FROM Invoice a
                        WHERE a.EmployeeJobId = jb.id 
                         GROUP BY EmployeeJobId) As InvoiceDetails
                ,(SELECT MAX(UserTimeSheet.JobDate) FROM UserTimeSheet
                INNER JOIN Jobs ON Jobs.Id = UserTimeSheet.JobId
                WHERE Jobs.Id = jb.id  AND Jobs.Status = 15) AS CompletionDate
               ,jb.CustomerGeneralInfoId
               ,jb.SiteId
               ,jb.JobType
               ,jb.CreatedDate
               ,custdetail.Contracted,custdetail.StreetName
               ,(SELECT UserName = STUFF((
                            SELECT ', ' + ANU.UserName
                            from JobAssignToMapping jap
                INNER JOIN  AspNetUsers ANU on jap.AssignTo=ANU.Id 
                where jap.JobId = jb.Id and jap.IsDelete=0
                            FOR XML PATH('')
                            ), 1, 1, '')) AssignUser
                ,(cc.FirstName +' '+cc.LastName) as ContactName
                ,cust.CustomerLastName CustomerName
                ,cust.StrataPlan
                ,cust.StrataNumber
                ,users.UserName BookedByName
                ,jb.datebooked
                ,ISNULL(NULLIF(custdetail.Street, '')+'  ', '') + ISNULL(NULLIF(custdetail.StreetName, '') + ' ', '')+ISNULL(NULLIF(custdetail.Suburb, '')+'  ', '')+ISNULL(NULLIF(custdetail.state, ''), '')+ CONVERT(varchar(10), custdetail.PostalCode) as SiteAddress
                from Jobs jb 
                Left join AspNetUsers users on users.id=jb.BookedBy
                Left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                Left join CustomerSiteDetail custdetail on jb.SiteId=custdetail.SiteDetailId
                left join CustomerContacts cc on custdetail.ContactId=cc.ContactId
                left join CustomerResidenceDetail CRD on CRD.SiteDetailId = custdetail.SiteDetailId
                WHERE CRD.NeedTwoPPL = 1 AND jb.IsDelete=0)t
                WHERE CustomerGeneralInfoId = '" + custGeneralInfoId + "'";
            var CustomerSiteList = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<EmployeeJobVieweModel> GetEmployeeJobs()
        {

            string sql = @"Select * from(
                select distinct jb.id
               ,jb.WorkType
               ,jb.JobNo
               ,jb.JobId
               ,jb.Status
               ,jb.CustomerGeneralInfoId
               ,jb.SiteId
               ,jb.JobType
               ,custdetail.Contracted,custdetail.StreetName
               ,custdetail.SiteFileName
               ,ANU.UserName AssignUser
                ,(cc.FirstName +' '+cc.LastName) as ContactName
                ,cust.CustomerLastName CustomerName
                ,cust.StrataPlan
                ,cust.StrataNumber
                ,users.UserName BookedByName
                ,jb.datebooked
               ,(custdetail.SiteFileName+' '+custdetail.Suburb) as SiteAddress
               -- ,ISNULL(NULLIF(custdetail.Street, '')+'  ', '') + ISNULL(NULLIF(custdetail.StreetName, '') + ', ', '')+ISNULL(NULLIF(custdetail.Suburb, '')+'  ', '')+ISNULL(NULLIF(custdetail.state, ''), '')+ CONVERT(varchar(10), custdetail.PostalCode) as SiteAddress
                from Jobs jb 
                Left join AspNetUsers users on users.id=jb.BookedBy
                Left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                Left join CustomerSiteDetail custdetail on jb.SiteId=custdetail.SiteDetailId
                left join CustomerContacts cc on custdetail.ContactId=cc.ContactId
                left join CustomerResidenceDetail CRD on CRD.SiteDetailId = custdetail.SiteDetailId
                left join JobAssignToMapping Jap on jb.id=Jap.JobId
                left join AspNetUsers ANU on jap.AssignTo=ANU.Id 
                WHERE (jb.IsDelete=0) AND (CRD.NeedTwoPPL = 0 OR CRD.NeedTwoPPL is NULL) AND Jap.IsDelete=0
                
               UNION 
                
                select distinct jb.id
               ,jb.WorkType
               ,jb.JobNo
               ,jb.JobId
               ,jb.Status
               ,jb.CustomerGeneralInfoId
               ,jb.SiteId
               ,jb.JobType
               ,custdetail.Contracted,custdetail.StreetName
                ,(custdetail.SiteFileName+' '+custdetail.Suburb) as SiteFileName
               ,(SELECT UserName = STUFF((
                            SELECT distinct ', ' + ANU.UserName
                            from JobAssignToMapping jap
                INNER JOIN  AspNetUsers ANU on jap.AssignTo=ANU.Id 
                where jap.IsDelete=0 AND  jap.JobId = jb.Id
                            FOR XML PATH('')
                            ), 1, 1, '')) AssignUser
                ,(cc.FirstName +' '+cc.LastName) as ContactName
                ,cust.CustomerLastName CustomerName
                ,cust.StrataPlan
                ,cust.StrataNumber
                ,users.UserName BookedByName
                ,jb.datebooked
                 ,(custdetail.SiteFileName+' '+custdetail.Suburb) as SiteAddress
               -- ,ISNULL(NULLIF(custdetail.Street, '')+'  ', '') + ISNULL(NULLIF(custdetail.StreetName, '') + ', ', '')+ISNULL(NULLIF(custdetail.Suburb, '')+'  ', '')+ISNULL(NULLIF(custdetail.state, ''), '')+ CONVERT(varchar(10), custdetail.PostalCode) as SiteAddress
                from Jobs jb 
                Left join AspNetUsers users on users.id=jb.BookedBy
                Left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                Left join CustomerSiteDetail custdetail on jb.SiteId=custdetail.SiteDetailId
                left join CustomerContacts cc on custdetail.ContactId=cc.ContactId
                left join CustomerResidenceDetail CRD on CRD.SiteDetailId = custdetail.SiteDetailId
                WHERE (jb.IsDelete=0) AND (CRD.NeedTwoPPL = 1 ))t 
                ";


            var CustomerSiteList = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<EmployeeJobVieweModel> GetJobInfoList()
        {

            string sql = @"select distinct js.JobId,js.Id EmployeeJobId, js.JobType,sd.Suburb,sd.StreetName,cs.CustomerLastName,ISNULL(inv.InvoiceStatus, 0 ) InvoiceStatus,inv.Id InvoiceId
                            from 
                            dbo.Jobs js
                         left join dbo.CustomerSiteDetail sd on sd.SiteDetailId=js.SiteId
						left join dbo.CustomerGeneralInfo cs on cs.CustomerGeneralInfoId=js.CustomerGeneralInfoId
						left join dbo.Invoice inv on inv.JobId=js.JobId";


            var CustomerSiteList = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobsByJobid(string jobid)
        {
            string sql = @"select Top 1 jb.id, jb.JobId,jb.JobNo,jb.Siteid SiteId,ISNULL(c.FirstName,'') ContactName,ISNULL(c.PhoneNo1,'') ContactNo,ISNULL(jb.status, 0 ) status,ISNULL(s.Latitude, 0 ) Latitude,ISNULL(s.Longitude, 0 ) Longitude,
                        ISNULL(NULLIF(s.Street, '')+'  ', '') + ISNULL(NULLIF(s.StreetName, '') + ', ', '')+ISNULL(NULLIF(s.Suburb, '')+'  ', '')+ISNULL(NULLIF(s.state, '')+'  ', '')+ CONVERT(varchar(10), s.PostalCode) as Address
                        ,ISNULL(jb.JobType, 0 ) JobType,ISNULL(jb.WorkType,0) WorkType,jb.DateBooked,ISNULL(jb.PreferTime, 0 ) PreferTime,jb.BookedBy,jb.JobNotes,jb.OperationNotes
                        ,ISNull(s.Street,'') Street ,ISNull(s.streetName,'') StreetName ,ISNull(s.StreetType,0) StreetType ,ISNull(s.State,'') State, ISNull(s.PostalCode,0) PostalCode ,ISNull(s.Notes,'') SiteNotes
                        ,cust.CustomerLastName CustomerName
                        ,cust.CustomerNotes CustomerNotes
                        ,users.UserName BookedByName
                        ,jb.datebooked DateBooked
                        from jobs jb 
                        left join AspNetUsers users on users.id=jb.BookedBy
                        left join CustomerSiteDetail s on s.SiteDetailId=jb.Siteid
                           left join CustomerContacts c on c.SiteId=s.SiteDetailId
                        left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId  where jb.id='" + jobid + "'";
            var CustomerSiteList = Context.Database.SqlQuery<Dashboardjobdetailcoreviewmodel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobsInfo()
        {
            string sql = @"
DECLARE @DW int
DECLARE @CurrentDate datetime
SET @DW = (SELECT DATEPART(DW, GETDATE()))

IF(@DW = 7)
SET @CurrentDate = GETDATE() + 2
ELSE IF( @DW = 1)
SET @CurrentDate = GETDATE() + 1 
ELSE
SET @CurrentDate = GETDATE()

Select * from
                          (select distinct TOP 100 jb.id,ISNull(s.Suburb,'') suburb,Res.NotWet WetRequiredType,Res.Height StoreysType,jb.OTRWRequired
                          ,(SELECT COUNT(ID) FROM  JobAssignToMapping WHERE IsDelete=0 AND JobId = jb.Id ) OTRWAssignCount
                          ,s.SiteFileName as Address
                          ,s.StrataPlan
                         ,jb.JobNo, jb.JobId,jb.Siteid SiteId,ISNULL(jb.status, 0 ) status,ISNULL(s.Latitude, 0 ) Latitude,ISNULL(s.Longitude, 0 ) Longitude,
                        ISNULL(jb.JobType, 0 ) JobType,ISNULL(jb.WorkType,0) WorkType,ISNULL(jb.PreferTime, 0 ) PreferTime,jb.BookedBy,jb.JobNotes,jb.OperationNotes,jb.EstimatedHrsPerUser
                        ,ISNull(s.Street,'') Street ,ISNull(s.streetName,'') StreetName ,ISNull(s.StreetType,0) StreetType ,ISNull(s.State,'') State, ISNull(s.PostalCode,0) PostalCode ,
                        cust.CustomerLastName CustomerName
                        ,users.UserName BookedByName
                        ,cast(jb.DateBooked as date) DateBooked
                        ,cust.StrataNumber
                        from jobs jb 
                        left join AspNetUsers users on users.id=jb.BookedBy
                        left join CustomerSiteDetail s on s.SiteDetailId=jb.Siteid
                        left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId 
                        left join CustomerResidenceDetail Res on Res.SiteDetailId = s.SiteDetailId
                        where (jb.status!=13) AND(jb.IsDelete=0) AND(Cast(jb.DateBooked as date) = CAST(@CurrentDate as date)))t
                        where ((t.OTRWRequired <>  t.OTRWAssignCount) or (t.OTRWRequired is null and t.OTRWAssignCount=0))";
            var CustomerSiteList = Context.Database.SqlQuery<Dashboardjobdetailcoreviewmodel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<Dashboardjobdetailcoreviewmodel> GetJobsOnFullMap(string date, string userId)
        {
            string userSql = string.Empty;
            if (string.IsNullOrEmpty(userId))
            {
                userSql = " AND JobAssignToMapping.AssignTo = ISNULL(null,JobAssignToMapping.AssignTo)";
            }
            else
            {
                userSql = " AND JobAssignToMapping.AssignTo = ISNULL('" + userId + "',JobAssignToMapping.AssignTo)";
            }
            string sql = @"SELECT Jobs.Id,Jobs.JobNo,Jobs.DateBooked,CustomerSiteDetail.Latitude ,CustomerSiteDetail.Longitude,CustomerSiteDetail.StreetName
                             FROM Jobs
                             INNER JOIN CustomerSiteDetail ON Jobs.SiteId = CustomerSiteDetail.SiteDetailId 
                             LEFT JOIN JobAssignToMapping ON JobAssignToMapping.JobId = Jobs.Id
                              WHERE JobAssignToMapping.AssignTo is not null
                             AND convert(date,Jobs.DateBooked) = convert(date,'" + date + "')" + userSql + "";
            var CustomerSiteList = Context.Database.SqlQuery<Dashboardjobdetailcoreviewmodel>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<JobScheduleViewModel> SaveEndTime(Guid JobId, string EndTime, string StartTime, Guid ResourceId)
        {
            string userSql = string.Empty;
            string userSql1 = string.Empty;

            DateTime dFrom = DateTime.Parse(StartTime);
            DateTime dTo = DateTime.Parse(EndTime);

            double TS = Math.Round((dTo - dFrom).TotalHours, 2);

            //if (string.IsNullOrEmpty(Convert.ToString(JobNo)))
            if (JobId == Guid.Empty || JobId == null)
            {
                userSql = null;
            }
            else
            {
                userSql = @"UPDATE Jobs set EndTime=convert(datetime,'" + dTo + "',103), StartTime=convert(datetime,'" + dFrom + "',103), EstimatedHours=(SELECT EstimatedHours+" + TS + "-EstimatedHrsPerUser from Jobs where Id='" + JobId + "') where Id = '" + JobId + "'";
                userSql1 = @"UPDATE JobAssignToMapping set StartTime=convert(datetime,'" + dFrom + "',103),EndTime=convert(datetime,'" + dTo + "',103), EstimatedHrsPerUser=(SELECT EstimatedHrsPerUser+" + TS + "-EstimatedHrsPerUser from Jobs where Id='" + JobId + "' )where AssignTo='" + ResourceId + "' and JobAssignToMapping.JobId =(SELECT Id from Jobs where Id='" + JobId + "')";
            }
            var List = Context.Database.SqlQuery<JobScheduleViewModel>(userSql).AsQueryable();
            var List1 = Context.Database.SqlQuery<JobScheduleViewModel>(userSql1).AsQueryable();
            var ListFinal = List.Concat(List1);
            return ListFinal;
        }
        public string GetJobTimeSpent(string jobid)
        {
            string sql = @"select CAST(DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', TimeSpent)), '00:00:00.000') as Time) as TimeSpent
                            from UserTimeSheet
                            Where JobId = 'E280658A-D041-4859-AEB7-0A5573464009'
                            AND Reason = 'job'";

            return Context.Database.SqlQuery<string>(sql).FirstOrDefault();
        }

        public IQueryable<EmployeeJobVieweModel> GetJobInfoListBySearchkeyword(string keyword, string status)
        {

            string sql;
            if (status == "0")
            {
                sql = @"SELECT * FROM(select distinct js.JobId,js.Id EmployeeJobId, js.JobType,sd.Suburb,sd.StreetName,cs.CustomerLastName,ISNULL(inv.InvoiceStatus,0) InvoiceStatus,inv.Id InvoiceId
                            from 
                            dbo.Jobs js
                         left join dbo.CustomerSiteDetail sd on sd.SiteDetailId=js.SiteId
						left join dbo.CustomerGeneralInfo cs on cs.CustomerGeneralInfoId=js.CustomerGeneralInfoId
						left join dbo.Invoice inv on inv.JobId=js.JobId where ISNULL(inv.InvoiceStatus,0)=0)t
                            Where  t.StreetName Like '%" + keyword + "%'Or t.CustomerLastName Like '%" + keyword + "%'  Or t.JobId Like '%" + keyword + "%' Or t.Suburb Like '%" + keyword + "%' ";
            }
            else if (status == "1")
            {
                sql = @"SELECT * FROM(select distinct js.JobId,js.Id EmployeeJobId, js.JobType,sd.Suburb,sd.StreetName,cs.CustomerLastName,inv.InvoiceStatus InvoiceStatus,inv.Id InvoiceId
                            from 
                            dbo.Jobs js
                         left join dbo.CustomerSiteDetail sd on sd.SiteDetailId=js.SiteId
						left join dbo.CustomerGeneralInfo cs on cs.CustomerGeneralInfoId=js.CustomerGeneralInfoId
						left join dbo.Invoice inv on inv.JobId=js.JobId where inv.InvoiceStatus=1)t
                            Where t.StreetName Like '%" + keyword + "%'Or t.CustomerLastName Like '%" + keyword + "%'  Or t.JobId Like '%" + keyword + "%'  Or t.Suburb Like '%" + keyword + "%' ";
            }
            else
            {
                sql = @"SELECT * FROM(select distinct js.JobId,js.Id EmployeeJobId, js.JobType,sd.Suburb,sd.StreetName,cs.CustomerLastName,inv.InvoiceStatus InvoiceStatus,inv.Id InvoiceId
                            from 
                            dbo.Jobs js
                         left join dbo.CustomerSiteDetail sd on sd.SiteDetailId=js.SiteId
						left join dbo.CustomerGeneralInfo cs on cs.CustomerGeneralInfoId=js.CustomerGeneralInfoId
						left join dbo.Invoice inv on inv.JobId=js.JobId)t
                            Where t.StreetName Like '%" + keyword + "%'Or t.CustomerLastName Like '%" + keyword + "%'  Or t.JobId Like '%" + keyword + "%' Or t.Suburb Like '%" + keyword + "%' ";

            }

            var CustomerSiteList = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<JobScheduleViewModel> GetJobSchedule(string Jobtype = "null", string Status = "null", string selectdate = "null", string searchkey = "")
        {
            try
            {
                DateTime myDateTime = DateTime.Now;
                DayOfWeek DayOfWeek;
                string sqlForDayHrs = "";
                string sqlForPublicDays = "";
                DateTime? SelectDateForPublicHoliday = DateTime.Now;

                if (selectdate != "null")
                {
                    DayOfWeek = Convert.ToDateTime(selectdate).DayOfWeek;
                    //  selectdate = "'" + selectdate + "'";
                    SelectDateForPublicHoliday = Convert.ToDateTime(selectdate);
                    selectdate = "'" + Convert.ToDateTime(selectdate).ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    DayOfWeek = myDateTime.DayOfWeek;
                    if (Convert.ToString(DayOfWeek) == "Saturday")
                    {
                        myDateTime = myDateTime.AddDays(2);
                    }
                    else if (Convert.ToString(DayOfWeek) == "Sunday")
                    {
                        myDateTime = myDateTime.AddDays(1);
                    }
                    DayOfWeek = myDateTime.DayOfWeek;
                    selectdate = "'" + myDateTime.Date.ToString("yyyy-MM-dd") + "'";
                }


                switch (Convert.ToString(DayOfWeek))
                {
                    case "Monday":
                        sqlForDayHrs = " (MondayHrs = 0 OR MondayHrs IS NULL)";
                        break;
                    case "Tuesday":
                        sqlForDayHrs = " (TuesdayHrs = 0 OR TuesdayHrs IS NULL)";
                        break;
                    case "Wednesday":
                        sqlForDayHrs = " (WednesdayHrs = 0 OR WednesdayHrs IS NULL)";
                        break;
                    case "Thursday":
                        sqlForDayHrs = " (ThursdayHrs = 0 OR ThursdayHrs IS NULL)";
                        break;
                    case "Friday":
                        sqlForDayHrs = " (FridayHrs = 0 OR FridayHrs IS NULL)";
                        break;
                    case "Saturday":
                        sqlForDayHrs = " (Saturdayhrs = 0 OR Saturdayhrs IS NULL)";
                        break;
                    case "Sunday":
                        sqlForDayHrs = " (SundayHrs = 0 OR SundayHrs IS NULL)";
                        break;
                    default:
                        sqlForDayHrs = "";
                        break;
                }
                FsmContext db = new FsmContext();
                var objPublicHoliday = db.PublicHoliday.Where(m => m.Date == SelectDateForPublicHoliday).FirstOrDefault();

                if (objPublicHoliday != null)
                {
                  sqlForPublicDays = @"SELECT 
         'Public Holiday' AS JobNo
        , NULL JobId
        ,CAST('06:00:00' as time) StartTime
        ,CAST('21:00:00'  as time) EndTime
        ,CAST('1900-01-01 00:00:00.000' as datetime) MinJobDate
        ,CAST('1900-01-01 00:00:00.000' as datetime) MaxJobDate
        ,'0.00' Latitude
        ,'0.00' Longitude
        ,'0000' Sitpostalcode
        ,cast(" + selectdate + @" as datetime) DateBooked
        ,'' SiteName
        ,''Address
        ,''StrataPlan
        ,IsNull(empdetail.FirstName + ' ' + empdetail.LastName, '') as Employeename
        ,empdetail.EmployeeId as EmployeeId
      ,CAST(0 as bit) WetRequiredType
        ,0 StoreysType
        ,0 OTRWAssignCount
        ,0 JobNumber
        ,0 JobNoVal
        ,'' CustomerLastName
        ,'' StrataNumber
        ,0 JobType
        ,0 WorkType
        ,0 PreferTime
        ,'' Jobnotes
        ,0 JobStatus
        ,CAST(0 as float) EstimatedHrsPerUser
        ,'' Suburb
        from  EmployeeDetail empdetail
        where empdetail.IsDelete = 0 And empdetail.IsActive = 1
        AND empdetail.Role = '31cf918d-b8fe-4490-b2d7-27324bfe89b4'";
                    return Context.Database.SqlQuery<JobScheduleViewModel>(sqlForPublicDays).AsQueryable();
                }

                string sqlOtrwNotWorking = @"SELECT 
         'Not Working' AS JobNo
        , NULL JobId
        ,'06:00:00' StartTime
        ,'21:00:00' EndTime
        ,'' MinJobDate
        ,'' MaxJobDate
        ,'0.00' Latitude
        ,'0.00' Longitude
        ,'0000' Sitpostalcode
        ,cast(" + selectdate + @" as datetime) DateBooked
        ,'' SiteName
        ,''Address
        ,''StrataPlan
        ,IsNull(empdetail.FirstName + ' ' + empdetail.LastName, '') as Employeename
        ,empdetail.EmployeeId as EmployeeId
        ,'' WetRequiredType
        ,'' StoreysType
        ,'' OTRWAssignCount
        ,'' JobNumber
        ,'' JobNoVal
        ,'' CustomerLastName
        ,'' StrataNumber
        ,'' JobType
        ,'' WorkType
        ,'' PreferTime
        ,'' Jobnotes
        ,'' JobStatus
        ,'' EstimatedHrsPerUser
        ,'' Suburb
        from  EmployeeDetail empdetail
        where empdetail.IsDelete = 0 And empdetail.IsActive = 1
        AND empdetail.EmployeeId not in (SELECT AssignTo from JobAssignToMapping where IsDelete = 0 and DateBooked = (cast(" + selectdate + @" as datetime)))
        AND empdetail.Role = '31cf918d-b8fe-4490-b2d7-27324bfe89b4' AND " + sqlForDayHrs + "";


                string sql = @"SELECT 'JobId: ' + CAST(JobNoVal as nvarchar(50)) JobNo
            ,jobid
             ,CASE WHEN AssignedStartTime IS NULL
                   THEN '06:00:00.0000000'
				   WHEN StartTime <= '06:00:00.0000000'
                   THEN '06:00:00.0000000'
                   WHEN StartTime != cast(AssignedStartTime As time)
                   THEN StartTime
                   ELSE cast(AssignedStartTime As time)
              END StartTime
            , CASE WHEN AssignedEndTime IS NULL
                   THEN '09:00:00.0000000'
                   --WHEN EndTime <= '06:00:00.0000000'
                   --THEN '07:00:00.0000000'
                   WHEN DATEDIFF(ss, StartTime, EndTime)/60 < 60
                   THEN  DATEADD(MINUTE,(DATEDIFF(ss, StartTime, EndTime) /60) + (60 - (DATEDIFF(ss, StartTime, EndTime) /60)) , StartTime)
                   WHEN DATEDIFF(ss, AssignedStartTime, AssignedEndTime)/60 < 60
                   THEN  DATEADD(MINUTE,(DATEDIFF(ss, cast(AssignedStartTime As time), cast(AssignedEndTime As time)) /60) + (60 - (DATEDIFF(ss, cast(AssignedStartTime As time), cast(AssignedEndTime As time)) /60)) , cast(AssignedStartTime As time))
                   WHEN EndTime > cast(AssignedEndTime As time)
                   THEN EndTime 
                   ELSE cast(AssignedEndTime As time)
             END EndTime
            , CAST((CASE WHEN MinJobDate IS NULL
                   THEN ISNULL(DateBooked, '1900-01-01 00:00:00.000')
                   ELSE MinJobDate
               END)as datetime)   MinJobDate
            ,CAST((CASE WHEN MaxJobDate IS NULL
                   THEN ISNULL(DateBooked, '1900-01-01 00:00:00.000')
                   ELSE MaxJobDate
               END) as datetime)   MaxJobDate
            ,Latitude
            ,Longitude
            ,Sitpostalcode
            ,isnull(DateBooked, '') DateBooked
            ,SiteName
            ,Address
            ,StrataPlan
            ,Employeename
            ,EmployeeId
            ,WetRequiredType
            ,StoreysType
            ,OTRWAssignCount
            ,JobNumber
            ,JobNoVal
            ,CustomerLastName
            ,StrataNumber
            ,JobType
            ,WorkType
            ,PreferTime
            ,Jobnotes
            ,JobStatus
            ,EstimatedHrsPerUser
            ,Suburb
            FROM
            (
            select job.jobid as JobNo
            , job.Id jobid
            ,Res.NotWet WetRequiredType
            ,Res.Height StoreysType
            ,(SELECT COUNT(ID) FROM  JobAssignToMapping WHERE JobId = job.Id AND IsDelete=0 AND DateBooked= " + selectdate + @") OTRWAssignCount
            , Job.JobId JobNumber
            , Job.JobNo JobNoVal
            , CustomerGeneralInfo.CustomerLastName CustomerLastName
            , CustomerGeneralInfo.StrataNumber
            , job.JobType
            , job.WorkType
            , job.PreferTime
            , job.Jobnotes
            , JobAssignToMapping.Status JobStatus
            , job.EstimatedHrsPerUser EstimatedHrsPerUser
            , sitedetail.Suburb
            , (select MIN(StartTime) from UserTimeSheet where JobId = job.Id and UserId = JobAssignToMapping.AssignTo and JobDate = " + selectdate + @") as StartTime
            ,(select MAX(EndTime) from UserTimeSheet where JobId = job.Id and UserId = JobAssignToMapping.AssignTo and JobDate = " + selectdate + @") EndTime
            ,(select MIN(JobDate) from UserTimeSheet where JobId = job.Id and UserId = JobAssignToMapping.AssignTo ) as MinJobDate
            ,(select MAX(JobDate) from UserTimeSheet where JobId = job.Id and UserId = JobAssignToMapping.AssignTo ) MaxJobDate
            ,Convert(varchar(20)
            , sitedetail.Latitude) Latitude
            ,Convert(varchar(20)
            , sitedetail.Longitude)Longitude
            ,Convert(varchar(20)
            , sitedetail.PostalCode) as Sitpostalcode
            ,JobAssignToMapping.DateBooked
            ,sitedetail.Street + ' ' + sitedetail.StreetName SiteName
            ,sitedetail.SiteFileName as Address
            ,sitedetail.StrataPlan
            --,ISNULL(NULLIF(sitedetail.Street, '') + '  ', '') + ISNULL(NULLIF(sitedetail.StreetName, '') + ', ', '') + ISNULL(NULLIF(sitedetail.Suburb, '') + '  ', '') + ISNULL(NULLIF(sitedetail.state, '') + '  ', '') + CONVERT(varchar(10), ISNULL(sitedetail.PostalCode, '')) as Address
            ,IsNull(empdetail.FirstName + ' ' + empdetail.LastName, '') as Employeename
            ,empdetail.EmployeeId as EmployeeId
            ,JobAssignToMapping.StartTime AssignedStartTime
            , JobAssignToMapping.EndTime AssignedEndTime
            from EmployeeDetail empdetail
            left join JobAssignToMapping on JobAssignToMapping.AssignTo = empdetail.EmployeeId
            left join jobs job on job.Id = JobAssignToMapping.JobId
            left join CustomerGeneralInfo on CustomerGeneralInfo.CustomerGeneralInfoId = job.CustomerGeneralInfoId
            left join CustomerSiteDetail sitedetail on job.SiteId = sitedetail.SiteDetailId
            left join CustomerResidenceDetail Res on Res.SiteDetailId = sitedetail.SiteDetailId
            left join AspNetUserRoles on AspNetUserRoles.UserId = empdetail.EmployeeId
            left join AspNetRoles  on AspNetRoles.Id = AspNetUserRoles.RoleId
            where AspNetRoles.Name = 'OTRW' and empdetail.IsDelete = 0 And empdetail.IsActive = 1 ANd JobAssignToMapping.DateBooked = " + selectdate + @"
            AND JobAssignToMapping.AssignTo is not null AND JobAssignToMapping.IsDelete=0
            )t
        UNION
        SELECT
         'On Leave' AS JobNo
        , NULL JobId
        ,'06:00:00' StartTime
        ,'21:00:00' EndTime
        ,StartDate MinJobDate
        , EndDate MaxJobDate
        ,'0.00' Latitude
        ,'0.00' Longitude
        ,'0000' Sitpostalcode
        ,cast(" + selectdate + @" as datetime) DateBooked
        ,'' SiteName
        ,''Address
        ,''StrataPlan
        ,IsNull(empdetail.FirstName + ' ' + empdetail.LastName, '') as Employeename
        ,empdetail.EmployeeId as EmployeeId
        ,'' WetRequiredType
        ,'' StoreysType
        ,'' OTRWAssignCount
        ,'' JobNumber
        ,'' JobNoVal
        ,'' CustomerLastName
        ,'' StrataNumber
        ,'' JobType
        ,'' WorkType
        ,'' PreferTime
        ,'' Jobnotes
        ,'' JobStatus
        ,'' EstimatedHrsPerUser
        ,'' Suburb
        from  EmployeeDetail empdetail
        left JOIN  Vacation ON empdetail.EmployeeId = Vacation.EmployeeId
        left join AspNetUserRoles on AspNetUserRoles.UserId = empdetail.EmployeeId
        left join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId
        where AspNetRoles.Name = 'OTRW' and empdetail.IsDelete = 0 And empdetail.IsActive = 1
        AND Vacation.Status = 2  AND Vacation.RoastedOffId is null AND (cast(" + selectdate + @" as datetime) between Vacation.StartDate and Vacation.EndDate)
        UNION
  SELECT
         'Not Working' AS JobNo
        , NULL JobId
        ,'06:00:00' StartTime
        ,'21:00:00' EndTime
        , Vacation.StartDate MinJobDate
        , Vacation.EndDate MaxJobDate
        ,'0.00' Latitude
        ,'0.00' Longitude
        ,'0000' Sitpostalcode
        ,cast(" + selectdate + @" as datetime) DateBooked
        ,'' SiteName
        ,''Address
        ,''StrataPlan
        ,IsNull(empdetail.FirstName + ' ' + empdetail.LastName, '') as Employeename
        ,empdetail.EmployeeId as EmployeeId
        ,'' WetRequiredType
        ,'' StoreysType
        ,'' OTRWAssignCount
        ,'' JobNumber
        ,'' JobNoVal
        ,'' CustomerLastName
        ,'' StrataNumber
        ,'' JobType
        ,'' WorkType
        ,'' PreferTime
        ,'' Jobnotes
        ,'' JobStatus
        ,'' EstimatedHrsPerUser
        ,'' Suburb
        from  EmployeeDetail empdetail
        left JOIN  Vacation ON empdetail.EmployeeId = Vacation.EmployeeId
              left JOIN RoastedOff on RoastedOff.ID = Vacation.RoastedOffId
        left join AspNetUserRoles on AspNetUserRoles.UserId = empdetail.EmployeeId
        left join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId
        where AspNetRoles.Name = 'OTRW' and empdetail.IsDelete = 0 And empdetail.IsActive = 1
        AND RoastedOff.RoastedOnOff = 'Off' AND RoastedOff.IsDelete=0
        AND Vacation.EmployeeId not in (SELECT AssignTo from JobAssignToMapping where IsDelete = 0 and DateBooked = (cast(" + selectdate + @" as datetime)))
        AND Vacation.Status = 2 AND (cast(" + selectdate + @" as datetime) between Vacation.StartDate and Vacation.EndDate)       
        UNION
        " + sqlOtrwNotWorking + "";

                return Context.Database.SqlQuery<JobScheduleViewModel>(sql).AsQueryable();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IQueryable<AspNetUsersCore> GetOTRWUser()
        {
            string sql = @"SELECT AspNetUsers.Id,AspNetUsers.UserName  
                           FROM AspNetUsers
                           join AspNetUserRoles on AspNetUsers.Id = AspNetUserRoles.UserId
                           join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id
                           left Join EmployeeDetail ED on AspNetUsers.Id=ED.EmployeeId
                           WHERE AspNetRoles.Name = 'OTRW' And Ed.ISDelete=0 And Ed.IsActive=1 order by AspNetUsers.UserName asc";

            var userList = Context.Database.SqlQuery<AspNetUsersCore>(sql).AsQueryable();
            return userList;
        }

        public IQueryable<AspNetUsersCore> GetOTRWUserForWorkType(int WorkType)
        {
            string sql = @"SELECT distinct AspNetUsers.Id,AspNetUsers.UserName  
                           FROM AspNetUsers
                           join AspNetUserRoles on AspNetUsers.Id = AspNetUserRoles.UserId
                           join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id
                           join EmployeeDetail emp on AspNetUsers.id=emp.EmployeeId
                           join EmployeeWorkType EWT on emp.EmployeeId=EWT.EmployeeId
                           WHERE AspNetRoles.Name = 'OTRW' and EWT.WorkType='" + WorkType + "' And Emp.IsDelete=0 And emp.IsActive=1";

            var userList = Context.Database.SqlQuery<AspNetUsersCore>(sql).AsQueryable();
            return userList;
        }
        public IQueryable<Dashboardjobdetailcoreviewmodel> GetEmployeeJobswithJobtype(string Jobtype = "null", string Status = "null", string selectdate = "null", string searchkey = "null")

        {
            string sql = "";
            if (searchkey != null)
            {
                searchkey = searchkey.Replace("'", "''");
            }
            if (selectdate != "null")
            {
                //  selectdate = "'" + selectdate + "'";
                selectdate = "'" + Convert.ToDateTime(selectdate).ToString("MM/dd/yyyy") + "'";
            }
            else
            {
                DateTime myDateTime = DateTime.Now;
                selectdate = "'" + myDateTime.Date.ToString("MM/dd/yyyy") + "'";
            }

            if (Status == "3" || Status == "null")
            {
                Status = "!=13";
                sql = @"select * from
                      (select * from(select distinct TOP 100  jb.id,ISNull(s.Suburb,'') suburb,Res.NotWet WetRequiredType,Res.Height StoreysType,jb.OTRWRequired
                ,(SELECT COUNT(AssignTo) CountAssigned FROM  JobAssignToMapping 
				 WHERE IsDelete=0 ANd JobId = jb.Id) as OTRWAssignCount
                 ,s.SiteFileName as Address
                 ,s.SiteFileName+' '+ISNull(s.Suburb,'') AddressWithSuburb
                 ,s.StrataPlan
                 ,jb.JobNo, jb.JobId,jb.Siteid SiteId,ISNULL(jb.status, 0 ) status,js.JobStatus JobStatusName,ISNULL(s.Latitude, 0 ) Latitude,ISNULL(s.Longitude, 0 ) Longitude,
                        ISNULL(jb.JobType, 0 ) JobType,jt.JobType JobTypeName,ISNULL(jb.WorkType, 0 ) WorkType,wt.WorkTypeName,ISNULL(jb.PreferTime, 0 ) PreferTime,jb.BookedBy,jb.JobNotes,jb.OperationNotes,jb.EstimatedHrsPerUser
                        ,ISNull(s.Street,'') Street ,ISNull(s.streetName,'') StreetName ,ISNull(s.StreetType,0) StreetType ,ISNull(s.State,'') State, ISNull(s.PostalCode,0) PostalCode ,
                        cust.CustomerLastName CustomerName
                        ,users.UserName BookedByName
                        ,cast(jb.DateBooked as date) DateBooked
                        ,cust.StrataNumber
                        from jobs jb 
                        left join WorkType wt on jb.WorkType=wt.value
                        left join JobStatus js on jb.Status=js.value
                        left join JobType jt on jb.JobType=jt.value
                        left join AspNetUsers users on users.id=jb.BookedBy
                        left join CustomerSiteDetail s on s.SiteDetailId=jb.Siteid
                        left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId 
                        left join CustomerResidenceDetail Res on Res.SiteDetailId = s.SiteDetailId
                        where  jb.JobType=Isnull(" + Jobtype + @",jb.JobType)
                                     AND (jb.Status" + Status + @")
                                      AND(jb.IsDelete=0)
                                      AND jb.DateBooked=Isnull(" + selectdate + @",jb.DateBooked))t
                        where t.Address like '%" + searchkey + @"%' OR t.AddressWithSuburb like '%" + searchkey + "%' OR t.JobNo like '%" + searchkey + @"%' OR t.CustomerName like '%" + searchkey + @"%'
                            OR t.status like '%" + searchkey + @"%' OR t.JobType like '%" + searchkey + @"%' OR t.WorkTypeName like '%" + searchkey + @"%'
                           OR t.JobTypeName like '%" + searchkey + @"%' OR t.JobStatusName like '%" + searchkey + @"%')t
                           where ((t.OTRWRequired <>  t.OTRWAssignCount) or (t.OTRWRequired is null and t.OTRWAssignCount=0))";
            }
            else if (Status == "13")
            {
                Status = "=13";
                selectdate = "null";

                sql = @"select * from
                      (select * from(select distinct TOP 100  jb.id,ISNull(s.Suburb,'') suburb,Res.NotWet WetRequiredType,Res.Height StoreysType,jb.OTRWRequired
                       ,(SELECT COUNT(AssignTo) CountAssigned FROM  JobAssignToMapping 
				 WHERE IsDelete=0 ANd JobId = jb.Id) as OTRWAssignCount
                 ,s.SiteFileName as Address
                 ,s.SiteFileName+' '+ISNull(s.Suburb,'') AddressWithSuburb
                 ,s.StrataPlan
                 ,jb.JobNo, jb.JobId,jb.Siteid SiteId,ISNULL(jb.status, 0 ) status,js.JobStatus JobStatusName,ISNULL(s.Latitude, 0 ) Latitude,ISNULL(s.Longitude, 0 ) Longitude,
                        ISNULL(jb.JobType, 0 ) JobType,jt.JobType JobTypeName,ISNULL(jb.WorkType, 0 ) WorkType,wt.WorkTypeName,ISNULL(jb.PreferTime, 0 ) PreferTime,jb.BookedBy,jb.JobNotes,jb.OperationNotes,jb.EstimatedHrsPerUser
                        ,ISNull(s.Street,'') Street ,ISNull(s.streetName,'') StreetName ,ISNull(s.StreetType,0) StreetType ,ISNull(s.State,'') State, ISNull(s.PostalCode,0) PostalCode ,
                        cust.CustomerLastName CustomerName
                        ,users.UserName BookedByName
                        ,cast(jb.DateBooked as date) DateBooked
                        ,cust.StrataNumber
                        from jobs jb 
                        left join WorkType wt on jb.WorkType=wt.value
                        left join JobStatus js on jb.Status=js.value
                        left join JobType jt on jb.JobType=jt.value
                        left join AspNetUsers users on users.id=jb.BookedBy
                        left join CustomerSiteDetail s on s.SiteDetailId=jb.Siteid
                        left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId 
                        left join CustomerResidenceDetail Res on Res.SiteDetailId = s.SiteDetailId
                        where  jb.JobType=Isnull(" + Jobtype + @",jb.JobType)
                                      AND (jb.JobCategory='Stand By')
                                      AND(jb.IsDelete=0))t
                        where t.Address like '%" + searchkey + @"%' OR t.AddressWithSuburb like '%" + searchkey + "%' OR t.JobNo like '%" + searchkey + @"%' OR t.CustomerName like '%" + searchkey + @"%'
                            OR t.status like '%" + searchkey + @"%' OR t.JobType like '%" + searchkey + @"%' OR t.WorkTypeName like '%" + searchkey + @"%'
                           OR t.JobTypeName like '%" + searchkey + @"%' OR t.JobStatusName like '%" + searchkey + @"%')t
                           where ((t.OTRWRequired <>  t.OTRWAssignCount) or (t.OTRWRequired is null and t.OTRWAssignCount=0))";
            }



            var CustomerSiteList = Context.Database.SqlQuery<Dashboardjobdetailcoreviewmodel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<GetHours> GetAvailableBookedHours()
        {
            string sql = @"DECLARE @StartDate DATETIME
                           SET @StartDate = GETDATE()
                           
                           DECLARE @EndDate DATETIME 
                           SET @EndDate = GETDATE()+ 30
                           
                           DECLARE @TableOfDates TABLE(DateValue DATETIME)
                           
                           DECLARE @CurrentDate DATETIME
                           
                           SET @CurrentDate = @startDate
                           
                           WHILE @CurrentDate <= @endDate
                           BEGIN
                               INSERT INTO @TableOfDates(DateValue) VALUES (@CurrentDate)
                           
                               SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
                           END

                           SELECT 
                            DateValue
                           ,CASE WHEN DayOfWeek = 'Mondayhrs'
                                 THEN (SELECT 
                                   COUNT(Mondayhrs)
                                   FROM  employeedetail 
                                   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                   (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                 WHEN DayOfWeek = 'Tuesdayhrs'
                                 THEN (SELECT 
                                   COUNT(Tuesdayhrs)
                                   FROM  employeedetail 
                                   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                   (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                 WHEN DayOfWeek = 'Wednesdayhrs'
                                 THEN (SELECT 
                                   COUNT(Wednesdayhrs)
                                   FROM  employeedetail 
                                   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                     (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                   WHEN DayOfWeek = 'Thursdayhrs'
                                   THEN (SELECT 
                                     COUNT(Thursdayhrs)
                                     FROM  employeedetail 
                                     INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                     WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                     (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                   WHEN DayOfWeek = 'Fridayhrs'
                                   THEN (SELECT 
                                     COUNT(Fridayhrs)
                                     FROM  employeedetail 
                                     INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                     WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                     (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                   WHEN DayOfWeek = 'Saturdayhrs'
                                   THEN (SELECT 
                                     COUNT(Saturdayhrs)
                                     FROM  employeedetail 
                                     INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                     WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                     (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                   WHEN DayOfWeek = 'Sundayhrs'
                                   THEN (SELECT 
                                   COUNT(Sundayhrs)
                                   FROM  employeedetail 
                                   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
                                   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4') - 
                                   (SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue)
                                          ELSE null
                                      END AS HoursAvailable
                                    ,(SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue) HoursBooked
                                    
                                    FROM (
                                    SELECT DateValue ,DATENAME(dw,DateValue) + 'hrs' As DayOfWeek 
                                     FROM @TableOfDates)t";

            var userList = Context.Database.SqlQuery<GetHours>(sql).AsQueryable();
            return userList;
        }
        public IQueryable<CheckSiteTwoPeopleJobCoreViewModel> CheckTwoPeopleJob(string siteId)
        {
            string sql = @"select cr.NeedTwoPPL from CustomerSiteDetail cs
                  inner Join CustomerResidenceDetail cr on cs.SiteDetailId=cr.SiteDetailId
                  where cs.SiteDetailId='" + siteId + "' and cr.NeedTwoPPL='1'";

            var siteDetail = Context.Database.SqlQuery<CheckSiteTwoPeopleJobCoreViewModel>(sql).AsQueryable();
            return siteDetail;
        }
        public IQueryable<OtrwJobNoForSheetCoreViewMode> GetOtrwAssignJobNo(Guid UserId)
        {
            string sql = @"select J.Id JobId,j.JobNo 
                       from Jobs j
                       inner Join jobAssignToMapping JAM  on JAM.JobId=j.id
                       where JAM.AssignTo='" + UserId + "'";

            var jobDetail = Context.Database.SqlQuery<OtrwJobNoForSheetCoreViewMode>(sql).AsQueryable();
            return jobDetail;
        }
        public IQueryable<int> OTRWHasJob(Guid AssignTo, DateTime AssignedDate, DateTime AssignedEndDate, Guid JobId = default(Guid))
        {
            string sql = @"select j.JobNo
                         from Jobs j join JobAssignToMapping jm on j.Id=jm.JobId 
                         where jm.AssignTo='" + AssignTo + "' and j.Id not in(select JobId from UserTimeSheet where IsDelete = 0) and j.IsDelete = 0 and jm.IsDelete=0 and j.Id <> '" + JobId + "' and ('" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' >= jm.StartTime and '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' <= jm.EndTime or  '"
                           + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "' >= jm.StartTime and  '"
                           + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "' <= jm.EndTime or  jm.StartTime >= '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' and jm.EndTime <= '"
                           + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "') and '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "'!= jm.EndTime and  '"
                           + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "'!= jm.StartTime";

            //if (JobId != null && JobId != Guid.Empty)
            //{
            //    sql = sql + " and j.Id='" + JobId + "'";
            //}

            var hasJob = Context.Database.SqlQuery<int>(sql).AsQueryable();
            return hasJob;
        }

        public IQueryable<int> OTRWHasSameJob(Guid AssignTo, DateTime AssignedDate, Guid JobId = default(Guid))
        {
            string sql = @"select j.JobNo
                         from Jobs j join JobAssignToMapping jm on j.Id=jm.JobId 
                         where jm.AssignTo='" + AssignTo + "' and j.IsDelete = 0 and jm.IsDelete=0 and j.Id = '" + JobId + "'and ('" + AssignedDate.ToString("MM/dd/yyyy") + "' = jm.DateBooked)";

            var hasJob = Context.Database.SqlQuery<int>(sql).AsQueryable();
            return hasJob;
        }
        public IQueryable<AspNetUsersCore> OTRWHasJobWithName(Guid AssignTo, DateTime AssignedDate, DateTime AssignedEndDate, Guid JobId = default(Guid))
        {
            string sql = @"select j.JobNo,Emp.UserName  from Jobs j join JobAssignToMapping jm on j.Id=jm.JobId 
                           inner Join EmployeeDetail Emp on jm.AssignTo=Emp.EmployeeId
                          where jm.AssignTo='" + AssignTo + "' and j.JobType!=4 and j.IsDelete = 0 and jm.IsDelete=0 and j.Id <> '" + JobId + "' and ('" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' >= jm.StartTime and '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' <= jm.EndTime or  '"
                         + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "' >= jm.StartTime and  '"
                         + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "' <= jm.EndTime or  jm.StartTime >= '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "' and jm.EndTime <= '"
                         + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "') and '" + AssignedDate.ToString("MM/dd/yyyy HH:mm:ss") + "'!= jm.EndTime and  '"
                         + AssignedEndDate.ToString("MM/dd/yyyy HH:mm:ss") + "'!= jm.StartTime";

            //if (JobId != null && JobId != Guid.Empty)
            //{
            //    sql = sql + " and j.Id='" + JobId + "'";
            //}

            var hasJob = Context.Database.SqlQuery<AspNetUsersCore>(sql).AsQueryable();
            return hasJob;
        }

        public bool IsWorkTypeMatch(Guid UserId, Guid JobId)
        {
            var UserWorkType = Context.Jobs.Where(m => m.Id == JobId).Select(m => m.WorkType).FirstOrDefault();
            var JobWorkType = Context.EmployeeWorkType.Where(m => m.EmployeeId == UserId).Select(m => m.WorkType).ToList();

            foreach (var type in JobWorkType)
            {
                if (UserWorkType == type)
                {
                    return true;
                }
            }
            return false;
        }
        public IQueryable<GetQuickViewJobDataCoreViewModel> GetQuickViewData(Guid JobId)
        {

            string sql = @"select CAST(CAST(DATEADD(MS, SUM(DATEDIFF(MS, '00:00',UTS.TimeSpent)), '00:00') as time) as nvarchar) TimeTaken,j.Id as JobId, j.JobType,j.JobNo,j.JobNotes,j.OperationNotes, CG.CustomerNotes,IV.Price InvoicePrice,Iv.Paid,Iv.ApprovedBy from Jobs j
                          left Join CustomerGeneralInfo CG on j.CustomerGeneralInfoId=CG.CustomerGeneralInfoId
                          left join Invoice IV on j.JobNo=IV.JobId
                          left join UserTimeSheet UTS on j.id=UTS.JobId
                          where j.Id='" + JobId + "'GROUP BY j.Id, j.JobType,j.JobNo,j.JobNotes,j.OperationNotes, CG.CustomerNotes,IV.Price,Iv.Paid,Iv.ApprovedBy";

            var CustomerSiteList = Context.Database.SqlQuery<GetQuickViewJobDataCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<EmployeeJobVieweModel> UpdateRescheduleJobDate(string CurrentDate, string UpdateRescheduleDate)
        {
            string sql = @"
                           update JobAssignToMapping set DateBooked='" + UpdateRescheduleDate + "', StartTime='" + UpdateRescheduleDate + "', EndTime='" + UpdateRescheduleDate + "' where DateBooked ='" + CurrentDate + @"'

                               select DateBooked,Id from JobAssignToMapping Where DateBooked='" + UpdateRescheduleDate + "'";

            string sqljobsupdate = @"
                           update jobs set DateBooked='" + UpdateRescheduleDate + "' where DateBooked='" + CurrentDate + @"'";

            // var hasJob = Context.Database.SqlQuery<int>(sql).AsQueryable();
            var jobsupdate = Context.Database.ExecuteSqlCommand(sqljobsupdate);
            var jobDetail = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return jobDetail;
        }

        public IQueryable<CustomerContactsCoreViewModel> GetJobSiteContactsEmail(Guid JobId)
        {
            string sql = @"select CC.EmailId  from Jobs
                           inner join CustomerSiteDetail CSD on Jobs.SiteId=CSD.SiteDetailId
                           inner join CustomerContacts CC on CSD.SiteDetailId=CC.SiteId
                           where Jobs.Id='" + JobId + "'";

            var ContactsEmail = Context.Database.SqlQuery<CustomerContactsCoreViewModel>(sql).AsQueryable();
            return ContactsEmail;
        }
        public IQueryable<JobPerFormanceCoreViewModel> GetJobPerFormanceData(string JobId)
        {
            string sql = @"SELECT DISTINCT TOP 1
                                 ISNULL(JSPOCost,0) + ISNULL(StockItemCost,0) TotalCost
                                 ,Inv.Price- (ISNULL(JSPOCost,0) + ISNULL(StockItemCost,0)) LabourIncome
                                 ,(Inv.Price- (ISNULL(JSPOCost,0) + ISNULL(StockItemCost,0)))/(RevHours + NRLHours) LIPHR
                                 ,(Inv.Price- (ISNULL(JSPOCost,0) + ISNULL(StockItemCost,0)))-(employeeDetail.ActualRate * (RevHours + NRLHours)) LabourProfit
                                 ,((Inv.Price- (ISNULL(JSPOCost,0) + ISNULL(StockItemCost,0)))-(employeeDetail.ActualRate * (RevHours + NRLHours)))/(RevHours + NRLHours) LPPHR
                                 ,0.00 SalesBonus
                                 ,Inv.Price SaleIncome
                                 ,RevHours
                                 ,JSPOCost
                                 ,employeeDetail.ActualRate * (RevHours + NRLHours) LabourCost
                                 ,employeeDetail.ActualRate LCPHR
                                 ,NRLHours
                                 ,StockItemCost
                                 ,RevHours + NRLHours [Hours]
                                 
                                 
                                  from
                                 (
                                 SELECT JB.Id JobId
                                 ,(SELECT SUM(Cost) FROM PurchaseOrderByJob WHERE JobID = JB.Id  AND IsDelete = 0) JSPOCost
                                 
                                 ,(SELECT SUM(Stock.Price) FROM Stock inner join JobStock on JobStock.StockId = Stock.ID
                                 WHERE JobID = JB.Id  AND Stock.IsDelete = 0) StockItemCost
                                 
                                 ,(SELECT SUM(DATEDIFF(second, StartTime, EndTime)/ 3600.0) [hours]  FROM(
                                 SELECT MIN(UserTimeSheet.StartTime) StartTime, MAX(UserTimeSheet.EndTime) EndTime
                                 ,UserTimeSheet.JobDate
                                 ,UserTimeSheet.Reason
                                  FROM Jobs
                                 INNER JOIN UserTimeSheet ON Jobs.Id =  UserTimeSheet.JobId
                                 WHERE Jobs.Id = JB.Id 
                                 group by UserTimeSheet.JobDate,UserTimeSheet.Reason
                                  having UserTimeSheet.Reason = 'Job' 
                                 )t where t.StartTime != '00:00:00.0000000' AND t.EndTime!= '00:00:00.0000000'
                                 Group by t.Reason) RevHours
                                 ,
                                 (SELECT  SUM(DATEDIFF(second, StartTime, EndTime)/ 3600.0) [hours]  FROM(
                                 SELECT MIN(UserTimeSheet.StartTime) StartTime, MAX(UserTimeSheet.EndTime) EndTime
                                 ,UserTimeSheet.JobDate
                                 ,UserTimeSheet.Reason
                                  FROM Jobs
                                 INNER JOIN UserTimeSheet ON Jobs.Id =  UserTimeSheet.JobId
                                 WHERE Jobs.Id = JB.Id 
                                 group by UserTimeSheet.JobDate,UserTimeSheet.Reason
                                  having UserTimeSheet.Reason = 'Travelling'
                                 )t where t.StartTime != '00:00:00.0000000' AND t.EndTime!= '00:00:00.0000000'
                                 Group by t.Reason)NRLHours
                                 FROM Jobs JB where JB.Id = '" + JobId + @"'
                                 )tt 
                                 left join(SELECT TOP 1 Price,EmployeeJobId FROM invoice WHERE IsDelete = 0 AND EmployeeJobId='" + JobId + @"'
                                  order by CreatedDate desc) INV
                                 ON tt.JobId = Inv.EmployeeJobId
                                 left join JobAssignToMapping on JobAssignToMapping.JobId = tt.JobId
                                 left join employeeDetail on employeeDetail.EmployeeId = JobAssignToMapping.AssignTo
                                 where tt.JobId = '" + JobId + @"'";

            var CustomerSiteList = Context.Database.SqlQuery<JobPerFormanceCoreViewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public bool OTRWHasJobBooked(Guid? AssignTo, DateTime? DateBooked)
        {
            string sql = @"select Id from JobAssignToMapping
                           where AssignTo='" + AssignTo + "' AND IsDelete=0 AND DateBooked='" + DateBooked.Value.ToString("MM/dd/yyyy") + "'";

            var hasJob = Context.Database.SqlQuery<Guid>(sql).AsQueryable();
            if (hasJob.Any())
            {
                return true;
            }

            return false;
        }

        public bool EmployeeHasJob(string AssignTo, DateTime? StartDate, DateTime? EndDate)
        {
            bool employeeHasJob = false;
            string sql = @"select id from JobAssignToMapping where AssignTo='" + AssignTo +
                          "' and (DateBooked between '" + StartDate.Value.ToString("MM/dd/yy") + "' and '" +
                          EndDate.Value.ToString("MM/dd/yy") + "')";

            var employeeJob = Context.Database.SqlQuery<Guid>(sql).AsQueryable();
            if (employeeJob.Any())
            {
                employeeHasJob = true;
            }
            return employeeHasJob;
        }
        public IQueryable<GetWorkTypeCoreViewModel> GetOTRWUserUsingByWorkType(int? Worktype)
        {
            string sql = @"select EWT.EmployeeId OTRWID,Ed.UserName OTRWUserName from EmployeeWorkType EWT
                         inner join EmployeeDetail Ed on EWT.EmployeeId=Ed.EmployeeId
                         where  Ed.isdelete = 0 and EWT.WorkType='" + Worktype + "' ";

            var OtrwDetail = Context.Database.SqlQuery<GetWorkTypeCoreViewModel>(sql).AsQueryable();
            return OtrwDetail;
        }

        public IQueryable<EmployeeJobVieweModel> GetEmployeeJobsWithKeyword(string keyWord, int? jobType, bool? contracted, Guid customerId)
        {
            string sql = @"Select DISTINCT TOP(1000) *
                ,(SELECT count(*) from jobs where IsDelete = 0) TotalCount from(
                SELECT  jb.id
               ,jb.WorkType
               ,jb.JobNo
               ,jb.JobId
               ,jb.CreatedDate
               ,jb.Status
               ,jb.CustomerGeneralInfoId
               ,jb.SiteId
               ,jb.JobType
               ,custdetail.Contracted,custdetail.StreetName
                ,(custdetail.SiteFileName+' '+custdetail.Suburb) as SiteFileName
               ,(SELECT UserName = STUFF((
                            SELECT distinct ', ' + ANU.UserName +' '+cast(jap.DateBooked as nvarchar(50))
                            from JobAssignToMapping jap
                INNER JOIN  AspNetUsers ANU on jap.AssignTo=ANU.Id 
                where jap.IsDelete=0 AND  jap.JobId = jb.Id
                            FOR XML PATH('')
                            ), 1, 1, '')) AssignUser
                ,(cc.FirstName +' '+cc.LastName) as ContactName
                ,cust.CustomerLastName CustomerName
                ,cust.StrataPlan
                ,cust.StrataNumber
                ,users.UserName BookedByName
                ,jb.datebooked
                 ,(custdetail.SiteFileName+' '+custdetail.Suburb) as SiteAddress
                from Jobs jb 
                Left join AspNetUsers users on users.id=jb.BookedBy
                Left join CustomerGeneralInfo cust on cust.CustomerGeneralInfoId=jb.CustomerGeneralInfoId
                Left join CustomerSiteDetail custdetail on jb.SiteId=custdetail.SiteDetailId
                left join CustomerContacts cc on custdetail.ContactId=cc.ContactId
                left join CustomerResidenceDetail CRD on CRD.SiteDetailId = custdetail.SiteDetailId
                WHERE (jb.IsDelete=0))t";


            if (!string.IsNullOrEmpty(keyWord) || jobType != null || contracted == true || customerId != Guid.Empty)
            {
                sql = sql + " WHERE 1=1";
            }


            if (!string.IsNullOrEmpty(keyWord))
            {
                sql = sql + " AND (t.JobNo LIKE '%" + keyWord + "%' OR t.SiteFileName LIKE '%" + keyWord + "%' OR t.CustomerName LIKE '%" + keyWord + "%')";
            }

            if (jobType != null && jobType > 0)
            {
                sql = sql + " AND t.JobType='" + jobType + "'";
            }

            if (contracted == true)
            {
                sql = sql + " AND t.Contracted=1";
            }

            if (customerId != Guid.Empty)
            {
                sql = sql + " AND t.CustomerGeneralInfoId='" + customerId + "'";

            }
            sql = sql + " ORDER BY t.CreatedDate DESC";

            var CustomerSiteList = Context.Database.SqlQuery<EmployeeJobVieweModel>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public void SavejobStatus(Jobs empjob)
        {
            try
            {
                if (empjob != null)
                {
                    FsmContext db = new FsmContext();
                    var job = db.Jobs.Where(i => i.Id == empjob.Id).FirstOrDefault();
                    job.IsDelete = true;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
