using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApplicationMVC.Models;
using Microsoft.AspNet.Identity;

namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles ="Admin,Manager")]
    public class CalendarController : Controller
    {
        CalendarService service = null;
        // GET: Calendar
        public ActionResult Index()
        {
            
            return View();
        }
        [HttpGet]
        public JsonResult LoadEvents(int Year,int Month)
        {
            service = new CalendarModel().GetService();
            List<string> eventsList = new List<string>();
            var userId = User.Identity.GetUserId();

            EventsResource.ListRequest request = service.Events.List("primary");
           // request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 100;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            
            // List events.
            Events events = request.Execute();
            events.Items = events.Items.Where(e => e.Description == userId &&( (e.Start.DateTime.Value.Year == Year) &&( e.Start.DateTime.Value.Month ==Month))).ToList();
            
            return Json(events.Items,JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateEvent(string Summary,string ColorId,string Location,string startDate, DateTime startTime, string endDate, DateTime? endTime)
        {
            service = new CalendarModel().GetService();
            Event myEvent = new Event
            {
                Summary = Summary    
            };
            if (String.IsNullOrEmpty(Location))
                myEvent.Location = "";
            else
                myEvent.Location = Location;
            ///
            var arrStart = startDate.Split('-'); 
            DateTime startDateDT = new DateTime(Convert.ToInt32(arrStart[0]),Convert.ToInt32(arrStart[1]),Convert.ToInt32(arrStart[2]),startTime.Hour,startTime.Minute,0);

            EventDateTime evStart = new EventDateTime();
            evStart.DateTime = startDateDT;
            evStart.TimeZone = "Europe/Kiev";
            myEvent.Start = evStart;
            ///
            if(endDate!="" && endTime!=null)
            {
                var arrEnd = startDate.Split('-');

                    
                    DateTime endDateDT = new DateTime(Convert.ToInt32(arrEnd[0]), Convert.ToInt32(arrEnd[1]), Convert.ToInt32(arrEnd[2]),endTime.Value.Hour,endTime.Value.Minute,0);
                EventDateTime evEnd = new EventDateTime();
                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                myEvent.End = evEnd;
            }
            else if(!String.IsNullOrEmpty(endDate) && endTime ==null)
            {
                var arrEnd = endDate.Split('-');


                DateTime endDateDT = new DateTime(Convert.ToInt32(arrEnd[0]), Convert.ToInt32(arrEnd[1]), Convert.ToInt32(arrEnd[2]),startTime.Hour+1,startTime.Minute,0);
                var evEnd = new EventDateTime();
               
                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                myEvent.End = evEnd;
            }
            else if(String.IsNullOrEmpty(endDate) && endTime == null)
            {
                var arrEnd = startDate.Split('-');


                DateTime endDateDT = new DateTime(Convert.ToInt32(arrEnd[0]), Convert.ToInt32(arrEnd[1]), Convert.ToInt32(arrEnd[2]), startTime.Hour + 1, startTime.Minute, 0);
                var evEnd = new EventDateTime();

                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                myEvent.End = evEnd;
            }
            myEvent.Description = User.Identity.GetUserId();
            myEvent.ColorId = ColorId;
            service.Events.Insert(myEvent, "primary").Execute();
            return new RedirectResult("/Calendar/Index");
        }
        public JsonResult GetEvent(string Id)
        {
            service = new CalendarModel().GetService();
            Event ev = service.Events.Get("primary", Id).Execute();


            return Json(ev,JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteEvent(string EventId)
        {
            service = new CalendarModel().GetService();
            service.Events.Delete("primary", EventId).Execute();
            return null;
        }
        public ActionResult EditEvent(string EventId, string Summary, string ColorId, string Location, DateTime startDate, DateTime startTime, DateTime? endDate, DateTime? endTime)
        {
            service = new CalendarModel().GetService();
            var Event  = service.Events.Get("primary", EventId).Execute();

            Event.Summary = Summary;

            if (String.IsNullOrEmpty(Location))
                Event.Location = "";
            else
                Event.Location = Location;
            ///




            EventDateTime eventDateTime = new EventDateTime();
            TimeSpan time = new TimeSpan(startTime.Hour, startTime.Minute, 0);
            startDate = startDate.Add(time);
            eventDateTime.DateTime = startDate;
            eventDateTime.TimeZone = "Europe/Kiev";
            Event.Start = eventDateTime;
            ///
            if (endDate != null && endTime != null)
            {
                DateTime endDateDT = endDate.Value.Add(new TimeSpan( endTime.Value.Hour, endTime.Value.Minute, 0));
                EventDateTime evEnd = new EventDateTime();
                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                Event.End = evEnd;
            }
            else if (endDate != null && endTime == null)
            {
                DateTime endDateDT = endDate.Value.Add(new TimeSpan(startTime.Hour+1, startTime.Minute, 0));
                var evEnd = new EventDateTime();

                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                Event.End = evEnd;
            }
            else if (endDate == null && endTime == null)
            {


                DateTime endDateDT = startDate.AddHours(1);
                var evEnd = new EventDateTime();

                evEnd.DateTime = endDateDT;
                evEnd.TimeZone = "Europe/Kiev";
                Event.End = evEnd;
            }

            Event.ColorId = ColorId;
            service.Events.Update(Event,"primary",EventId).Execute();
            return new RedirectResult("/Calendar/Index");
        }
    }
}