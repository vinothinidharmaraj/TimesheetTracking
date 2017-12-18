using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Web.Mvc;
using TimeTracking.Models;

namespace TimeTracking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db;// = new ApplicationDbContext();


        public ActionResult Index()
        {
            using (db = new ApplicationDbContext())
            {
                var roles = db.Roles.ToList();
                return View(roles);
            }
        }

        // GET: Roles
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            using (db = new ApplicationDbContext())
            {
                try
                {
                    db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                    {
                        Name = collection["RoleName"]
                    });
                    db.SaveChanges();
                    ViewBag.ResultMessage = "Role created successfully !";
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
        }


        public ActionResult ManageUserRoles()
        {
            // prepopulat roles for the view dropdown
            using (db = new ApplicationDbContext())
            {
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
                                            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            using (db = new ApplicationDbContext())
            {
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //var account = new AccountController();
                //account.UserManager.AddToRole(user.Id, RoleName);
                var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var idResult = usermanager.AddToRole(user.Id, RoleName);

                if (idResult.Succeeded)
                {
                    ViewBag.ResultMessage = "Role created successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "Role Not Created, Check and Retry !";
                }
                    // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View("ManageUserRoles");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            using (db = new ApplicationDbContext())
            {
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    //var account = new AccountController();
                    //ViewBag.RolesForThisUser = account.UserManager.GetRoles(user.Id);
                    if (user != null)
                    {
                        var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                        ViewBag.RolesForThisUser = usermanager.GetRoles(user.Id);   
                    }
                    else
                    {
                        string[] userDoesnotExists = new string[] { "User with UserName:" + UserName + " doesn't exists"};
                        ViewBag.RolesForThisUser = userDoesnotExists;
                    }

                    // prepopulat roles for the view dropdown
                    var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                    ViewBag.Roles = list;
                }
                return View("ManageUserRoles");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            using (db = new ApplicationDbContext())
            {
                //var account = new AccountController();
                var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (usermanager.IsInRole(user.Id, RoleName))
                {
                    usermanager.RemoveFromRole(user.Id, RoleName);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                // prepopulat roles for the view dropdown
                var list = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;

                return View("ManageUserRoles");
            }
        }
    }
}