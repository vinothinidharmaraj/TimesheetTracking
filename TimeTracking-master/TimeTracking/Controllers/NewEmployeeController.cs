using System.Web.Mvc;

namespace TimeTracking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewEmployeeController : Controller
    {
        // GET: NewEmployee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateUser()
        {
            return View();
        }
    }
}