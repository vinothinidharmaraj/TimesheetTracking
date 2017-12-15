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

namespace TimeTracking.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();


            var model = await db.Projects.Where(p => p.Owner.Id == userId || p.ApplicationUsers.Any(m => m.Id == userId))
                                                .Select(p => new IndexProjectViewModels()
                                                {
                                                    ID = p.ProjectID,
                                                    Name = p.Name,
                                                    Type = p.ProjectType.Description,
                                                    CreationDate = p.CreationDate,
                                                    IsOwner = p.Owner.Id == userId,
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

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //List<keyvaluePair> mycurrentActivities = new List<keyvaluePair>();
        //    var activityNames = db.Activities.Where(p => p.Project.ProjectID == id).ToDictionary(x => x.ActivityID, x => x.Name);

    //   var data = await  db.Activities.Join(activity => activity.ActivityID, timedat => timedat.ActivityId,)
    //        var td = await (from s in db.Activities
    //join r in db.Timedata on s.ActivityID equals r.ActivityId
    //where s.Project.ProjectID == id
    //select new { s, r }).ToListAsync();

          //  var timesheet = db.Timedata.Where(t => activityNames.ContainsKey(t.ActivityId));
            var model = await db.Projects.Where(p => p.ProjectID == id).Select(p => new DetailsProjectViewModel()
            {
                ProjectID = p.ProjectID,
                Name = p.Name,
                Type = p.ProjectType.Description,
                CreationDate = p.CreationDate,
                Owner = p.Owner.UserName,
                Members = p.ApplicationUsers.Select(ap => ap.UserName),
                IsOwner = p.Owner.Id == userId
            }).FirstOrDefaultAsync();

            if (model == null)
            {
                return HttpNotFound();
            }

          //  model.Activities = activityNames;
            ViewBag.dates = new DatesClass()
            {
                weekDates = Returncurrentweek(DateTime.Today)
            };
            return View(model);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.ProjectTypes = db.ProjectTypes.ToList();
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
                var userId = User.Identity.GetUserId();
                var owner = await db.Users.FirstAsync(u => u.Id == userId);
                var projectType = await db.ProjectTypes.FindAsync(model.TypeID);

                var project = new Project()
                {
                    Name = model.Name,
                    ProjectType = projectType,
                    CreationDate = DateTime.Now,
                    Owner = owner

                };

                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectTypes = db.ProjectTypes.ToList();
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
                TypeID = p.ProjectType.ProjectTypeID
            }).FirstOrDefaultAsync(ep => ep.ID == id);
            if (editProject == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectTypes = db.ProjectTypes.ToList();
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
                project.ProjectType = await db.ProjectTypes.FindAsync(model.TypeID);


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


            if (project.Owner.Id != User.Identity.GetUserId())
            {

                return new HttpUnauthorizedResult();
            }
            var model = await db.Projects.Where(p => p.ProjectID == id).Select(p => new MembersProjectViewModel()
            {
                Owner = p.Owner.UserName,
                Members = p.ApplicationUsers.Select(ap => new MemberViewModel()
                {
                    ID = ap.Id,
                    Name = ap.UserName

                }),
                IdProject = p.ProjectID
            }).FirstOrDefaultAsync();

            var userNames = new List<string>(){
            model.Owner
            };

            userNames.AddRange(model.Members.Select(s => s.Name));

            ViewBag.Users = db.Users.Where(u => !userNames.Contains(u.UserName)).ToList();
            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> AddMember(MembersProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = await db.Projects.FindAsync(model.IdProject);

                var newMember = await db.Users.FirstAsync(u => u.Id == model.IdMemberAdd);

                project.ApplicationUsers.Add(newMember);

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

                var thisMember = await db.Users.FirstAsync(u => u.Id == model.IdMemberRemove);

                project.ApplicationUsers.Remove(thisMember);

                db.SaveChanges();

            }

            return RedirectToAction("Members", new { id = model.IdProject });
        }
    }
}
