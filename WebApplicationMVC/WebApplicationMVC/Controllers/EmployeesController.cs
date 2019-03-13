using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        ApplicationDbContext db;
        UserStore<ApplicationUser> userStore;
        UserManager<ApplicationUser> userManager;
        public EmployeesController()
        {
            db = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(db);
            userManager = new UserManager<ApplicationUser>(userStore);
        }
        public ActionResult Index()
        {           
            string userId = User.Identity.GetUserId();

            var managersId = db.Roles.Where(e=>e.Name=="Manager").FirstOrDefault().Users.Select(e=>e.UserId);
            var managers = db.Users.Where(e => managersId.Contains(e.Id)).AsEnumerable();
            var counterpatriesId = db.Roles.Where(e => e.Name == "Counterparty").FirstOrDefault().Users.Select(e => e.UserId);
            var counterpatries = db.Users.Where(e => counterpatriesId.Contains(e.Id)).AsEnumerable();
            EmployeesViewModel model = new EmployeesViewModel { Managers = managers, Counterparties = counterpatries};
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(string Id,string LastName,string FirstName,string MiddleName,string Email,string Tel)
        {
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
        public ActionResult EditCounterparty(EditCounterpartyModel model)
        {
            var changingUser = db.Users.Where(e => String.Equals(e.Id, model.Id)).FirstOrDefault();
            if (changingUser != null)
            {
                changingUser.FirstName =model.FirstName;
                changingUser.LastName = model.LastName;
                changingUser.MiddleName = model.MiddleName;
                changingUser.Email = model.Email;
                changingUser.Tel = model.Tel;

                var newPassword = userManager.PasswordHasher.HashPassword(model.ConfirmPassword);
                if (changingUser.PasswordHash!=newPassword)
                {                  
                    changingUser.PasswordHash = newPassword;
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ContentResult> DeleteUser(string Id)
        {
            var User = db.Users.Where(e => String.Equals(e.Id, Id)).FirstOrDefault();
            User.EmailConfirmed = false;
            string fullName = $"{User.FirstName} {User.LastName}";
            await db.SaveChangesAsync();
            return Content(fullName);
        }
    }
}