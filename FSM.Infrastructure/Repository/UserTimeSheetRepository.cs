using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSM.Core.ViewModels;

namespace FSM.Infrastructure.Repository
{
    public class UserTimeSheetRepository : GenericRepository<FsmContext, UserTimeSheet>, IUserTimeSheetRepository
    {
        public IQueryable<TimeSheetViewModel> GetSheetAll(string UserIds)
        {
            string sql = @"
                           DECLARE @sql nvarchar(max)

                          SET @sql='select 
                           UTS.Id, UTS.UserId, UTS.JobId, UTS.JobDate, UTS.StartTime, UTS.EndTime,
                           UTS.TimeSpent, UTS.Reason, UTS.IsRunning, 
                           S.StreetName+'' ''+S.Suburb+'' ''+S.State+'' ''+CONVERT(varchar(50), S.PostalCode) as Site,
                           U.UserName,
                           C.CustomerLastName,
                           J.JobId as Job,
                           J.JobNo
                           from dbo.UserTimeSheet UTS
                           inner join dbo.Jobs J on UTS.JobId=J.Id
                           inner join dbo.CustomerSiteDetail S on J.SiteId=S.SiteDetailId
                           inner join dbo.CustomerGeneralInfo C on S.CustomerGeneralInfoId=C.CustomerGeneralInfoId
                           inner join dbo.AspNetUsers U on UTS.UserId=U.Id
                           where 1=1'

                          IF(isnull('" + UserIds + @"','')<>'')
                            BEGIN
                                SET @sql+=' AND UTS.UserId in (" + UserIds + @")'
                            END
                           
                         SET @sql+=' order by UTS.JobDate desc, UTS.StartTime desc'
                          
                        EXEC(@sql)";

            var TimeSheetList = Context.Database.SqlQuery<TimeSheetViewModel>(sql).AsQueryable();
            return TimeSheetList;
        }

        public IQueryable<TimeSheetTotal> GetSheetTotalHrs(DateTime? JobStartDate, DateTime? JobEndDate, string UserIds, string Keyword)
        {
            var StartDate = JobStartDate.HasValue ? JobStartDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            var EndDate = JobEndDate.HasValue ? JobEndDate.Value.ToString("yyyy/MM/dd") : string.Empty;

            string sql = @"DECLARE @JobStartDate nvarchar(50)= '" + StartDate + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + EndDate + @"'
                           DECLARE @Keyword nvarchar(50)= '" + Keyword + @"'
                           DECLARE @Sql nvarchar(max)

                           SET @Sql ='
                           SELECT
                           Reason
                           ,cast(sum(datediff(second,0,TimeSpent))/3600 as varchar(12)) + '':'' + 
                                   right(''0'' + cast(sum(datediff(second,0,TimeSpent))/60%60 as varchar(2)),2) +
                                   '':'' + right(''0'' + cast(sum(datediff(second,0,TimeSpent))%60 as varchar(2)),2)
                           CalculatedHour
                           FROM
                           (
                           	SELECT UTS.Reason
                           	,UTS.TimeSpent
                           	FROM UserTimeSheet UTS
                            INNER JOIN Jobs ON Jobs.Id = UTS.JobId
                            INNER JOIN CustomerGeneralInfo ON CustomerGeneralInfo.CustomerGeneralInfoId = Jobs.CustomerGeneralInfoId
                           
                           WHERE 1=1'

                           IF(ISNULL(Cast(@Keyword as nvarchar(50)),'') <> '')
						   BEGIN
						   SET @Sql +=' AND (Jobs.JobNo  like ''%'+@Keyword+'%'' OR CustomerGeneralInfo.CustomerLastName like ''%'+@Keyword+'%'')'
						   END
                           
                            IF(isnull('" + UserIds + @"','')<>'')
                            BEGIN
                                SET @sql+=' AND UTS.UserId in (" + UserIds + @")'
                            END
                           
                           IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' and 
                           ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '')
                           BEGIN
                           	SET @Sql +=' AND JobDate between '''+ cast(@JobStartDate as nvarchar(50))+''' 
                           	AND '''+ cast(@JobEndDate as nvarchar(50))+''''
                           END
                           
                           ELSE IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' )
                           BEGIN
                           	SET @Sql +=' AND JobDate >= '''+ cast(@JobStartDate as nvarchar(50))+''''
                           END
                           
                           ELSE IF(ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '' )
                           BEGIN
                           	SET @Sql +=' AND JobDate <= '''+ cast(@JobEndDate as nvarchar(50))+''''
                           END
                           
                           SET @Sql +=')t
                           Group by t.Reason'
                                                      
                           EXEC(@Sql)";

            var CustomerSiteList = Context.Database.SqlQuery<TimeSheetTotal>(sql).AsQueryable();
            return CustomerSiteList;
        }

        public IQueryable<string> GetSheetHrs(DateTime? JobStartDate, DateTime? JobEndDate, string UserIds, string Keyword)
        {
            var StartDate = JobStartDate.HasValue ? JobStartDate.Value.ToString("yyyy/MM/dd") : string.Empty;
            var EndDate = JobEndDate.HasValue ? JobEndDate.Value.ToString("yyyy/MM/dd") : string.Empty;

            string sql = @"DECLARE @JobStartDate nvarchar(50)= '" + StartDate + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + EndDate + @"'
                           DECLARE @Keyword nvarchar(50)= '" + Keyword + @"'
                           DECLARE @Sql nvarchar(max)

                           SET @Sql ='
                           select  cast(sum(datediff(second,0,TimeSpent))/3600 as varchar(12)) + '':'' + 
                           right(''0'' + cast(sum(datediff(second,0,TimeSpent))/60%60 as varchar(2)),2) +
                           '':'' + right(''0'' + cast(sum(datediff(second,0,TimeSpent))%60 as varchar(2)),2)
                           FROM
                           (
                           	SELECT UTS.Reason
                           	,UTS.TimeSpent
                           	FROM UserTimeSheet UTS
                            INNER JOIN Jobs ON Jobs.Id = UTS.JobId
                            INNER JOIN CustomerGeneralInfo ON CustomerGeneralInfo.CustomerGeneralInfoId = Jobs.CustomerGeneralInfoId
                           
                           WHERE 1=1'

                           IF(ISNULL(Cast(@Keyword as nvarchar(50)),'') <> '')
						   BEGIN
						   SET @Sql +=' AND (Jobs.JobId  like ''%'+@Keyword+'%'' OR CustomerGeneralInfo.CustomerLastName like ''%'+@Keyword+'%'')'
						   END

                           IF(isnull('" + UserIds + @"','')<>'')
                            BEGIN
                                SET @sql+=' AND UTS.UserId in (" + UserIds + @")'
                            END
                           
                           IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' and 
                           ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '')
                           BEGIN
                           	SET @Sql +=' AND JobDate between '''+ cast(@JobStartDate as nvarchar(50))+''' 
                           	AND '''+ cast(@JobEndDate as nvarchar(50))+''''
                           END
                           
                           ELSE IF(ISNULL(cast(@JobStartDate as nvarchar(50)),'') <> '' )
                           BEGIN
                           	SET @Sql +=' AND JobDate >= '''+ cast(@JobStartDate as nvarchar(50))+''''
                           END
                           
                           ELSE IF(ISNULL(cast(@JobEndDate as nvarchar(50)),'') <> '' )
                           BEGIN
                           	SET @Sql +=' AND JobDate <= '''+ cast(@JobEndDate as nvarchar(50))+''''
                           END
                           
                           SET @Sql +=')t'
                                                      
                           EXEC(@Sql)";

            var CustomerSiteList = Context.Database.SqlQuery<string>(sql).AsQueryable();
            return CustomerSiteList;
        }
        public IQueryable<TimeSheetViewModel> GetAggregateSheetAll(string UserIds)
        {
            //string sql = @"
            //              DECLARE @sql nvarchar(max)

            //              SET @sql='
            //              select
            //                 UserId, JobId, JobDate, StartTime, EndTime
            //               , (select convert(varchar(5),DateDiff(s, StartTime, EndTime)/3600)+'':''+
            //                  convert(varchar(5),DateDiff(s, StartTime, EndTime)%3600/60)+'':''+
            //                  convert(varchar(5),(DateDiff(s, StartTime, EndTime)%60))) as TotalTimeSpent
            //               , JobTimeSpent , LunchTimeSpent , PersonnalTimeSpent
            //               , Site, UserName, CustomerLastName, JobNo
            //                 from (
            //                 select distinct
            //                 UTS.UserId, UTS.JobId, UTS.JobDate
            //               , (SELECT MIN(StartTime) FROM UserTimeSheet where JobId = UTS.JobId) StartTime
            //               , (SELECT Max(EndTime) FROM UserTimeSheet where JobId = UTS.JobId) EndTime
            //               ,(SELECT CAST(t.time_sum/3600 AS VARCHAR(2)) + '':''
            //                                          + CAST(t.time_sum%3600/60 AS VARCHAR(2)) + '':''
            //                                          + CAST(((t.time_sum%3600)%60) AS VARCHAR(2))
            //                                    FROM ( SELECT SUM(DATEDIFF(S, StartTime, EndTime)) AS time_sum
            //                                                FROM UserTimeSheet 
            //                                    where JobId = UTS.JobId
            //                                    and Reason=''Job'') t) JobTimeSpent
            //                           ,(SELECT CAST(t.time_sum/3600 AS VARCHAR(2)) + '':''
            //                                    + CAST(t.time_sum%3600/60 AS VARCHAR(2)) + '':''
            //                                    + CAST(((t.time_sum%3600)%60) AS VARCHAR(2))
            //                              FROM ( SELECT SUM(DATEDIFF(S, StartTime, EndTime)) AS time_sum
            //                                          FROM UserTimeSheet 
            //                              WHERE JobId = UTS.JobId
            //                              and Reason=''Lunch'') t) LunchTimeSpent   
            //                           ,(SELECT CAST(t.time_sum/3600 AS VARCHAR(2)) + '':''
            //                                    + CAST(t.time_sum%3600/60 AS VARCHAR(2)) + '':''
            //                                    + CAST(((t.time_sum%3600)%60) AS VARCHAR(2))
            //                              FROM ( SELECT SUM(DATEDIFF(S, StartTime, EndTime)) AS time_sum
            //                                          FROM UserTimeSheet 
            //                              WHERE JobId = UTS.JobId
            //                              and Reason=''Personal'') t) PersonnalTimeSpent 
            //              ,S.StreetName+'' ''+S.Suburb+'' ''+S.State+'' ''+CONVERT(varchar(50), S.PostalCode) as Site,
            //               U.UserName,
            //               C.CustomerLastName,
            //               J.JobId as Job,
            //               J.JobNo
            //               from dbo.UserTimeSheet UTS
            //               left outer join dbo.Jobs J on UTS.JobId=J.Id
            //               left outer join dbo.CustomerSiteDetail S on J.SiteId=S.SiteDetailId
            //               left outer join dbo.CustomerGeneralInfo C on S.CustomerGeneralInfoId=C.CustomerGeneralInfoId
            //               left outer join dbo.AspNetUsers U on UTS.UserId=U.Id
            //               where 1=1 '

            //              IF(isnull('" + UserIds + @"','')<>'')
            //                BEGIN
            //                       SET @sql+=' AND UTS.UserId in (" + UserIds + @")'
            //                END

            //               SET @sql+=')t'
            //               SET @sql+=' order by UserName asc'

            //            EXEC(@sql)
            //            print (@sql)";

            string sql = @"
                          DECLARE @sql nvarchar(max)

                          SET @sql='  select userid, JobDate, Min(StartTime)StartTime, Max(EndTime)EndTime, U.UserName from UserTimeSheet
                                     inner join dbo.AspNetUsers U on userid = U.Id
                                    where 1=1 '

                          IF(isnull('" + UserIds + @"', '') <> '')
                            BEGIN
                                   SET @sql += ' AND userid in (" + UserIds + @")'
                            END

                           SET @sql+='   group by userid, JobDate, UserName order by UserName asc '  
                            EXEC(@sql)
                            print (@sql)";


            var TimeSheetList = Context.Database.SqlQuery<TimeSheetViewModel>(sql).AsQueryable();
            return TimeSheetList;
        }

        public IQueryable<LaboutCostReportViewModel> GetLabourCostPerHour(List<Nullable<Guid>> EmployeeIds, DateTime? StartDate, DateTime? EndDate)
        {
            try
            {
                if (EmployeeIds.Count > 0)
                {
                    string EmployeesIds = "";
                    foreach (Guid id in EmployeeIds)
                    {
                        EmployeesIds = EmployeesIds + "'" + id + "'" + ",";
                    }
                    EmployeesIds = EmployeesIds.TrimEnd(',');


                    string sql = @"SELECT ED.EID,ED.EmployeeId,ISNULL(ED.FirstName, '') + ' ' + ISNULL(ED.LastName, '') AS Name, RSC.BaseRate,RSC.S_WC,RSC.AL_PH,RSC.TAFE,RSC.Payroll,
                            RSC.Cont_MV_EQ_Cost,RSC.Emp_MV_Cost,RSC.Equip_Cost,RSC.Emp_Mob_Ph_Cost,
							Gross_Labour_Cost AS GrossLabourCost
                            FROM employeedetail ED LEFT JOIN RateCategory RC
                            ON ED.CategoryId = RC.CategoryId
                            LEFT JOIN RateSubCategory RSC
                            ON ED.SubCategoryId = RSC.SubCategoryId
                            WHERE ED.EmployeeId IN(" + EmployeesIds + @") ";

                    var labourcost = Context.Database.SqlQuery<LaboutCostReportViewModel>(sql).AsQueryable();
                    return labourcost;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<LabourCostperhourReportViewModel> GetLabourCost(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds)
        {

            var Start = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            var End = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            string EmployeesIds = "";
            foreach (Guid id in EmployeeIds)
            {
                EmployeesIds = EmployeesIds + "'" + id + "'" + ",";
            }
            EmployeesIds = EmployeesIds.TrimEnd(',');

            string sql = @"
                        DECLARE @JobStartDate nvarchar(50)= '" + Start + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + End + @"'
                                
                                select *
						  ,TimeSpentInSeconds/3600 TimeSpentHour
						  ,TimeSpentInSeconds / 60 - (TimeSpentInSeconds/3600 * 60) TimeSpentMinute
						  ,TimeSpentInSeconds - ((TimeSpentInSeconds/3600 * 3600) + (TimeSpentInSeconds / 60 - TimeSpentInSeconds/3600 * 60) * 60) TimeSpentSecond
						   from(
						        
                                SELECT (ISNULL(ED.FirstName,'')+' '+ ISNULL(Ed.LastName,'')) AS Name,Ed.EID,
								Ed.EmployeeId
                                ,JobDate
                                ,Reason
                                ,  sum( DATEPART(SECOND, TimeSpent) + 60 * 
									  DATEPART(MINUTE, TimeSpent) + 3600 * 
									  DATEPART(HOUR, TimeSpent ) 
									) as TimeSpentInSeconds
                                ,AB.ST
                                ,ABS.EST
                                ,DATEDIFF(MINUTE,AB.ST,ABS.EST)/60.00 FTOLHour
                                FROM employeedetail ED LEFT JOIN usertimesheet UT
                                ON ED.EmployeeId = UT.UserId

                                INNER JOIN (SELECT ST,EID FROM (SELECT EID,Min(StartTime) AS ST
                                FROM employeedetail ED LEFT JOIN usertimesheet UT
                                ON ED.EmployeeId = UT.UserId where JobDate between @JobStartDate and @JobEndDate
                                Group By EID)A) AS AB ON AB.EID = ED.EID

                                INNER JOIN (SELECT EST,EID FROM (SELECT EID,Max(EndTime) AS EST
                                FROM employeedetail ED LEFT JOIN usertimesheet UT
                                ON ED.EmployeeId = UT.UserId where JobDate between @JobStartDate and @JobEndDate
                                Group By EID)A) AS ABS ON ABS.EID = ED.EID
                                 where ED.EmployeeId IN(" + EmployeesIds + @") and JobDate between @JobStartDate and @JobEndDate	
                                GROUP BY Reason,ED.FirstName,Ed.LastName,Ed.EID,EmployeeId
                                ,JobDate,ST,EST)t
                                ORDER BY t.EID";











            //SELECT (ISNULL(ED.FirstName,'')+' '+ ISNULL(Ed.LastName,'')) AS Name,
            //                            Ed.EID,
            //                            JobDate,
            //                            Reason,(SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,TimeSpent))/60.00)) AS TimeSpent
            //                            ,AB.ST,ABS.EST,DATEDIFF(MINUTE,AB.ST,ABS.EST)/60.00 FTOLHour
            //                            FROM employeedetail ED LEFT JOIN usertimesheet UT
            //                            ON ED.EmployeeId = UT.UserId

            //                            INNER JOIN (SELECT ST,EID FROM (SELECT EID,Min(StartTime) AS ST
            //                            FROM employeedetail ED LEFT JOIN usertimesheet UT
            //                            ON ED.EmployeeId = UT.UserId
            //                            Group By EID)A) AS AB ON AB.EID = ED.EID

            //                            INNER JOIN (SELECT EST,EID FROM (SELECT EID,Max(EndTime) AS EST
            //                            FROM employeedetail ED LEFT JOIN usertimesheet UT
            //                            ON ED.EmployeeId = UT.UserId
            //                            Group By EID)A) AS ABS ON ABS.EID = ED.EID
            //                            where ED.EmployeeId IN(" + EmployeesIds + @") and JobDate between @JobStartDate and @JobEndDate
            //                            GROUP BY Reason,Ed.EID,EmployeeId
            //                            ,JobDate,ST,EST,FirstName,LastName
            //                            ORDER BY Ed.EID"
            //                            ;

            var LabourCostPerHour = Context.Database.SqlQuery<LabourCostperhourReportViewModel>(sql).AsQueryable();
            return LabourCostPerHour;
        }

        public IQueryable<SalesBonusReportViewModel> GetSalesBonusReport(List<Nullable<Guid>> EmployeeIds, DateTime? StartDate, DateTime? EndDate)
        {
            var Start = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            var End = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;

            string EmployeesIds = "";
            foreach (Guid id in EmployeeIds)
            {
                EmployeesIds = EmployeesIds + "'" + id + "'" + ",";
            }
            EmployeesIds = EmployeesIds.TrimEnd(',');


            string sql = @"DECLARE @StartDate NVARCHAR(50) = '" + Start + @"'
            DECLARE @EndDate NVARCHAR(50) = '" + End + @"'
           SELECT id
       , Sitefilename
          ,Price SaleIncome
       , CASE WHEN((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par  THEN(totalcost + salesbonus) ELSE totalcost END AS TotalCost
       , CASE WHEN ((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par THEN(labourprofit - salesbonus) ELSE labourprofit END AS LobourIncome
       , CASE WHEN((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par  THEN(labourprofit - salesbonus) / NULLIF(revhours, 0) ELSE labourprofit / NULLIF(revhours, 0) END AS LobourIncomePerHour
       , revhours
       , jsprice AS StockItemCost
       , totalemployeecost AS LabourCost
       , totalemployeecost / NULLIF(revhours, 0) AS LabourCostPerHour
       , pojcost AS  JSPOCost
       , CASE WHEN((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par THEN(labourprofit - salesbonus) - totalemployeecost ELSE labourprofit - totalemployeecost END AS LabourProfit
       , CASE WHEN((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par THEN((labourprofit - salesbonus) - totalemployeecost) / NULLIF(revhours, 0) ELSE(labourprofit - totalemployeecost) / CASE WHEN revhours <= 0.00 THEN 1 ELSE REVHours END END AS LabourProfitPerHour
       , revhours AS LabourHours
       , CASE WHEN((labourprofit - salesbonus) / NULLIF(revhours, 0)) > gp_hour_par THEN salesbonus ELSE 0.00 END AS SalesBonus
       , ISNULL(nrlhours, 0.0) AS NRLHours
       , invoiceno
       , invoicedate
       , customerlastname
       ,OTRWName
FROM(


      SELECT id
              , SiteFileName
              , price
              , jsprice
              , pojcost
              , revhours
              , grosslabourcost
              , totalemployeecost
              , gp_hour_par
              , totalcost
              , labourprofit
              , CASE WHEN sb1 > sb2 THEN sb2 ELSE sb1 END AS SalesBonus
              , nrlhours
              , invoiceno
              , invoicedate
              , customerlastname
              , OTRWName
       FROM(


              SELECT JO.id
                     , CAST(Iv.Price - (iv.Price/11) as decimal(12,2))As price
                     , site.SiteFileName
                     , SUM(ISNULL(JS.price, 0.00)) JSPrice
                     , SUM(ISNULL(POJ.cost, 0.00)) POJCost
                     , revhours
                     , grosslabourcost
                     , TotalEmployeeCost
                     , gp_hour_par
                     , (SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + totalemployeecost) AS TotalCost
                     , (CAST(Iv.Price - (iv.Price/11) as decimal(12,2)) - (SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + totalemployeecost)) AS LabourProfit
                     , ((CAST(Iv.Price - (iv.Price/11) as decimal(12,2)) - (SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + totalemployeecost)) * 0.1) AS SB1
                     , (((CAST(Iv.Price - (iv.Price/11) as decimal(12,2)) - (SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + totalemployeecost)) * 0.1) + ((IV.price - (SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + totalemployeecost)) * 0.9 / CASE  WHEN(((SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + ISNULL(totalemployeecost, 0.00)))) <= 0.00 THEN 1 ELSE(((SUM(ISNULL(JS.price, 0.00)) + SUM(ISNULL(POJ.cost, 0.00)) + ISNULL(totalemployeecost, 0.00)))) END - gp_hour_par) * revhours) AS SB2
                     , nrlhours
                     , IV.invoiceno
                     , IV.invoicedate
                     , CGI.customerlastname
                     , OTRWName
              FROM jobs JO
              LEFT JOIN invoice IV ON JO.id = IV.employeejobid
              LEFT JOIN (SELECT SUM(Price) Price,JobId from jobstock Group BY JobId) AS JS ON JS.JobId = JO.Id 
              LEFT JOIN purchaseorderbyjob POJ ON JO.id = POJ.jobid 
              LEFT JOIN CustomerSiteDetail site ON JO.SiteId = site.SiteDetailId
              LEFT JOIN customergeneralinfo CGI ON JO.customergeneralinfoid = CGI.customergeneralinfoid
              LEFT JOIN(
                     SELECT jobid
                           , SUM(tspent) REVHours
                           , SUM(grosslabourcost) GrossLabourCost
                           , SUM(totalemployeecost) TotalEmployeeCost
                           , gp_hour_par
                           , OTRWName
                     FROM(
                           SELECT jobid
                                  , (SUM(DATEDIFF(MINUTE, '00:00:00.0000000', CONVERT(TIME, timespent)) / 60.00)) AS TSpent
                                  , grosslabourcost
                                  , Round(((SUM(DATEDIFF(MINUTE, '00:00:00.0000000', CONVERT(TIME, timespent)) / 60.00)) * grosslabourcost), 2) AS TotalEmployeeCost
                                  , gp_hour_par
                                    ,AB.OTRWName
                           FROM usertimesheet UTS
              LEFT JOIN(SELECT employeeid
                                , gp_hour_par
                                , grosslabourcost
                                ,A.OTRWName
                        FROM(
                                SELECT ED.employeeid
                                    --, gp_hour_par
                                    --, gross_labour_cost AS GrossLabourCost
                                    , isnull(gp_hour_par, 0.00) as gp_hour_par
                                    , isnull(gross_labour_cost, 0.00) AS GrossLabourCost
                                ,ED.UserName OTRWName
                                                       --,JAM.JobId
                                FROM employeedetail ED
                                LEFT JOIN ratecategory RC ON ED.categoryid = RC.categoryid
                                LEFT JOIN ratesubcategory RSC ON ED.subcategoryid = RSC.subcategoryid
                                                       --LEFT JOIN JobAssignToMapping JAM ON JAM.AssignTo = ED.EmployeeId AND JAM.IsDelete = 0 
                                ) A
                        ) AS AB ON AB.employeeid = UTS.userid
                           WHERE ISNULL(isfirsttraveling, 0) <> 1
                                  AND reason <> 'Lunch'
                                  AND reason <> 'Personal'
                                 AND AB.employeeid IN(" + EmployeesIds + @") 
                           GROUP BY userid
                                  , grosslabourcost
                                  , gp_hour_par
                                  , jobid
                                  , OTRWName
                           ) A
                     GROUP BY jobid
                           , gp_hour_par,OTRWName

                           --checkd



                     ) AS AAA ON AAA.jobid = JO.id

                     LEFT JOIN(
                     SELECT jobid
                           , SUM(tspent) NRLHours
                     FROM(
                           SELECT jobid
                                  , (SUM(DATEDIFF(MINUTE, '00:00:00.0000000', CONVERT(TIME, timespent)) / 60.00)) AS TSpent
                           FROM usertimesheet UTS
                           WHERE jobid IN(
                                         SELECT employeejobid
                                         FROM invoice
                                         WHERE createddate BETWEEN @StartDate
                                                       AND @EndDate
                                         )
                                  AND ISNULL(isfirsttraveling, 0) = 1
                                  AND reason = 'Lunch'
                                  AND reason = 'Personal'
                           GROUP BY userid
                                  , jobid
                                   
                           ) A
                     GROUP BY jobid
                     ) AS AAAA ON AAAA.jobid = Jo.id

              WHERE JO.id IN(
                           SELECT employeejobid
                           FROM invoice
                           WHERE createddate BETWEEN @StartDate
                                         AND @EndDate 
                           )--AND JO.Status = 15

              GROUP BY JO.id
                     , IV.price
                     , site.SiteFileName
                     , revhours
                     , grosslabourcost
                     , totalemployeecost
                     , gp_hour_par
                     , nrlhours
                     , invoiceno
                     , invoicedate
                     , CGI.customerlastname
                      ,OTRWName
   --ORDER BY CustomerLastName
              ) AS A
       ) AS AA  where invoicedate between   @StartDate and @EndDate ORDER BY OTRWName,CustomerLastName";

            var LabourCostPerHour = Context.Database.SqlQuery<SalesBonusReportViewModel>(sql).AsQueryable();
            return LabourCostPerHour;
        }


        public IQueryable<UnpaidInvoiceCoreReportViewModel> GetUnpaidReportList(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var Start = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                var End = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                string sql = @"
                        DECLARE @StartDate nvarchar(50)= '" + Start + @"'
                           DECLARE @EndDate nvarchar(50)= '" + End + @"'
                         select * from(    SELECT JO.JobNo
                                , IV.InvoiceNo
                                                       ,iv.CreatedDate
                                ,JT.JobType
                                ,JS.JobStatus
                                ,CSD.SiteFileName
                                ,ISNULL(IV.Price, 0) AS Amount
                                ,JO.DateBooked
                                ,IV.InvoiceDate
                                FROM Invoice IV
                                LEFT JOIN Jobs JO
                                ON JO.Id = IV.EmployeeJobId
                                LEFT JOIN JobType JT
                                ON JO.JobType = JT.Value 
                                LEFT JOIN JobStatus JS
                                ON JO.Status = JS.Value
                                LEFT JOIN CustomerSiteDetail CSD
                                ON JO.SiteId = CSD.SiteDetailId
                                WHERE((IV.Due > 0 OR IV.Due IS NULL OR IV.Paid = 0 OR IV.Paid IS NULL) and iv.InvoiceType='Invoice' and iv.IsDelete = 0 and 
                                IV.InvoiceDate >= @StartDate  and IV.InvoiceDate <= @EndDate)) t  order by t.InvoiceDate";
                var unpaidreports = Context.Database.SqlQuery<UnpaidInvoiceCoreReportViewModel>(sql).AsQueryable();
                return unpaidreports;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public IQueryable<OperationalCoreViewModel> GetoperationalReport(DateTime? startDate, DateTime? endDate, int ReportType)
        {
            try
            {
                var Start = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                var End = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                string sql = @"
                        DECLARE @JobStartDate nvarchar(50)= '" + Start + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + End + @"'
                                
                               
                                         SELECT 
                               SUM(TotalEmployeeTime) TotalEmployeeTime
                               ,MIN(JobStartDate) JobStartDate
                               ,MAX(JobEndDate) JobEndDate
                               ,SUM(TimeSpentInDays) TimeSpentInDays
                               ,SUM(Price) Price
                               ,SUM(LabourCost) LabourCost
                               ,SUM(LabourCost)/SUM(TotalEmployeeTime) AS LabourCostPerHour
                               ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice)) AS LabourIncome
                               ,(SUM(Price) -  SUM(LabourCost))/SUM(TotalEmployeeTime) AS LabourIncomePerHour
                               ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                               ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                                FROM 
                               (SELECT 1 AS Number,
                               ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                               ,MIN(JobDate) AS JobStartDate 
                               ,MAX(JobDate) AS  JobEndDate
                               ,DATEDIFF(DAY,MIN(JobDate),MAX(JobDate)) AS TimeSpentInDays
                               ,SUM(ISNULL(IV.Price,0.00)) AS Price
                               ,Gross_Labour_Cost AS GrossLabourCost
                               ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                               (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                               +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                               ,SUM(JS.Price) AS JSPrice
                               ,SUM(POJ.Cost) AS POJPrice
                                FROM JOBS JO
                               INNER JOIN UserTimeSheet UTS 
                                      ON JO.Id =  UTS.JobId
                               LEFT JOIN Invoice IV 
                                      ON JO.Id = IV.EmployeeJobId
                               LEFT JOIN employeedetail ED 
                                      ON UTS.UserId = ED.EmployeeId
                               LEFT JOIN RateCategory RC
                                      ON ED.CategoryId = RC.CategoryId
                               LEFT JOIN RateSubCategory RSC
                                      ON ED.SubCategoryId = RSC.SubCategoryId
                               LEFT JOIN JobStock JS 
                                      ON JO.Id = JS.JobId
                               LEFT JOIN PurchaseOrderByJob POJ 
                                      ON JO.Id = POJ.JobID
                               WHERE ISNULL(IsFirstTraveling,0) <> 1 
                               AND Reason <> 'Lunch' 
                               AND Reason <> 'Personal'
                               AND Status = 15
                                          and UTS.JobDate between @JobStartDate and @JobEndDate
                               GROUP BY JO.Status
                                      ,RSC.BaseRate
                                      ,RSC.S_WC
                                      ,AL_PH,TAFE
                                      ,Payroll
                                      ,Cont_MV_EQ_Cost
                                      ,Emp_MV_Cost
                                      ,Equip_Cost
                                      ,Emp_Mob_Ph_Cost
                                               ,Gross_Labour_Cost
                                      )A";

                var operationalReport = Context.Database.SqlQuery<OperationalCoreViewModel>(sql).AsQueryable();
                return operationalReport;

                //string sql = "";
                //switch (ReportType)
                //{
                //    case 1: //week
                //        sql = @"SELECT 
                //               SUM(TotalEmployeeTime) TotalEmployeeTime
                //               ,MIN(JobStartDate) JobStartDate
                //               ,MAX(JobEndDate) JobEndDate
                //               ,SUM(TimeSpentInDays) TimeSpentInDays
                //               ,SUM(Price) Price
                //               ,SUM(LabourCost) LabourCost
                //               ,SUM(LabourCost)/SUM(TotalEmployeeTime) AS LabourCostPerHour
                //               ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice)) AS LabourIncome
                //               ,(SUM(Price) -  SUM(LabourCost))/SUM(TotalEmployeeTime) AS LabourIncomePerHour
                //               ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                //               ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                //                FROM 
                //               (SELECT 1 AS Number,
                //               ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                //               ,MIN(JobDate) AS JobStartDate 
                //               ,MAX(JobDate) AS  JobEndDate
                //               ,DATEDIFF(DAY,MIN(JobDate),MAX(JobDate)) AS TimeSpentInDays
                //               ,SUM(ISNULL(IV.Price,0.00)) AS Price
                //               ,Gross_Labour_Cost AS GrossLabourCost
                //               ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                //               (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                //               +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                //               ,SUM(JS.Price) AS JSPrice
                //               ,SUM(POJ.Cost) AS POJPrice
                //                FROM JOBS JO
                //               INNER JOIN UserTimeSheet UTS 
                //                      ON JO.Id =  UTS.JobId
                //               LEFT JOIN Invoice IV 
                //                      ON JO.Id = IV.EmployeeJobId
                //               LEFT JOIN employeedetail ED 
                //                      ON UTS.UserId = ED.EmployeeId
                //               LEFT JOIN RateCategory RC
                //                      ON ED.CategoryId = RC.CategoryId
                //               LEFT JOIN RateSubCategory RSC
                //                      ON ED.SubCategoryId = RSC.SubCategoryId
                //               LEFT JOIN JobStock JS 
                //                      ON JO.Id = JS.JobId
                //               LEFT JOIN PurchaseOrderByJob POJ 
                //                      ON JO.Id = POJ.JobID
                //               WHERE ISNULL(IsFirstTraveling,0) <> 1 
                //               AND Reason <> 'Lunch' 
                //               AND Reason <> 'Personal'
                //               AND Status = 15
                //                           AND JobDate BETWEEN   GETDATE()-(SELECT DATEPART(dw,GETDATE()) AS DayNumber) AND GETDATE() 
                //               GROUP BY JO.Status
                //                      ,RSC.BaseRate
                //                      ,RSC.S_WC
                //                      ,AL_PH,TAFE
                //                      ,Payroll
                //                      ,Cont_MV_EQ_Cost
                //                      ,Emp_MV_Cost
                //                      ,Equip_Cost
                //                      ,Emp_Mob_Ph_Cost
                //                               ,Gross_Labour_Cost
                //                      )A";
                //        var operationalReport = Context.Database.SqlQuery<OperationalCoreViewModel>(sql).AsQueryable();
                //        return operationalReport;
                //        break;

                //    case 2:
                //        sql = @"SELECT 
                //                   SUM(TotalEmployeeTime) TotalEmployeeTime
                //                   ,MIN(JobStartDate) JobStartDate
                //                   ,MAX(JobEndDate) JobEndDate
                //                   ,SUM(TimeSpentInDays) TimeSpentInDays
                //                   ,SUM(Price) Price
                //                   ,SUM(LabourCost) LabourCost
                //                   ,SUM(LabourCost)/SUM(TotalEmployeeTime) AS LabourCostPerHour
                //                   ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice)) AS LabourIncome
                //                   ,(SUM(Price) -  SUM(LabourCost))/SUM(TotalEmployeeTime) AS LabourIncomePerHour
                //                   ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                //                   ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                //                    FROM 
                //                   (SELECT 1 AS Number,
                //                   ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                //                   ,MIN(JobDate) AS JobStartDate 
                //                   ,MAX(JobDate) AS  JobEndDate
                //                   ,DATEDIFF(DAY,MIN(JobDate),MAX(JobDate)) AS TimeSpentInDays
                //                   ,SUM(ISNULL(IV.Price,0.00)) AS Price
                //                   ,Gross_Labour_Cost AS GrossLabourCost
                //                   ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                //                   (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                //                   +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                //                   ,SUM(JS.Price) AS JSPrice
                //                   ,SUM(POJ.Cost) AS POJPrice
                //                   FROM JOBS JO
                //                   INNER JOIN UserTimeSheet UTS 
                //                          ON JO.Id =  UTS.JobId
                //                   LEFT JOIN Invoice IV 
                //                          ON JO.Id = IV.EmployeeJobId
                //                   LEFT JOIN employeedetail ED 
                //                          ON UTS.UserId = ED.EmployeeId
                //                   LEFT JOIN RateCategory RC
                //                          ON ED.CategoryId = RC.CategoryId
                //                   LEFT JOIN RateSubCategory RSC
                //                          ON ED.SubCategoryId = RSC.SubCategoryId
                //                   LEFT JOIN JobStock JS 
                //                          ON JO.Id = JS.JobId
                //                   LEFT JOIN PurchaseOrderByJob POJ 
                //                          ON JO.Id = POJ.JobID
                //                   WHERE ISNULL(IsFirstTraveling,0) <> 1 
                //                   AND Reason <> 'Lunch' 
                //                   AND Reason <> 'Personal'
                //                   AND Status = 15
                //                               AND JobDate BETWEEN  DATEADD(day,DATEDIFF(day,'19000101',DATEADD(month,DATEDIFF(MONTH,0,DATEADD(MONTH,-1,GETDATE())),30))/7*7,'19000101')  AND GETDATE() 
                //                   GROUP BY JO.Status
                //                          ,RSC.BaseRate
                //                          ,RSC.S_WC
                //                          ,AL_PH,TAFE
                //                          ,Payroll
                //                          ,Cont_MV_EQ_Cost
                //                          ,Emp_MV_Cost
                //                          ,Equip_Cost
                //                          ,Emp_Mob_Ph_Cost
                //                                   ,Gross_Labour_Cost
                //                          )A 
                //            ";
                //        var operationalReportmonthly = Context.Database.SqlQuery<OperationalCoreViewModel>(sql).AsQueryable();
                //        return operationalReportmonthly;
                //        break;

                //    case 3:

                //        sql = @"DECLARE @VAL1 Datetime;
                //                    Select  @VAL1 =  dateadd(qq, DateDiff(qq, 0, GETDATE()), 0) 
                //                    SET @VAL1 = (SELECT DATEADD(wk, DATEDIFF(wk, 1, 
                //                    CASE DATEPART(dw,@VAL1)
                //                    WHEN 1 THEN DATEADD(d,-1,@VAL1)
                //                    ELSE @VAL1
                //                    END
                //                    ), 0))



                //                    DECLARE @VAL Datetime;
                //                    --Select  @VAL =  dateadd(qq, DateDiff(qq, 0, GETDATE()), 0) 
                //                    select @VAL=  DATEADD(d, -1, DATEADD(q,
                //                    DATEDIFF(q, 0, GETDATE()) + 1, 0))
                //                    --SELECT DATEADD(wk, DATEDIFF(wk, 6,@VAL), 0)
                //                    --SELECT @Val
                //                    SET @VAL = (SELECT DATEADD(wk, DATEDIFF(wk, 6, 
                //                    CASE DATEPART(dw,@Val)
                //                    WHEN 1 THEN DATEADD(d,0,@Val)
                //                    ELSE @Val
                //                    END
                //                    ), 6))


                //                                                                      SELECT 
                //                           SUM(TotalEmployeeTime) TotalEmployeeTime
                //                           ,MIN(JobStartDate) JobStartDate
                //                           ,MAX(JobEndDate) JobEndDate
                //                           ,SUM(TimeSpentInDays) TimeSpentInDays
                //                           ,SUM(Price) Price
                //                           ,SUM(LabourCost) LabourCost
                //                           ,SUM(LabourCost)/SUM(TotalEmployeeTime) AS LabourCostPerHour
                //                           ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice)) AS LabourIncome
                //                           ,(SUM(Price) -  SUM(LabourCost))/SUM(TotalEmployeeTime) AS LabourIncomePerHour
                //                           ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                //                           ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                //                    FROM 
                //                           (SELECT 1 AS Number,
                //                           ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                //                           ,MIN(JobDate) AS JobStartDate 
                //                           ,MAX(JobDate) AS  JobEndDate
                //                           ,DATEDIFF(DAY,MIN(JobDate),MAX(JobDate)) AS TimeSpentInDays
                //                           ,SUM(ISNULL(IV.Price,0.00)) AS Price
                //                           ,Gross_Labour_Cost AS GrossLabourCost
                //                           ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                //                           (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                //                           +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                //                           ,SUM(JS.Price) AS JSPrice
                //                           ,SUM(POJ.Cost) AS POJPrice
                //                    FROM JOBS JO
                //                           INNER JOIN UserTimeSheet UTS 
                //                                  ON JO.Id =  UTS.JobId
                //                           LEFT JOIN Invoice IV 
                //                                  ON JO.Id = IV.EmployeeJobId
                //                           LEFT JOIN employeedetail ED 
                //                                  ON UTS.UserId = ED.EmployeeId
                //                           LEFT JOIN RateCategory RC
                //                                  ON ED.CategoryId = RC.CategoryId
                //                           LEFT JOIN RateSubCategory RSC
                //                                  ON ED.SubCategoryId = RSC.SubCategoryId
                //                           LEFT JOIN JobStock JS 
                //                                  ON JO.Id = JS.JobId
                //                           LEFT JOIN PurchaseOrderByJob POJ 
                //                                  ON JO.Id = POJ.JobID
                //                    WHERE ISNULL(IsFirstTraveling,0) <> 1 
                //                           AND Reason <> 'Lunch' 
                //                           AND Reason <> 'Personal'
                //                           AND Status = 15
                //                                       AND JobDate BETWEEN  @VAL1  AND @VAL
                //                    GROUP BY JO.Status
                //                                  ,RSC.BaseRate
                //                                  ,RSC.S_WC
                //                                  ,AL_PH,TAFE
                //                                  ,Payroll
                //                                  ,Cont_MV_EQ_Cost
                //                                  ,Emp_MV_Cost
                //                                  ,Equip_Cost
                //                                  ,Emp_Mob_Ph_Cost
                //                                  ,Gross_Labour_Cost
                //                                  )A 
                //                    ";
                //        var operationalReportQuarterly = Context.Database.SqlQuery<OperationalCoreViewModel>(sql).AsQueryable();
                //        return operationalReportQuarterly;
                //        break;

                //    case 4:  //year
                //        sql = @"DECLARE @StartYear DATETIME ; 

                //                                DECLARE @EndYear DATETIME;
                //                                SET @StartYear = (select CAST(CAST(((((MONTH(GETDATE()) - 1) / 12) * 12) + 7) AS VARCHAR) + '-1-' + CAST(YEAR(GETDATE()) AS VARCHAR) AS DATETIME))
                //                                SET @StartYear = (SELECT DATEADD(wk, DATEDIFF(wk, 1, 
                //                                CASE DATEPART(dw,@StartYear)
                //                                WHEN 1 THEN DATEADD(d,-1,@StartYear)
                //                                ELSE @StartYear
                //                                END
                //                                ), 0))

                //                                DECLARE @Month INT
                //                                DECLARE @DAATE DATETIME
                //                                SET @Month = (SELECT DATEPART(MM,GETDATE()))
                //                                IF(@Month>=7)
                //                                BEGIN
                //                                SET @DAATE = (SELECT DATEADD(year, 1, GETDATE()))
                //                                END
                //                                ELSE
                //                                BEGIN
                //                                SET @DAATE = (SELECT DATEADD(year, 0,GETDATE()))
                //                                END
                //                                SET @EndYear = (select CAST(CAST(((((MONTH(@DAATE) - 1) / 12) * 12) + 7) AS VARCHAR) + '-1-' + CAST(YEAR(@DAATE) AS VARCHAR) AS DATETIME))
                //                                SET @EndYear = (SELECT DATEADD(wk, DATEDIFF(wk, 6, 
                //                                CASE DATEPART(dw,@EndYear)
                //                                WHEN 1 THEN DATEADD(d,0,@EndYear)
                //                                ELSE @EndYear
                //                                END
                //                                ), 6))





                //                                                                                  SELECT 
                //                                       SUM(TotalEmployeeTime) TotalEmployeeTime
                //                                       ,MIN(JobStartDate) JobStartDate
                //                                       ,MAX(JobEndDate) JobEndDate
                //                                       ,SUM(TimeSpentInDays) TimeSpentInDays
                //                                       ,SUM(Price) Price
                //                                       ,SUM(LabourCost) LabourCost
                //                                       ,SUM(LabourCost)/SUM(TotalEmployeeTime) AS LabourCostPerHour
                //                                       ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice)) AS LabourIncome
                //                                       ,(SUM(Price) -  SUM(LabourCost))/SUM(TotalEmployeeTime) AS LabourIncomePerHour
                //                                       ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                //                                       ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                //                                FROM 
                //                                       (SELECT 1 AS Number,
                //                                       ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                //                                       ,MIN(JobDate) AS JobStartDate 
                //                                       ,MAX(JobDate) AS  JobEndDate
                //                                       ,DATEDIFF(DAY,MIN(JobDate),MAX(JobDate)) AS TimeSpentInDays
                //                                       ,SUM(ISNULL(IV.Price,0.00)) AS Price
                //                                       ,Gross_Labour_Cost AS GrossLabourCost
                //                                       ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                //                                       (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                //                                       +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                //                                       ,SUM(JS.Price) AS JSPrice
                //                                       ,SUM(POJ.Cost) AS POJPrice
                //                                FROM JOBS JO
                //                                       INNER JOIN UserTimeSheet UTS 
                //                                              ON JO.Id =  UTS.JobId
                //                                       LEFT JOIN Invoice IV 
                //                                              ON JO.Id = IV.EmployeeJobId
                //                                       LEFT JOIN employeedetail ED 
                //                                              ON UTS.UserId = ED.EmployeeId
                //                                       LEFT JOIN RateCategory RC
                //                                              ON ED.CategoryId = RC.CategoryId
                //                                       LEFT JOIN RateSubCategory RSC
                //                                              ON ED.SubCategoryId = RSC.SubCategoryId
                //                                       LEFT JOIN JobStock JS 
                //                                              ON JO.Id = JS.JobId
                //                                       LEFT JOIN PurchaseOrderByJob POJ 
                //                                              ON JO.Id = POJ.JobID
                //                                WHERE ISNULL(IsFirstTraveling,0) <> 1 
                //                                       AND Reason <> 'Lunch' 
                //                                       AND Reason <> 'Personal'
                //                                       AND Status = 15
                //                                                   AND JobDate BETWEEN  @StartYear  AND @EndYear
                //                                GROUP BY JO.Status
                //                                              ,RSC.BaseRate
                //                                              ,RSC.S_WC
                //                                              ,AL_PH,TAFE
                //                                              ,Payroll
                //                                              ,Cont_MV_EQ_Cost
                //                                              ,Emp_MV_Cost
                //                                              ,Equip_Cost
                //                                              ,Emp_Mob_Ph_Cost
                //                                              ,Gross_Labour_Cost
                //                                              )A 
                                                  
                //                                ";
                //        var operationalReportyearly = Context.Database.SqlQuery<OperationalCoreViewModel>(sql).AsQueryable();
                //        return operationalReportyearly;
                //}
                //return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<PerformanceBonusCoreViewModel> GetPerformanceBonusReportList(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds)
        {

            var Start = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            var End = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            string EmployeesIds = "";
            foreach (Guid id in EmployeeIds)
            {
                EmployeesIds = EmployeesIds + "'" + id + "'" + ",";
            }
            EmployeesIds = EmployeesIds.TrimEnd(',');

            string sql = @"
                        DECLARE @JobStartDate nvarchar(50)= '" + Start + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + End + @"'
                                
                               
                                           SELECT *,((A.LabourProfitPerHour-70)*10)/100 PerformanceBonusRate,(((A.LabourProfitPerHour-70)*10)/100)*(HoursWorkCount) PerformanceBonus FROM
                                                  (
                                        select UserId,UserName AS EmployeeName, cast(H.Val as varchar(10))+':'+right(M.Val+100, 2)+':'+right(S.Val+100, 2) HoursWorked,cast(H.Val as varchar(10))+'.'+right(M.Val+100, 2) HoursWorkCount
                                        from (
                                         --Your query goes here
                                                    select dateadd(second, sum(datediff(second, 0, UserTimeSheet.TimeSpent)), 0) as sumtime
                                                          ,UserTimeSheet.UserId
                                                          ,EmployeeDetail.UserName
                                                          from UserTimeSheet 
                                                          inner join EmployeeDetail on EmployeeDetail.EmployeeId = UserTimeSheet.UserId
                                                          INNER JOIN Jobs JO ON JO.Id =  UserTimeSheet.JobId
                                                          where 
                                                            ISNULL(UserTimeSheet.IsFirstTraveling,0) <> 1 
                                                            AND UserTimeSheet.Reason <> 'Lunch' 
                                                            AND UserTimeSheet.Reason <> 'Personal'
                                                            AND JO.Status = 15 
                                                            AND UserTimeSheet.UserId IN(" + EmployeesIds + @") 
                                                           and JobDate between @JobStartDate and @JobEndDate
                                                           group by EmployeeDetail.UserName,UserTimeSheet.UserId 
                                               ) as T
                                cross apply (select datedifF(hour, 0, T.sumTime)) as H(Val)
                                cross apply (select datediff(minute, 0, dateadd(hour, -H.Val, T.sumTime))) as M(Val)
                                cross apply (select datediff(second, 0, dateadd(hour, -H.Val, dateadd(minute, -M.Val, T.sumTime)))) as S(Val)) as AB 

                                      LEFT JOIN(
                                                 SELECT 
                                                         UserId
                                                        ,(SUM(Price) -  SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))  AS LabourProfit
                                                        ,(SUM(Price) -  (SUM(LabourCost)+SUM(JSPrice)+SUM(POJPrice))-  SUM(LabourCost))/SUM(TotalEmployeeTime)  AS LabourProfitPerHour
                                                 FROM 
                                                        (SELECT 1 AS Number,
                                                        ((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) AS TotalEmployeeTime
                                                        , UTS.UserId
                                                        ,SUM(ISNULL(IV.Price,0.00)) AS Price
                                                        ,Gross_Labour_Cost AS GrossLabourCost
                                                        ,((SUM(DATEDIFF(MINUTE,'00:00:00.0000000',CONVERT(TIME,UTS.TimeSpent))/60.00))) * 
                                                        (ISNULL(RSC.BaseRate,0.00001)*ISNULL(RSC.S_WC,0.00001)*ISNULL(RSC.AL_PH,0.00001)*ISNULL(RSC.TAFE,0.00001)*ISNULL(RSC.Payroll,0.00001)) 
                                                        +(ISNULL(RSC.Cont_MV_EQ_Cost,0)+ISNULL(RSC.Emp_MV_Cost,0)+ISNULL(RSC.Equip_Cost,0)+ISNULL(RSC.Emp_Mob_Ph_Cost,0)) AS LabourCost
                                                        ,SUM(JS.Price) AS JSPrice
                                                        ,SUM(POJ.Cost) AS POJPrice
                                                  FROM JOBS JO
                                                       INNER JOIN UserTimeSheet UTS 
                                                              ON JO.Id =  UTS.JobId
                                                       LEFT JOIN Invoice IV 
                                                              ON JO.Id = IV.EmployeeJobId
                                                       LEFT JOIN employeedetail ED 
                                                              ON UTS.UserId = ED.EmployeeId
                                                       LEFT JOIN RateCategory RC
                                                              ON ED.CategoryId = RC.CategoryId
                                                       LEFT JOIN RateSubCategory RSC
                                                              ON ED.SubCategoryId = RSC.SubCategoryId
                                                       LEFT JOIN JobStock JS 
                                                              ON JO.Id = JS.JobId
                                                       LEFT JOIN PurchaseOrderByJob POJ 
                                                              ON JO.Id = POJ.JobID
                                                                         WHERE 
                                                                              ISNULL(IsFirstTraveling,0) <> 1 
                                                                              AND Reason <> 'Lunch' 
                                                                               AND Reason <> 'Personal'
                                                                               AND Status = 15 
                                                                               AND UTS.userId IN(" + EmployeesIds + @") 
                                                                               and UTS.JobDate between @JobStartDate and @JobEndDate
                                                                         GROUP BY JO.Status
                                                                                       ,RSC.BaseRate
                                                                                       ,RSC.S_WC
                                                                                       ,AL_PH,TAFE
                                                                                       ,Payroll
                                                                                       ,Cont_MV_EQ_Cost
                                                                                       ,Emp_MV_Cost
                                                                                       ,Equip_Cost
                                                                                       ,Emp_Mob_Ph_Cost
                                                                                       ,UTS.UserId
                                                                                       ,Gross_Labour_Cost
                                                                                       )A  group by UserId ) A
                                                                           
                                                                           ON AB.UserId = A.UserId";


            var LabourCostPerHour = Context.Database.SqlQuery<PerformanceBonusCoreViewModel>(sql).AsQueryable();
            return LabourCostPerHour;
        }


        //public IQueryable<InvoiceSalesReportCoreViewModel> InvoiceSalesReportDailyOrByRange(DateTime? startDate, DateTime? endDate)
        //{
        //    try
        //    {

        //        var Start = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : string.Empty;
        //        var End = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : string.Empty;
        //        string sql = @"
        //                DECLARE @StartDate nvarchar(50)= '" + Start + @"'
        //                   DECLARE @EndDate nvarchar(50)= '" + End + @"'
        //                 select * from(    SELECT JO.JobNo
        //                        , IV.InvoiceNo
								//,iv.CreatedDate
        //                        ,JT.JobType
        //                        ,JS.JobStatus
        //                        ,CSD.SiteFileName
        //                        ,ISNULL(IV.Price, 0) AS Amount
        //                        FROM Jobs JO
        //                        INNER JOIN Invoice IV
        //                        ON JO.Id = IV.EmployeeJobId
        //                        INNER JOIN JobType JT
        //                        ON JO.JobType = JT.Value 
        //                        inner JOIN JobStatus JS
        //                        ON JO.Status = JS.Value
        //                        inner JOIN CustomerSiteDetail CSD
        //                        ON JO.SiteId = CSD.SiteDetailId
        //                        WHERE(IV.Due > 0 OR IV.Due IS NULL and iv.InvoiceType='Invoice'  and Jo.JobType=2  and    IV.JobStatus=15 and   iv.CreatedDate between 
        //                        @StartDate and @EndDate ) ) t  where t.JobType='Do' and t.JobStatus='Completed' order by  t.invoiceno";
        //        var unpaidreports = Context.Database.SqlQuery<InvoiceSalesReportCoreViewModel>(sql).AsQueryable();
        //        return unpaidreports;

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}


        public IQueryable<InvoiceSalesReportCoreViewModel> InvoiceSalesReportDailyOrByRange(DateTime? startDate, DateTime? endDate)
        {
            try
            {

                var Start = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                var End = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                string sql = @"
                         DECLARE @StartDate nvarchar(50)= '" + Start + @"'
                          DECLARE @EndDate nvarchar(50)= '" + End + @"'
                          select * from(    SELECT JO.JobNo
                                , IV.InvoiceNo
								,iv.CreatedDate
                                ,JT.JobType
                                ,JS.JobStatus
                                ,CSD.SiteFileName
                                ,ISNULL(IV.Price, 0) AS Amount
                                ,JO.DateBooked
                                ,IV.InvoiceDate
                                FROM Jobs JO
                                INNER JOIN Invoice IV
                                ON JO.Id = IV.EmployeeJobId
                                INNER JOIN JobType JT
                                ON JO.JobType = JT.Value 
                                inner JOIN JobStatus JS
                                ON JO.Status = JS.Value
                                inner JOIN CustomerSiteDetail CSD
                                ON JO.SiteId = CSD.SiteDetailId
                                WHERE(iv.InvoiceType='Invoice' AND iv.IsDelete=0  AND  iv.InvoiceDate between 
                                @StartDate AND @EndDate ) ) t  order by  t.InvoiceDate";
                var unpaidreports = Context.Database.SqlQuery<InvoiceSalesReportCoreViewModel>(sql).AsQueryable();
                return unpaidreports;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IQueryable<PaolhReportViewModel> GetPaolhReport(DateTime? StartDate, DateTime? EndDate, List<Nullable<Guid>> EmployeeIds)
        {

            var Start = StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            var End = EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            string EmployeesIds = "";
            foreach (Guid id in EmployeeIds)
            {
                EmployeesIds = EmployeesIds + "'" + id + "'" + ",";
            }
            EmployeesIds = EmployeesIds.TrimEnd(',');

            string sql = @"
                           DECLARE @JobStartDate nvarchar(50)= '" + Start + @"'
                           DECLARE @JobEndDate nvarchar(50)= '" + End + @"'
                                
                                 select 
								t.EID,t.OTRWName,t.HourAssigned,t.WorkedHour
								,ROUND(CAST((Count(t.WorkedHour)* 100 / (t.HourAssigned))AS float),2) as Percentage
								 from ( select 
                                        Distinct
                                             ED.EID
                                            ,ED.UserName OTRWName
                                            ,CAST(ROUND((SELECT  SUM(ISNULL(EstimatedHrsPerUser,0)) FROM JobAssignToMapping 
                                                      where  AssignTo=ED.EmployeeId AND  DateBooked BETWEEN @JobStartDate AND @JobEndDate and Isdelete=0 group By AssignTo) 
                                             ,2) AS decimal)HourAssigned
                                            ,ISNULL((SELECT   top 1 (CONVERT(varchar, DATEADD(ms, Sum(( DATEPART(hh, UserTimeSheet.TimeSpent) * 3600 ) +( DATEPART(mi, UserTimeSheet.TimeSpent) * 60 ) + DATEPART(ss, UserTimeSheet.TimeSpent))  * 1000, 0), 114)) 
                                                     FROM UserTimeSheet
                                                  INNER JOIN JobAssignToMapping On UserTimeSheet.JobId=JobAssignToMapping.JobId
                                                    where UserId=ED.EmployeeId AND JobDate BETWEEN @JobStartDate AND @JobEndDate   AND JobAssignToMapping.IsDelete=0 group By UserId
                                              ),0) WorkedHour
                                    from EmployeeDetail ED
	                                    INNER JOIN  JobAssignToMapping JAM ON JAM.AssignTo=ED.EmployeeId and DateBooked between @JobStartDate and @JobEndDate	
                                      where ED.EmployeeId IN(" + EmployeesIds + @")    )t group by WorkedHour,HourAssigned,EID,OTRWName	
                                ";
            

            var PaolhReportresult = Context.Database.SqlQuery<PaolhReportViewModel>(sql).AsQueryable();
            return PaolhReportresult;
        }
    }
}
