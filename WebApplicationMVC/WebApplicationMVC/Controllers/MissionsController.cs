using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC.Models;
using Microsoft.AspNet.Identity;
namespace WebApplicationMVC.Controllers
{
    [Authorize]
    public class MissionsController : Controller
    {
        // GET: Missions

        public ActionResult IndexUnCompleted()
        {
                var UserId = User.Identity.GetUserId();
                var db = new ApplicationDbContext();
                bool IsUserAdmin = User.IsInRole("Admin");
                ViewBag.IsUserAdmin = IsUserAdmin;

                var dateNow = DateTime.Now;
                List<Mission> resultPrev;
                if (!IsUserAdmin)
                    resultPrev = db.Missions.Where(e => e.UserId == UserId && (e.DateTime >= dateNow)).OrderBy(e => e.DateTime).ToList();
                else
                    resultPrev = db.Missions.Where(e => e.DateTime >= dateNow).OrderBy(e => e.DateTime).ToList();

                var result = resultPrev.ToList();
                var ListPreview = new List<string>();
                var ListColors = new List<string>();
                var ListLostDays = new List<string>();
                var ListUsers = new List<string>();
                foreach (var i in resultPrev)
                {
                    
                    var IsDone = db.TaskElements.Where(e => e.MissionId == i.Id && e.Status == false).Count();
                    if(IsDone==0)
                    {
                        result.Remove(i);
                        continue;
                    }
                    else
                    {
                        var tmp = db.TaskElements.Where(e => e.MissionId == i.Id).FirstOrDefault().Content + " ...";
                        ListPreview.Add(tmp);
                        var UserFull = db.Users.Where(e => e.Id == i.UserId).FirstOrDefault();
                        string UserFullName = $"{UserFull.LastName} {UserFull.FirstName} {UserFull.MiddleName}";
                        ListUsers.Add(UserFullName);
                        var date = i.DateTime;
                        var diff = date.Subtract(dateNow);
                        int countOfDays = diff.Days + 1;
                        if (countOfDays <= 0)
                            ListLostDays.Add("0 днів");
                        else
                        {
                            string s = countOfDays + "";
                            char number = s[s.Length - 1];
                            if (number == '1')
                                ListLostDays.Add($"{s} день");
                            else if (number >= '2' && number <= '4')
                                ListLostDays.Add($"{s} дня ");
                            else
                                ListLostDays.Add($"{s} днів ");
                        }
                        ListColors.Add("timenull");
                    }
                }
                ViewBag.Preview = ListPreview;
                ViewBag.Colors = ListColors;
                ViewBag.LostDays = ListLostDays;
                ViewBag.ListUsers = ListUsers;
                return View("Index",result);
        }
        public ActionResult Index()
            {
                var UserId = User.Identity.GetUserId();
                var db = new ApplicationDbContext();
                bool IsUserAdmin = User.IsInRole("Admin");
                ViewBag.IsUserAdmin = IsUserAdmin;
                var dateNow = DateTime.Now;
                List<Mission> resultPrev;
                if(!IsUserAdmin)
                   resultPrev = db.Missions.Where(e => e.UserId == UserId && (e.DateTime>=dateNow)).OrderBy(e=>e.DateTime).ToList();
                else
                    resultPrev = db.Missions.Where(e =>e.DateTime >= dateNow).OrderBy(e => e.DateTime).ToList();
                var result = resultPrev.ToList();
                if (IsUserAdmin)
                    result.AddRange(db.Missions.Where(e => e.DateTime < dateNow));
                else
                    result.AddRange(db.Missions.Where(e =>e.UserId==UserId && e.DateTime < dateNow));

                var ListPreview = new List<string>();
                var ListColors = new List<string>();
                var ListLostDays = new List<string>();
                var ListUsers = new List<string>();

                foreach (var i in result)
                {
                    var tmp = db.TaskElements.Where(e => e.MissionId == i.Id).FirstOrDefault().Content + " ...";
                    ListPreview.Add(tmp);
                    var UserFull = db.Users.Where(e => e.Id == i.UserId).FirstOrDefault();
                    string UserFullName = $"{UserFull.LastName} {UserFull.FirstName} {UserFull.MiddleName}";
                    ListUsers.Add(UserFullName);
                    var date = i.DateTime;
                    var diff = date.Subtract(dateNow);
                    int countOfDays = diff.Days+1;
                    if (countOfDays <= 0)
                        ListLostDays.Add("0 днів");
                    else
                    {
                        string s = countOfDays+"";
                        char number = s[s.Length - 1];
                        if (number == '1')
                            ListLostDays.Add($"{s} день");
                        else if (number>='2' && number<='4')
                            ListLostDays.Add($"{s} дня ");
                        else
                            ListLostDays.Add($"{s} днів ");
                    }

                    var IsDone = db.TaskElements.Where(e => e.MissionId == i.Id && e.Status == false).Count();
                    if(IsDone==0)
                        ListColors.Add("ready");
                    else if (countOfDays <= 2)
                        ListColors.Add("timenull");
                    else
                        ListColors.Add("notready");
                }
            ViewBag.Preview = ListPreview;
            ViewBag.Colors = ListColors;
            ViewBag.LostDays = ListLostDays;
            ViewBag.ListUsers = ListUsers;
            ViewBag.Users = db.Users.ToList();
            return View(result);
        }
        public ActionResult Create(DateTime date,DateTime? time,string UserId,List<string> Element)
        {
            Mission miss = new Mission();           
            if(time != null)
            {
                TimeSpan ts = new TimeSpan(time.Value.Hour, time.Value.Minute, 0);
                date = date.Date + ts;

            }
            miss.DateTime = date;
            if (String.IsNullOrEmpty(UserId))
                miss.UserId = User.Identity.GetUserId();
            else
                miss.UserId = UserId;
            var db = new ApplicationDbContext();
            db.Missions.Add(miss);
            db.SaveChanges();
            for(var i = 0;i<Element.Count;i++)
            {
                TaskElement t = new TaskElement();
                t.Content = Element[i];
                t.MissionId = miss.Id;
                t.Status = false;
                db.TaskElements.Add(t);
            }
            db.SaveChanges();
            return new RedirectResult("/Missions/Index");
        }
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var db = new ApplicationDbContext();
            var range = db.TaskElements.Where(e => e.MissionId == Id);

            db.TaskElements.RemoveRange(range);
            var mission = db.Missions.Where(e => e.Id == Id).FirstOrDefault();
            db.Missions.Remove(mission);
            db.SaveChanges();
            return null;
        }
        [HttpGet]
        public JsonResult GetInfo(int Id)
        {
            var db = new ApplicationDbContext();
            var result = db.TaskElements.Where(e => e.MissionId == Id).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(List<string> Status,List<string> Element,int TaskId,DateTime date,DateTime? time)
        {
            var db =  new ApplicationDbContext();
            Mission miss = db.Missions.Where(e=>e.Id == TaskId).FirstOrDefault();
            if (time != null)
            {
                TimeSpan ts = new TimeSpan(time.Value.Hour, time.Value.Minute, 0);
                date = date.Date + ts;
            }
            miss.DateTime = date;
            db.SaveChanges();
            var removeElements = db.TaskElements.Where(e => e.MissionId == TaskId).ToList();
            db.TaskElements.RemoveRange(removeElements);
            for (var i = 0; i < Element.Count; i++)
            {
                TaskElement t = new TaskElement();
                t.Content = Element[i];
                t.MissionId =  TaskId;
                t.Status = Convert.ToBoolean(Status[i]);
                
                db.TaskElements.Add(t);
            }
            db.SaveChanges();
            return new RedirectResult("/Missions/Index");
        }
    }
}