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
using Microsoft.AspNet.Identity;
using TimeTracking.Core.Models;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Scheduler;
using DayPilot.Web.Mvc.Recurrence;

namespace TimeTracking.Controllers
{
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index(int id)
        {
            var userId = User.Identity.GetUserId();
            var model = await db.Projects.Where(p => p.ProjectID == id)
                                                 .SelectMany(p => p.Activities)
                                                 .Where(a => a.ActivityStatus != ActivityStatus.Deleted)
                                                 .Select(a => new IndexActivityViewModel()
                                                 {
                                                     ActivityId = a.ActivityID,
                                                     Name = a.Name,
                                                     AssignedUser = a.AssignedUser.UserName,
                                                     ActivityStatus = a.ActivityStatus.ToString(),
                                                     Creator = a.Creator.UserName,
                                                     WorkingTime = a.WorkingTime,
                                                     Type = a.ActivityType.Description,
                                                     CreationDate = a.CreationDate,
                                                     //tem que verificar se o usuário logado é o gerente do projeto ou o criador da atividade ou o assignedUser
                                                     CanEdit = a.Project.Owner.Id == userId || a.Creator.Id == userId || a.AssignedUser.Id == userId

                                                 })
                                          .ToListAsync();

            ViewBag.ProjectId = id;

            return PartialView("ListActivities", model);
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var model = await db.Activities.Where(a => a.ActivityID == id
                && a.ActivityStatus != ActivityStatus.Deleted).
                Select(a => new DetailsActivityViewModel()
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
                    CanEdit = a.Project.Owner.Id == userId || a.Creator.Id == userId || a.AssignedUser.Id == userId
                }).FirstOrDefaultAsync();

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }



        // GET: Activity/Create/IdProject
        public ActionResult Create(int id)
        {
            if (id <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.ActivityTypes = db.ActivityTypes.ToList();
            ViewBag.MemberList = db.Projects.Where(p => p.ProjectID == id).SelectMany(p => p.ApplicationUsers).ToList();

            var model = new CreateActivityViewModel
            {
                ProjectId = id
            };

            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create(CreateActivityViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = null;


                var activity = new Activity
                {
                    Name = model.Name,
                    ActivityType = await db.ActivityTypes.FindAsync(model.ActivityType),
                    CreationDate = DateTime.Now,
                    AssignedUser = user,
                    Project = await db.Projects.FirstAsync(p => p.ProjectID == model.ProjectId),
                    WorkingTime = model.WorkingTime,
                    Creator = await db.Users.FirstAsync(u => u.Id == userId),
                    ActivityStatus = Core.Models.ActivityStatus.Created
                };

                db.Activities.Add(activity);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = model.ProjectId });
            }

            ViewBag.ActivityTypes = db.ActivityTypes.ToList();
            ViewBag.MemberList = db.Projects.Where(p => p.ProjectID == model.ProjectId).SelectMany(p => p.ApplicationUsers).ToList();
            return View(model);
        }



        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> CreateTimesheetEntry(List<TimesheetViewModel> models = null)
        {

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = null;

                foreach (var model in models)
                {
                    var timesheetData = new TimesheetData
                    {
                        ActivityId = model.ActivityId,
                        TimeSheetDate = model.TimeSheetDate,
                        NoOfHours = model.NoOfHours,
                        ProjectId = model.ProjectId

                    };
                    db.Timedata.Add(timesheetData);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = models[0].ProjectId });
            }

            ViewBag.ActivityTypes = db.ActivityTypes.ToList();
            ViewBag.MemberList = db.Projects.Where(p => p.ProjectID == models[0].ProjectId).SelectMany(p => p.ApplicationUsers).ToList();
            return View(models);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var editActivity = await db.Activities.Select(a => new EditActivityViewModel()
            {
                ActivityId = a.ActivityID,
                Status = a.ActivityStatus.ToString(),
                ActivityType = a.ActivityType.ActivityTypeID,
                ProjectId = a.Project.ProjectID,
                AssignedUserId = a.AssignedUser.UserName

            }).FirstOrDefaultAsync(ea => ea.ActivityId == id);
            if (editActivity == null)
            {
                return HttpNotFound();
            }

            ViewBag.ActivityStatusList = Enum.GetValues(typeof(ActivityStatus)).Cast<ActivityStatus>().
                                         Select(a => new { Text = a.ToString(), Value = ((int)a).ToString() });
            ViewBag.ActivityTypes = await db.ActivityTypes.ToListAsync();
            ViewBag.Users = await db.Activities.Where(a => a.ActivityID == id).
                                                SelectMany(a => a.Project.ApplicationUsers.
                                                Select(u => new { Text = u.UserName, Value = u.Id }))
                                                .ToListAsync();


            return View(editActivity);
        }
        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditActivityViewModel model)
        {
            var activity = await db.Activities.FirstAsync(a => a.ActivityID == model.ActivityId);
            var newStatus = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), model.Status);
            DateTime? finalizationDate = null;

            if (newStatus == ActivityStatus.Finished)
            {
                finalizationDate = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                activity.ActivityType = await db.ActivityTypes.FindAsync(model.ActivityType);
                activity.ActivityStatus = newStatus;
                activity.AssignedUser = await db.Users.FirstAsync(u => u.Id == model.AssignedUserId);
                activity.FinalizationDate = finalizationDate;

                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = activity.Project.ProjectID });
            }


            ViewBag.ActivityStatusList = Enum.GetValues(typeof(ActivityStatus)).Cast<ActivityStatus>().
                                         Select(a => new { Text = a.ToString(), Value = ((int)a).ToString() });
            ViewBag.ActivityTypes = db.ActivityTypes.ToList();
            ViewBag.Users = await db.Activities.Where(a => a.ActivityID == model.ActivityId).
                                               SelectMany(a => a.Project.ApplicationUsers.
                                               Select(u => new { Text = u.UserName, Value = u.Id }))
                                               .ToListAsync();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Activities/ListComments/ActivityId
        public async Task<ActionResult> ListComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var activity = await db.Activities.FindAsync(id);

            if (activity == null)
            {
                return HttpNotFound();
            }

            var model = await db.Activities.Where(a => a.ActivityID == id).SelectMany(a => a.Comments).Select(c => new CommentsViewModel()
            {
                CommentId = c.CommentId,
                CommentOwner = c.CommentOwner.UserName,
                CreationDate = c.CreationDate,
                Description = c.Description
            }).ToListAsync();

            ViewBag.ActivityId = id;
            return PartialView(model);

        }

        // GET: Comment/AddComment/ActivityId
        public ActionResult AddComment(int id)
        {
            if (id <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);



            var model = new CreateCommentsViewModel
            {
                ActivityId = id
            };

            return View(model);
        }



        [HttpPost]
        public async Task<ActionResult> AddComment(CreateCommentsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                var activity = await db.Activities.FindAsync(model.ActivityId);

                var newComment = new Comment()
                {

                    Description = model.Description,
                    CommentOwner = await db.Users.FirstAsync(u => u.Id == userId),
                    CreationDate = DateTime.Now
                };
                activity.Comments.Add(newComment);

                db.SaveChanges();

            }
            return RedirectToAction("Details", new { id = model.ActivityId });

        }


    }
}
