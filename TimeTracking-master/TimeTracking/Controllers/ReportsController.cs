using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models;
using System.Data.Entity;
using Humanizer;
namespace TimeTracking.Controllers
{
    public class ReportsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reports


        public async Task<ActionResult> ProjectTotalHours()
        {
            var model = (await db.Projects.SelectMany(p => p.Activities).Finished().GroupBy(p => p.Project.Name).Select(p => new
                                                   {
                                                       Name = p.Key,
                                                       WorkTimes = p.Select(a => a.WorkingTime)
                                                   }).ToListAsync());

            var m2 = model.Select(a => new ProjectTotalHoursViewModel()
                                                {
                                                    Name = a.Name,
                                                    TotalHours = a.WorkTimes.Sum(w => w.TotalHours)
                                                }).OrderByDescending(p => p.TotalHours).ToList();
            return View(m2);

        }

        public async Task<ActionResult> ProjectTotalHoursRemaining()
        {
            var model = (await db.Projects.SelectMany(p=>p.Activities).Doing().GroupBy(p=>p.Project.Name).Select(p => new
            {
                Name = p.Key,
                WorkingTimes = p.Select(a => a.WorkingTime)
                
            }).ToListAsync());

            var m2 = model.Select(a => new ProjectTotalHoursViewModel()
            {
                Name = a.Name,
                TotalHours = a.WorkingTimes.Sum(w => w.TotalHours)
            }).OrderByDescending(p => p.TotalHours).ToList();
            return View(m2);
        }


        public ActionResult ProjectTotalHoursPerPeriod()
        {
            return View(new SetPeriodViewModel
            {

                FinalDate = DateTime.Now,
                InitialDate = DateTime.Now.AddDays(-7)

            });

        }


        [HttpPost]
        public async Task<ActionResult> ProjectTotalHoursPerPeriod(SetPeriodViewModel model)
        {

            var m = (await db.Projects.SelectMany(p=>p.Activities).Finished().OnInterval(model.FinalDate, model.InitialDate).GroupBy(p => p.Project.Name).Select(p => new
            {
                Name = p.Key,
                WorkTimes = p.Select(a1 => a1.WorkingTime)
            }).ToListAsync());

            var result = m.Where(x => x.WorkTimes.Count() > 0).Select(a => new ProjectTotalHoursViewModel()
            {
                Name = a.Name,
                TotalHours = a.WorkTimes.Sum(t => t.TotalHours)
            }).ToList();

            model.ResultProjects = result;

            return View(model);


        }


        public async Task<ActionResult> UserTotalHours()
        {
            var model = (await db.Projects.SelectMany(p => p.Activities)
                                                    .Finished()
                                                    .GroupBy(a => a.AssignedUser)
                                                    .Select(ag => new
                                                    {
                                                        WorkingTimes = ag.Select(a1 => a1.WorkingTime),
                                                        UserName = ag.Key.UserName
                                                    })
                                            .ToListAsync());

            var m2 = model.Select(u => new UserTotalHoursViewModel()
                                                {
                                                    Name = u.UserName,
                                                    TotalHours = u.WorkingTimes.Sum(w => w.TotalHours)
                                                }).OrderByDescending(u => u.TotalHours).ToList();
            return View(m2);
        }

        public async Task<ActionResult> UserTotalHoursRemaining()
        {
            var model = (await db.Projects.SelectMany(p => p.Activities)
                                                    .Doing()
                                                    .GroupBy(a => a.AssignedUser)
                                                    .Select(ag => new
                                                    {
                                                        WorkingTimes = ag.Select(a1 => a1.WorkingTime),
                                                        UserName = ag.Key.UserName
                                                    })
                                            .ToListAsync());

            var m2 = model.Select(u => new UserTotalHoursViewModel()
            {
                Name = u.UserName,
                TotalHours = u.WorkingTimes.Sum(w => w.TotalHours)
            }).OrderByDescending(u => u.TotalHours).ToList();
            return View(m2);
        }



        public ActionResult UserProductivityPerPeriod()
        {
            return View(new SetPeriodViewModel
            {

                FinalDate = DateTime.Now,
                InitialDate = DateTime.Now.AddDays(-7)

            });

        }

        [HttpPost]
        public async Task<ActionResult> UserProductivityPerPeriod(SetPeriodViewModel model)
        {
            var m = (await db.Projects.SelectMany(p => p.Activities).Finished().OnInterval(model.FinalDate, model.InitialDate).GroupBy(a => a.AssignedUser)
                                                    .Select(ag => new
                                                    {
                                                        WorkingTimes = ag.Select(a1 => a1.WorkingTime),
                                                        UserName = ag.Key.UserName
                                                    })
                                            .ToListAsync());


            model.ResultUsers = m.Select(u => new UserTotalHoursViewModel()
            {
                Name = u.UserName,
                TotalHours = u.WorkingTimes.Sum(w => w.TotalHours)
            }).OrderByDescending(u => u.TotalHours).ToList();



            return View(model);


        }

    }




}