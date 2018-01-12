using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TimeTracking.Models;
//using Microsoft.AspNet.Identity;
using System.Web.Helpers;

namespace TimeTracking.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index()
        {
            var userId = User.GetUserId();
            var model = await db.Projects.Select(p => new IndexProjectViewModels()
            {
                ID = p.ProjectID,
                Name = p.Name,
                Client = p.Client.ClientName,
                CreationDate = p.CreationDate,
             //   IsOwner = p.UserId == userId,
            }).ToListAsync();

            return View(model);
        }

        public static List<DateTime> Returncurrentweek(DateTime current)
        {
            List<DateTime> result = new List<DateTime>();

            int dayofweek = (int)current.DayOfWeek;

            // move to the previous week
            for (int i = 0; i < 7; i++)
            {
                result.Add(current.AddDays(i - dayofweek));
            }
            return result;
        }

        public async Task<JsonResult> GetDatewiseData(int id, string fromdate, string todate)
        {
            DateTime fdate;
            DateTime tdate;
            if (string.IsNullOrWhiteSpace(fromdate) || string.IsNullOrWhiteSpace(todate)
                || !DateTime.TryParse(fromdate, out fdate) || !DateTime.TryParse(todate, out tdate))
            {
                return Json(new { success = false, statusCode = HttpStatusCode.BadRequest });
            }

            var dates = DateRange(fdate, tdate);
            var userId = User.Identity.Name;//.GetUserId();
            var activities = await db.Activities.Where(p => p.Project.ProjectID == id && //p.Creator.Id == userId &&
            p.ActivityDate >= fdate && p.ActivityDate <= tdate).ToListAsync();/*.Select(a => new DetailsActivityViewModel()
            {
                ActivityId = a.ActivityID,
                Name = a.Name,
                ActivityType = a.ActivityType.Description,
                ActivityStatus = a.ActivityStatus.ToString(),
                Creator = a.Creator.UserName,
                AssignedUser = a.AssignedUser.UserName,
                WorkingTime = a.WorkingTime,
                CreationDate = a.CreationDate,
                ProjectId = a.Project.ProjectID,
                NoOfHours = a.NoOfHours,
                ActivityDate = a.ActivityDate.ToString(),
                CanEdit = a.Project.Owner.Id == userId || a.Creator.Id == userId || a.AssignedUser.Id == userId
            }).OrderBy(x => x.ActivityDate).ToListAsync();*/

            int clientId = 1;
            string holidays = await GetClientHolidays(clientId);
            var publicHolidays = new List<string>();
            if (!string.IsNullOrWhiteSpace(holidays))
            {
                publicHolidays = holidays.Split(',').ToList();
            }
            List<DetailsActivityViewModel> detailsVM = new List<DetailsActivityViewModel>();
            DateTime holidayDate;
            foreach (var date in dates)
            {
                var activitiesPerDate = activities.Where(a => a.ActivityDate.Date == date.Date);

                if (activitiesPerDate == null || !activitiesPerDate.Any())
                {
                    var model = new DetailsActivityViewModel()
                    {
                        ActivityDate = date.ToShortDateString()
                    };

                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        model.isWeekEnd = true;
                    }
                    model.isPublicHoliday = publicHolidays.Any(x => DateTime.TryParse(x, out holidayDate) && holidayDate.Date == date.Date);
                    detailsVM.Add(model);
                }
                else
                {
                    foreach (var item in activitiesPerDate)
                    {
                        var model = new DetailsActivityViewModel()
                        {
                            ActivityId = item.ActivityID,
                            Name = item.Name,
                            //   ActivityType = item.ActivityType.Description,
                            ActivityStatus = item.ActivityStatus.ToString(),
                            // Creator = item.Creator.UserName,
                            // AssignedUser = item.AssignedUser.UserName,
                            WorkingTime = item.WorkingTime,
                            CreationDate = item.CreationDate,
                            //ProjectId = item.Project.ProjectID,
                            NoOfHours = item.NoOfHours,
                            ActivityDate = item.ActivityDate.ToShortDateString(),
                            // CanEdit = item.Project.Owner.Id == userId || item.Creator.Id == userId,//|| item.AssignedUser.Id == userId
                        };

                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            model.isWeekEnd = true;
                        }
                        model.isPublicHoliday = publicHolidays.Any(x => DateTime.TryParse(x, out holidayDate) && holidayDate.Date == date.Date);
                        detailsVM.Add(model);
                    }
                }
            }

            return Json(new { success = true, statusCode = HttpStatusCode.OK, data = detailsVM }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<DateTime> DateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                             .Select(d => fromDate.AddDays(d));
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var userId = User.Identity.Name;//.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currentMonth = DateTime.Now.Month;
            var model = await db.Projects.Where(p => p.ProjectID == id).Select(p => new DetailsProjectViewModel()
            {
                ProjectID = p.ProjectID,
                Name = p.Name,
                //Type = p.ProjectType.Description,
                CreationDate = p.CreationDate,
                //  Owner = p.Owner.UserName,
                ClientId = p.Client.ClientId,
                // Members = p.ApplicationUsers.Select(ap => ap.UserName),
                // IsOwner = p.Owner.Id == userId
            }).FirstOrDefaultAsync();

            int prevYear = DateTime.Now.AddYears(-1).Year;
            model.PublicHolidays = await GetClientHolidays(model.ClientId);
            var activities = await db.Activities.Where(p => p.Project.ProjectID == id && //p.Creator.Id == userId &&
            p.ActivityDate.Month == currentMonth).Select(a => new DetailsActivityViewModel()
            {
                ActivityId = a.ActivityID,
                Name = a.Name,
                ActivityType = a.ActivityType.Description,
                ActivityStatus = a.ActivityStatus.ToString(),
                //  Creator = a.Creator.UserName,
                // AssignedUser = a.AssignedUser.UserName,
                WorkingTime = a.WorkingTime,
                CreationDate = a.CreationDate,
                ProjectId = a.Project.ProjectID,
                NoOfHours = a.NoOfHours,
                ActivityDate = a.ActivityDate.ToString(),
                //  CanEdit = a.Project.Owner.Id == userId || a.Creator.Id == userId || a.AssignedUser.Id == userId
            }).OrderBy(x => x.ActivityDate).ToListAsync();
            if (model == null)
            {
                return HttpNotFound();
            }

            model.Activities = activities;
            return View(model);
        }

        private async Task<string> GetClientHolidays(int clientId)
        {
            int prevYear = DateTime.Now.AddYears(-1).Year;
            var clientHoliday = await db.ClientHolidays.Where(c => c.Client.ClientId == clientId &&
            (c.HolidayYear == DateTime.Now.Year || c.HolidayYear == prevYear)).ToListAsync();
            string publicHolidays = string.Empty;
            if (clientHoliday != null && clientHoliday.Any())
            {
                foreach (var holiday in clientHoliday)
                {
                    publicHolidays += holiday.PublicHolidays + ",";
                }

                if (!string.IsNullOrWhiteSpace(publicHolidays))
                {
                    publicHolidays = publicHolidays.Substring(0, publicHolidays.Length - 1);
                }
            }

            return publicHolidays;
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.Clients = db.Clients.ToList();
            return View();
        }

        /// <param name="model"></param>
        /// <returns></returns>
        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(CreateProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId();
                var client = await db.Clients.FindAsync(model.ClientId);

                var project = new Project()
                {
                    Name = model.Name,
                    Client = client,
                    CreationDate = DateTime.Now,
                    //UserId = userId
                };

                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Clients = db.Clients.ToList();
            return View(model);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var editProject = await db.Projects.Select(p => new EditProjectViewModel()
            {
                ID = p.ProjectID,
                Name = p.Name,
                ClientId = p.Client.ClientId
            }).FirstOrDefaultAsync(ep => ep.ID == id);
            if (editProject == null)
            {
                return HttpNotFound();
            }
            ViewBag.Clients = db.Clients.ToList();
            return View(editProject);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProjectViewModel model)
        {
            if (ModelState.IsValid)
            {

                var project = await db.Projects.FindAsync(model.ID);

                project.Name = model.Name;
                project.Client = await db.Clients.FindAsync(model.ClientId);


                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectTypes = db.ProjectTypes.ToList();
            return View(model);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Members(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var project = await db.Projects.FindAsync(id);

            if (project == null)
            {
                return HttpNotFound();
            }


            /*  if (project.Owner.Id != User.Identity.GetUserId())
              {

                  return new HttpUnauthorizedResult();
              }*/
            var model = await db.Projects.Where(p => p.ProjectID == id).Select(p => new MembersProjectViewModel()
            {
                /* Owner = p.Owner.UserName,
                  Members = p.ApplicationUsers.Select(ap => new MemberViewModel()
                  {
                      ID = ap.Id,
                      Name = ap.UserName

                  }),*/
                IdProject = p.ProjectID
            }).FirstOrDefaultAsync();

            var userNames = new List<string>(){
            model.Owner
            };

            userNames.AddRange(model.Members.Select(s => s.Name));

            //ViewBag.Users = db.Users.Where(u => !userNames.Contains(u.UserName)).ToList();
            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> AddMember(MembersProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = await db.Projects.FindAsync(model.IdProject);

                /*var newMember = await db.Users.FirstAsync(u => u.Id == model.IdMemberAdd);

                project.ApplicationUsers.Add(newMember);*/

                db.SaveChanges();

            }

            return RedirectToAction("Members", new { id = model.IdProject });
        }

        [HttpPost]
        public async Task<ActionResult> RemoveMember(RemoveMemberViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = await db.Projects.FindAsync(model.IdProject);

                /* var thisMember = await db.Users.FirstAsync(u => u.Id == model.IdMemberRemove);

                 project.ApplicationUsers.Remove(thisMember);*/

                db.SaveChanges();

            }

            return RedirectToAction("Members", new { id = model.IdProject });
        }
    }
}
