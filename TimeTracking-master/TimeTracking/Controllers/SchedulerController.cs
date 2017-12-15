using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Scheduler;
using DayPilot.Web.Mvc.Recurrence;
using TimeTracking.Models;

namespace TimeTracking.Controllers
{

    public class SchedulerController : Controller
    {
        //
        // GET: /Scheduler/

        public ActionResult Backend()
        {
            return new Dps().CallBack(this);
        }

        class Dps : DayPilotScheduler
        {
            //TimesheetDataContext dc = new TimesheetDataContext();

            protected override void OnInit(InitArgs e)
            {
                StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                Days = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
                UpdateWithMessage("Welcome!", CallBackUpdateType.Full);
            }

            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "refresh":
                        Update(CallBackUpdateType.Full);
                        break;
                    case "businessOnly":
                        ShowNonBusiness = false;
                        Update(CallBackUpdateType.Full);
                        break;
                    case "fullDay":
                        ShowNonBusiness = true;
                        Update(CallBackUpdateType.Full);
                        break;
                }
            }



            protected override void OnBeforeEventRender(BeforeEventRenderArgs e)
            {
                e.Html = (e.End - e.Start).ToString("hh\\:mm") + " - " + e.Text;
            }


        }

    }
}
