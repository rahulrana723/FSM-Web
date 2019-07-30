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
    public class CustomerGeneralInfoRepository : GenericRepository<FsmContext, CustomerGeneralInfo>, ICustomerGeneralInfoRepository
    {
        public String GetMaxCID()
        {
            int count = Context.Jobs.Count();
            string sql = "";
            if (count > 0)
            {
                sql = "SELECT IDENT_CURRENT ('CustomerGeneralInfo')+1 AS Current_Identity";
            }
            else
            {
                sql = "SELECT IDENT_CURRENT ('CustomerGeneralInfo') AS Current_Identity";
            }
            return Convert.ToString(Context.Database.SqlQuery<decimal>(sql).FirstOrDefault());
        }

        public int GetMaxCTId()
        {
            string sql = "SELECT MAX(CTId) FROM dbo.CustomerGeneralInfo";
            var Ctid = Context.Database.SqlQuery<int>(sql).FirstOrDefault() + 1;
            return Ctid;
        }
        public IQueryable<CustomerGeneralInfoCoreViewModel> GetCustomerListBySearchKeyword(string keyword)
        {
            if (keyword != null)
            {
                keyword = keyword.Replace("'", "''");
            }
            string sql;
            if (keyword == null) { 
            sql = @"SELECT DISTINCT Top 1000 CGI.CustomerGeneralInfoId,CSD.BlackListed
            ,CGI.CTID,CGI.CustomerLastName,CGI.CustomerType
            ,(SELECT COUNT(SiteDetailId) FROM CustomerSiteDetail WHERE CustomerGeneralInfoId = CGI.CustomerGeneralInfoId) CustomerSiteCount
            ,CGI.Note,CGI.* FROM 
            CustomerGeneralInfo  CGI 
            LEFT JOIN CustomerContacts on CGI.CustomerGeneralInfoId = CustomerContacts.CustomerGeneralInfoId 
            LEFT JOIN CustomerSiteDetail CSD on CGI.CustomerGeneralInfoId=CSD.CustomerGeneralInfoId
            LEFT JOIN Jobs ON Jobs.CustomerGeneralInfoId = CGI.CustomerGeneralInfoId";
            }
            else
            {
              sql = @"SELECT DISTINCT Top 1000 CGI.CustomerGeneralInfoId
            ,CSD.BlackListed
            ,CSD.SiteFileName 
            ,CSD.SiteDetailId
            ,(SELECT COUNT(SiteDetailId) FROM CustomerSiteDetail WHERE CustomerGeneralInfoId = CGI.CustomerGeneralInfoId) CustomerSiteCount
            ,CGI.CTID,CGI.CustomerLastName,CGI.CustomerType
            ,CGI.Note,CGI.* FROM 
            CustomerGeneralInfo  CGI 
            LEFT JOIN CustomerContacts on CGI.CustomerGeneralInfoId = CustomerContacts.CustomerGeneralInfoId 
            LEFT JOIN CustomerSiteDetail CSD on CGI.CustomerGeneralInfoId=CSD.CustomerGeneralInfoId AND CSD.IsDelete = 0
            LEFT JOIN Jobs ON Jobs.CustomerGeneralInfoId = CGI.CustomerGeneralInfoId 
            WHERE (CGI.IsDelete=0 or CGI.IsDelete is null) 
            AND (CGI.CTId LIKE '%" + keyword + @"%' 
            OR CGI.CustomerLastName LIKE '%" + keyword + @"%'
            OR CGI.Note LIKE '%" + keyword + @"%'
            OR CustomerContacts.FirstName LIKE '%" + keyword + @"%'
            OR CustomerContacts.LastName LIKE '%" + keyword + @"%'
            OR CustomerContacts.PhoneNo1 LIKE '%" + keyword + @"%'
            OR RTRIM(LTRIM(CustomerContacts.FirstName)) +' '+RTRIM(LTRIM(CustomerContacts.LastName)) LIKE '%" + keyword + @"%'
            OR(CSD.Street +' '+ CSD.StreetName) like '%" + keyword + @"%'
			OR  (CSD.Street +' '+ CSD.StreetName +' '+CSD.Suburb) like '%" + keyword + @"%'
            OR(CSD.StreetName + ' ' + CSD.Suburb) like '%" + keyword + @"%'
            OR CSD.StreetName like '%" + keyword + @"%'
            OR CSD.Street like '%" + keyword + @"%'
            OR CSD.Suburb like '%" + keyword + @"%'
            OR CSD.SiteFileName like '%" + keyword + "%') order by CGI.CustomerLastName ";
            }
            var customerSiteList = Context.Database.SqlQuery<CustomerGeneralInfoCoreViewModel>(sql).AsQueryable();
            return customerSiteList;
        }

        public IQueryable<CustomerGeneralInfoCoreViewModel> GetCustomerNameWithCID(string Cid)
        {
            string sql = @"SELECT CustomerLastName,CID  from CustomerGeneralInfo order by CustomerLastName ";
            var CustomerList = Context.Database.SqlQuery<CustomerGeneralInfoCoreViewModel>(sql).AsQueryable();
            return CustomerList;
        }

        public IQueryable<WorkingHoursModel> GetAvailableWorkingHours(DateTime date, DateTime EndDate, String type)
        {
            try
            {
                string sql;

                if (type == "Month")
                {
                    string datestring = date.ToString("yyyy-MM-dd h:mm tt");
                    sql = @"DECLARE @StartDate DATETIME
                            SET @StartDate =  '" + datestring + @"'
                           
                           DECLARE @EndDate DATETIME 
                           SET @EndDate =(SELECT DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0, @StartDate)+1,0)))
                           
                           DECLARE @TableOfDates TABLE(DateValue DATETIME)
                           
                           DECLARE @CurrentDate DATETIME
                           
                           SET @CurrentDate = @startDate
                           
                           WHILE @CurrentDate <= @endDate
                           BEGIN
                               INSERT INTO @TableOfDates(DateValue) VALUES (@CurrentDate)
                           
                               SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
                           END

                            SELECT SUM(RoofHoursAvailable) RoofHoursAvailable
                            ,SUM(CleaningHoursAvailable) CleaningHoursAvailable
                            ,SUM(HoursBooked) HoursBooked FROM(
                           SELECT 
                            DateValue
                           ,CASE WHEN DayOfWeek = 'Mondayhrs'
                                 THEN (SELECT sum(Mondayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Mondayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Tuesdayhrs'
                                 THEN (SELECT sum(Tuesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Tuesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Wednesdayhrs'
                                 THEN (SELECT sum(Wednesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Wednesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                  
                                   WHEN DayOfWeek = 'Thursdayhrs'
                                   THEN (SELECT sum(Thursdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Thursdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                   WHEN DayOfWeek = 'Fridayhrs'
                                   THEN (SELECT sum(FridayHrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   FridayHrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                     
                                    ELSE null
                                    END AS RoofHoursAvailable
                                    
                                -------------------Cleaning--------------------------------------    
                                ,CASE WHEN DayOfWeek = 'Mondayhrs'
                                 THEN (SELECT sum(Mondayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Mondayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Tuesdayhrs'
                                 THEN (SELECT sum(Tuesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Tuesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Wednesdayhrs'
                                 THEN (SELECT sum(Wednesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Wednesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                  
                                   WHEN DayOfWeek = 'Thursdayhrs'
                                   THEN (SELECT sum(Thursdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Thursdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                   WHEN DayOfWeek = 'Fridayhrs'
                                   THEN (SELECT sum(FridayHrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   FridayHrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) 
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                     
                                    ELSE null
                                    END AS CleaningHoursAvailable 
                                 
                                    ,(SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue) HoursBooked
                                    
                                    FROM (
                                    SELECT DateValue ,DATENAME(dw,DateValue) + 'hrs' As DayOfWeek 
                                     FROM @TableOfDates)t
                                    )tt";
                }
                else
                {
                    string datestring = date.ToString("yyyy-MM-dd h:mm tt");
                    string endstring = EndDate.ToString("yyyy-MM-dd h:mm tt");
                    sql = @"DECLARE @StartDate DATETIME
                            SET @StartDate =  '" + datestring + @"'
                           
                           DECLARE @EndDate DATETIME 
                               SET @EndDate = '" + endstring + @"'
                           
                           DECLARE @TableOfDates TABLE(DateValue DATETIME)
                           
                           DECLARE @CurrentDate DATETIME
                           
                           SET @CurrentDate = @startDate
                           
                           WHILE @CurrentDate <= @endDate
                           BEGIN
                               INSERT INTO @TableOfDates(DateValue) VALUES (@CurrentDate)
                           
                               SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
                           END

                            SELECT SUM(RoofHoursAvailable) RoofHoursAvailable
                            ,SUM(CleaningHoursAvailable) CleaningHoursAvailable
                            ,SUM(HoursBooked) HoursBooked FROM(
                           SELECT 
                            DateValue
                           ,CASE WHEN DayOfWeek = 'Mondayhrs'
                                 THEN (SELECT sum(Mondayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Mondayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Tuesdayhrs'
                                 THEN (SELECT sum(Tuesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Tuesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Wednesdayhrs'
                                 THEN (SELECT sum(Wednesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Wednesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                  
                                   WHEN DayOfWeek = 'Thursdayhrs'
                                   THEN (SELECT sum(Thursdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Thursdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                   WHEN DayOfWeek = 'Fridayhrs'
                                   THEN (SELECT sum(FridayHrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   FridayHrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 1)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                     
                                    ELSE null
                                    END AS RoofHoursAvailable
                                    
                                -------------------Cleaning--------------------------------------    
                                ,CASE WHEN DayOfWeek = 'Mondayhrs'
                                 THEN (SELECT sum(Mondayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Mondayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Tuesdayhrs'
                                 THEN (SELECT sum(Tuesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Tuesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                 WHEN DayOfWeek = 'Wednesdayhrs'
                                 THEN (SELECT sum(Wednesdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Wednesdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                  
                                   WHEN DayOfWeek = 'Thursdayhrs'
                                   THEN (SELECT sum(Thursdayhrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   Thursdayhrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                
                                   WHEN DayOfWeek = 'Fridayhrs'
                                   THEN (SELECT sum(FridayHrs) from(SELECT 
								   distinct employeedetail.EmployeeId,
								   FridayHrs
								   FROM  employeedetail 
								   INNER JOIN aspnetuserroles ON aspnetuserroles.RoleId = employeedetail.Role
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = employeedetail.EmployeeId 
								   WHERE aspnetuserroles.RoleId = '31CF918D-B8FE-4490-B2D7-27324BFE89B4' 
								   AND employeeworktype.WorkType = 2)t)
								   -   
								   (SELECT Count(EstimatedHrsPerUser) from jobassigntoMapping 
								   INNER JOIN employeeworktype ON employeeworktype.EmployeeID = jobassigntoMapping.AssignTo 
								   WHERE CAST(jobassigntoMapping.DateBooked as DATE) = cast(DateValue as DATE) AND jobassigntoMapping.IsDelete=0
								   AND (employeeworktype.WorkType = 1 OR employeeworktype.WorkType = 2))
                                     
                                    ELSE null
                                    END AS CleaningHoursAvailable 
                                 
                                    ,(SELECT COUNT(Estimatedhours) FROM Jobs WHERE DateBooked = DateValue) HoursBooked
                                    
                                    FROM (
                                    SELECT DateValue ,DATENAME(dw,DateValue) + 'hrs' As DayOfWeek 
                                     FROM @TableOfDates)t
                                    )tt";
                }

                var AvailableWorkinghours = Context.Database.SqlQuery<WorkingHoursModel>(sql).AsQueryable();
                return AvailableWorkinghours;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
