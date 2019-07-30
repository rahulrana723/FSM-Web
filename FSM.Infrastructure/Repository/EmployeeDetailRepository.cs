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
    public class EmployeeDetailRepository : GenericRepository<FsmContext, EmployeeDetail>, IEmployeeDetailRepository
    {
        public object GetEmployeeDetailById()
        {
            string sql = @"
               SELECT * from EmployeeDetail where Employeeid = '7C21588C-3E4C-4AEC-99BC-65EDB3CBB2E2'";

            return Context.Database.SqlQuery<dynamic>(sql).First();
        }
        public IQueryable<EmployeeDetailViewModelCore> getUserRole(string RoleId)
        {
            string sql = @"select * from AspNetRoles where id='" + RoleId + "'";

            var CustomerSiteList = Context.Database.SqlQuery<EmployeeDetailViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public List<string> GetNoRateEmployeesByEID()
        {
            string sql = @"
               SELECT EID from EmployeeDetail where Eid not in (select Eid from EmployeeRates)";
            return Context.Database.SqlQuery<string>(sql).ToList();
        }

        public IQueryable<EmployeeDetailViewModelCore> GetEmployeeDetail(string searchKeywords = "")
        {
            IQueryable<EmployeeDetailViewModelCore> innerJoinQuery =
              from Employee in Context.EmployeeDetail
              join Roles in Context.AspNetRoles on Employee.Role equals Roles.Id
              where Employee.FirstName.Contains(searchKeywords)
                    || Employee.LastName.Contains(searchKeywords)
                    || (Employee.FirstName + " " + Employee.LastName).Contains(searchKeywords)
                    || Roles.Name.Contains(searchKeywords)
                    || Employee.UserName.Contains(searchKeywords)
                    || Employee.Mobile.Contains(searchKeywords)
                    || Employee.Email.Contains(searchKeywords)
                    || Employee.EID.Contains(searchKeywords)
              select new EmployeeDetailViewModelCore
              {
                  Name = Employee.FirstName + " " + Employee.LastName
              ,
                  TFN = Employee.TFN
              ,
                  UserName = Employee.UserName
              ,
                  Role = Roles.Name
              ,
                  EID = Employee.EID
              ,
                  EmployeeId = Employee.EmployeeId
              ,
                  EmailAddress = Employee.Email
              ,
                  Mobile = Employee.Mobile
              ,
                  CreatedDate = Employee.CreatedDate
              ,
                  IsDelete = Employee.IsDelete
              ,
                  IsActive = Employee.IsActive
              };

            return innerJoinQuery;
        }


        public string GetMaxEmployeeId()
        {

            string sql = @"
               select max(CAST(EID AS INT)) from EmployeeDetail";
            return Convert.ToString(Context.Database.SqlQuery<int?>(sql).FirstOrDefault());
        }

        public IQueryable<EmployeeDetailViewModelCore> GetEmployeeDetailWithUnreadMessage(string LoggedId = "", string searchKeywords = "")
        {
            if (searchKeywords != null)
            {
                searchKeywords = searchKeywords.Replace("'", "''");
            }
            string sql = @"SELECT DISTINCT * FROM (SELECT (SELECT COUNT(Id) FROM UserMessage 
       WHERE IsMessageRead = 0
       AND From_Id  = EmployeeDetail.EmployeeId
       AND To_Id = '" + LoggedId + @"'
       AND UserMessage.MessageThreadID = UserMessageThread.ID  ) UnReadMsg
      , EmployeeId,CONVERT(varchar(50), EID)EID,EmployeeDetail.UserName, r.Name Role,ISNULL(FirstName,'')+' '+ISNULL(LastName,'') Name 
      ,UserMessageThread.ModifiedDate
      ,CONVERT(VARCHAR(20),UserMessageThread.ModifiedDate,100) LastDateSent
      , (SELECT TOP 1 case when LEN([Message])>163 
	  THEN SUBSTRING([Message],1,163) + '...'
	  ELSE [Message]
	  END FROM UserMessage WHERE MessageThreadID = UserMessageThread.ID order by CreatedDate desc) [Message]
      FROM EmployeeDetail
      INNER JOIN AspNetRoles r on r.Id=EmployeeDetail.Role
      INNER JOIN UserMessageThread ON UserMessageThread.ToMessageUser = EmployeeDetail.EmployeeId  AND UserMessageThread.LoggedInUser = '" + LoggedId + @"'
      INNER JOIN UserMessage UM ON UM.MessageThreadID = UserMessageThread.ID
      where EID Like '%" + searchKeywords + "%' or [Message] Like '%" + searchKeywords + "%' or EmployeeDetail.UserName like '%" + searchKeywords + "%'or FirstName like '%" + searchKeywords + "%' or LastName like '%" + searchKeywords + "%' or RTRIM(LTRIM(FirstName)) +' '+ RTRIM(LTRIM(LastName)) like '%" + searchKeywords + "%')t WHERE t.[Message] is not null ORDER BY t.ModifiedDate desc";
            var CustomerSiteList = Context.Database.SqlQuery<EmployeeDetailViewModelCore>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public int GetTotalMessageCount(string LoggedId = "")
        {
            string sql = @"SELECT COUNT(Id) FROM UserMessage
                          WHERE IsMessageRead = 0
                          AND MessageThreadID IN(SELECT UserMessageThread.ID
                          FROM UserMessageThread 
                          WHERE LoggedInUser =  '" + LoggedId + @"')";
            int returnValue = Context.Database.SqlQuery<int>(sql).FirstOrDefault();
            return returnValue;
        }

        public IQueryable<GetLatitudeLongitudeCoreviewModel> GetLatitudeLongitude(string UserIds)
        {
            string sql = @"DECLARE @sql nvarchar(max)

                         SET @sql='select 
                           ED.EmployeeId,ED.UserName,ED.EID,ED.Latitude,ED.Longitude
                           from dbo.EmployeeDetail ED
                           where 1=1'

                          IF(isnull('" + UserIds + @"','')<>'')
                            BEGIN
                                SET @sql+=' AND ED.EmployeeId in (" + UserIds + @")'
                            END
                           
                        EXEC(@sql)";

            var CustomerSiteList = Context.Database.SqlQuery<GetLatitudeLongitudeCoreviewModel>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<EmployeeDetailViewModelDashboard> GetEmployeeDetailDashboard()
        {
            IQueryable<EmployeeDetailViewModelDashboard> innerJoinQuery =
              from Employee in Context.EmployeeDetail
              join Roles in Context.AspNetRoles on Employee.Role equals Roles.Id
              where Employee.IsDelete==false && Employee.IsActive == true && Roles.Id.ToString() == "31cf918d-b8fe-4490-b2d7-27324bfe89b4" //otrw user
              select new EmployeeDetailViewModelDashboard
              {
                  Name = Employee.FirstName + " " + Employee.LastName
              ,
                  TFN = Employee.TFN
              ,
                  UserName = Employee.UserName
              ,
                  Role = Roles.Name
              ,
                  EID = Employee.EID
              ,
                  EmployeeId = Employee.EmployeeId
              ,
                  EmailAddress = Employee.Email
              ,
                  Mobile = Employee.Mobile
              ,
                  CreatedDate = Employee.CreatedDate
              ,
                  IsDelete = Employee.IsDelete
              ,
                  IsActive = Employee.IsActive
              ,
                  OTRWOrder = Employee.OTRW_Order
              };

            return innerJoinQuery;
        }
        public IQueryable<EmployeeDetailViewModelCore> GetUser()
        {
            string sql = @"SELECT EmployeeId,UserName from EmployeeDetail
                           WHERE ISDelete=0 AND IsActive=1 AND (EmployeeId !='D5549F68-593A-4F34-99CA-24DF2C9DBC16' AND EmployeeId !='4E439EC2-016D-473A-81F8-893A5C9FAF7D' AND(EmployeeId !='3A676860-7548-4C7A-9F60-FE70E24DFA11')) order by UserName asc";

            var userList = Context.Database.SqlQuery<EmployeeDetailViewModelCore>(sql).AsQueryable();
            return userList;
        }
        public string UpdateOtrwOrder(int? otrwOrder)
        {
            int? updateOrder = otrwOrder - 1;
            string sql = @"Update EmployeeDetail Set OTRW_Order=OTRW_Order-1 where OTRW_Order >'"+otrwOrder+@"'
                             SELECT '0'";
            return Convert.ToString(Context.Database.SqlQuery<string>(sql).ToList());
        }

        public string DeleteAllReleatedDataByEmployeeid(string emloyeeId,string userID)
        {
            Guid Employeeid = Guid.Parse(emloyeeId);
            Guid UserID = Guid.Parse(userID);
            StringBuilder sql = new StringBuilder();

            sql.Append(@" UPDATE  JobAssignToMapping set IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where AssignTo ='" + Employeeid + @"'  ");
            sql.Append(@" UPDATE  Jobs set IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where ID in (SELECT JobId from JobAssignToMapping Where AssignTo ='" + Employeeid + @"' )");
            sql.Append(@" UPDATE  PurchaseOrderByJob set IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where JobID in (SELECT JobId from JobAssignToMapping Where AssignTo ='" + Employeeid + @"' )");
            sql.Append(@" UPDATE  PurchaseorderItemJob set IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where PurchaseOrderID in (SELECT ID from PurchaseOrderByJob Where JobID IN(SELECT JobId from JobAssignToMapping Where AssignTo = '" + Employeeid + @"'))");
            sql.Append(@" UPDATE  Invoice SET IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where EmployeeJobId IN(SELECT JobId from JobAssignToMapping Where AssignTo ='" + Employeeid + @"')");
            sql.Append(@" UPDATE  JobStock SET IsDelete = 1,ModifiedBy='" + UserID + @"',ModifiedDate=getDate() where JobId IN(SELECT JobId from JobAssignToMapping Where AssignTo = '" + Employeeid + @"')");
            sql.Append(@" UPDATE  CustomerGeneralInfo	set IsDelete=1 , ModifiedDate=getDate(),ModifiedBy='" + UserID + @"' Where  CreatedBy ='" + Employeeid + @"'");
            sql.Append(@" UPDATE  CustomerSiteDetail		set IsDelete=1 , ModifiedDate=getDate(),ModifiedBy='" + UserID + @"'  where CustomerGeneralInfoId in ( select CustomerGeneralInfoId from CustomerGeneralInfo Where  CreatedBy ='" + Employeeid + @"') ");
            sql.Append(@" UPDATE  CustomerConditionreport set IsDelete=1 , ModifiedDate=getDate(),ModifiedBy='" + UserID + @"' where SiteDetailId in (select SiteDetailId from CustomerSiteDetail  where CustomerGeneralInfoId in ( select CustomerGeneralInfoId from CustomerGeneralInfo Where  CreatedBy ='" + Employeeid + @"')) ");
            sql.Append(@" UPDATE  CustomerContacts	set IsDelete=1 , ModifiedDate=getDate(),ModifiedBy='" + UserID + @"' Where  CustomerGeneralInfoId in ( select CustomerGeneralInfoId from CustomerGeneralInfo Where  CreatedBy ='" + Employeeid + @"') ");
            sql.Append(@" UPDATE  CustomerContactLog	 set IsDelete=1 , ModifiedDate=getDate(),ModifiedBy='" + UserID + @"' Where  CustomerGeneralInfoId in ( select CustomerGeneralInfoId from CustomerGeneralInfo Where  CreatedBy ='" + Employeeid + @"') SELECT '0'");
            return Convert.ToString(Context.Database.SqlQuery<string>(sql.ToString()).ToList());
        }

    }
}
