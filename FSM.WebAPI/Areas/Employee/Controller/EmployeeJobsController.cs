using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FSM.Core.Entities;
using FSM.Infrastructure;
using log4net;
using FSM.WebAPI.Models;
using FSM.WebAPI.Common;

namespace FSM.WebAPI.Areas.Employee.Controller
{
    [Authorize]
    public class EmployeeJobsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FsmContext db = new FsmContext();

        // GET: api/EmployeeJobs
        public IQueryable<Jobs> GetEmployeeJobs()
        {
            return db.Jobs;
        }

        // GET: api/EmployeeJobs/5
        [ResponseType(typeof(Jobs))]
        public IHttpActionResult GetEmployeeJobs(Guid id)
        {
            Jobs employeeJobs = db.Jobs.Where(m => m.Id == id && m.IsDelete == false).FirstOrDefault();
            if (employeeJobs == null)
            {
                return NotFound();
            }


            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " get employee job by jobid.");

            return Ok(employeeJobs);
        }

        // PUT: api/EmployeeJobs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployeeJobs(Guid id, Jobs employeeJobs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employeeJobs.Id)
            {
                return BadRequest();
            }
            employeeJobs.IsDelete = false;
            db.Entry(employeeJobs).State = EntityState.Modified;

            try
            {
                db.SaveChanges();

                ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
                response = CommonFunctions.GetUserInfoByToken();
                string userId = response.ResponseList[0].UserId;
                string userName = response.ResponseList[0].UserName;


                log4net.ThreadContext.Properties["UserId"] = userId;
                log.Info(userName + " put employee job.");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeJobsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EmployeeJobs
        [ResponseType(typeof(Jobs))]
        public IHttpActionResult PostEmployeeJobs(Jobs employeeJobs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            employeeJobs.IsDelete = false;
            db.Jobs.Add(employeeJobs);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EmployeeJobsExists(employeeJobs.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = employeeJobs.Id }, employeeJobs);
        }

        // DELETE: api/EmployeeJobs/5
        [ResponseType(typeof(Jobs))]
        public IHttpActionResult DeleteEmployeeJobs(Guid id)
        {
            Jobs employeeJobs = db.Jobs.Find(id);
            if (employeeJobs == null)
            {
                return NotFound();
            }
            employeeJobs.IsDelete = true;
            db.Entry(employeeJobs).State = EntityState.Modified;
            db.SaveChanges();

            ServiceResponse<UserInfoViewModel> response = new ServiceResponse<UserInfoViewModel>();
            response = CommonFunctions.GetUserInfoByToken();
            string userId = response.ResponseList[0].UserId;
            string userName = response.ResponseList[0].UserName;


            log4net.ThreadContext.Properties["UserId"] = userId;
            log.Info(userName + " delete employee job.");

            return Ok(employeeJobs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool EmployeeJobsExists(Guid id)
        {
            return db.Jobs.Count(e => e.Id == id && e.IsDelete == false) > 0;
        }
    }
}