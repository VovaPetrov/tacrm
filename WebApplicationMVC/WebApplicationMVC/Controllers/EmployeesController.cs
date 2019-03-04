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
    public class EmployeesController : Controller
    {
        // GET: Employees
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();
            var model = db.Users.Where(e => e.Id!=userId && e.EmailConfirmed).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(string Id,string LastName,string FirstName,string MiddleName,string Email,string Tel)
        {
            var db = new ApplicationDbContext();
            var User = db.Users.Where(e => String.Equals(e.Id,Id)).FirstOrDefault();
            if (User != null)
            {
                User.FirstName = FirstName;
                User.LastName = LastName;
                User.MiddleName = MiddleName;
                User.Email = Email;
                User.Tel = Tel;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ContentResult DeleteUser(string Id)
        {
            var db = new ApplicationDbContext();
            var User = db.Users.Where(e => String.Equals(e.Id, Id)).FirstOrDefault();
            User.EmailConfirmed = false;
            string fullName = $"{User.FirstName} {User.LastName}";
            db.SaveChanges();
            return Content(fullName);
        }
    }
}