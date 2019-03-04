using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC.Models;
using PagedList.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;

namespace WebApplicationMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Page = "Головна";
            return View();
        }
        public ActionResult Contacts(int? page)
        {
            ViewBag.Page = "Контакти";
            var contactList = new ApplicationDbContext().Contacts.OrderBy(e=>e.LastName);
            contactList.Reverse();
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(contactList.ToPagedList(pageNumber, pageSize));

        }
        [HttpPost]
        public ActionResult Contacts(Contact contact)
        { 
            var db = new ApplicationDbContext();
            db.Contacts.Add(contact);
            db.SaveChanges();
            var contactList = db.Contacts.ToList();
            contactList.Reverse();
            int pageSize = 6;
            int pageNumber = 1;
            return View(contactList.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult EditContact(Contact contact)
        {
            var db = new ApplicationDbContext();
            var cont = db.Contacts.Where(e => e.Id == contact.Id).FirstOrDefault();

            cont.LastName = contact.LastName;
            cont.FirstName = contact.FirstName;
            cont.MiddleName = contact.MiddleName;
            cont.Company = contact.Company;
            cont.Position = contact.Position;
            cont.Tel = contact.Tel;
            cont.Email = contact.Email;
            cont.Comments = contact.Comments;
            db.SaveChanges();

            return new RedirectResult("/Home/Contacts");
        }
        [HttpPost]
        public ActionResult DelContact(int Id)
        {
            var db = new ApplicationDbContext();

            var contact = db.Contacts.Where(e => e.Id == Id).FirstOrDefault();

            db.Contacts.Remove(contact);
            db.SaveChanges();

           
           return new RedirectResult("/Home/Contacts");
        }
        [HttpPost]
        public ActionResult SearchContact(string query)
        {
            var db = new ApplicationDbContext();

            var first = db.Contacts.Where(e => e.FirstName.Contains(query)).ToList();
            var last = db.Contacts.Where(e => e.LastName.Contains(query)).ToList();
            var middle = db.Contacts.Where(e => e.MiddleName.Contains(query)).ToList();
            var email = db.Contacts.Where(e => e.Email.Contains(query)).ToList();
            var tel = db.Contacts.Where(e => e.Tel.Contains(query)).ToList();
            var company = db.Contacts.Where(e => e.Company.Contains(query)).ToList();
            var position = db.Contacts.Where(e=>e.Position.Contains(query)).ToList();
            var comments = db.Contacts.Where(e => e.Comments.Contains(query)).ToList();
            var contactList = first;
            contactList.AddRange(last);
            contactList.AddRange(middle);
            contactList.AddRange(email);
            contactList.AddRange(tel);
            contactList.AddRange(company);
            contactList.AddRange(position);
            var result = contactList.Distinct();
            ViewBag.SearchName = query;
            return View(result.ToList());
        }
        public ActionResult Messages()
        {
            var id = User.Identity.GetUserId();
            ViewBag.Page = "Повідомлення";
            ViewBag.Id = id;
            var mess = GetData();
            
            ViewBag.Messages = mess.messages;
            ViewBag.Unread = mess.unread;
            return View(mess.dialogs);
        }
        public class Messenger
        {
            public List<Dialog> dialogs { get; set; }
            public List<Message> messages { get; set; }
            public List<UnreadMessages> unread { get; set; }
        }
        public Messenger GetData() {
            var id = User.Identity.GetUserId();
            var db = new ApplicationDbContext();
            var dialogsId = from n in db.Members where n.UserId == id && n.IsEnable == true select n.DialogId;
          
            var dialogs =db.Dialogs.Where(e=>dialogsId.Contains(e.Id)).ToList();


            List<Message> messages = new List<Message>();
            List<UnreadMessages> unread_messages = new List<UnreadMessages>();
            foreach (var i in dialogs)
            {
                var member = db.Members.Where(e => (e.DialogId == i.Id) && (e.UserId != id)).Select(n => n.UserId).ToList();
                if (member.Count == 1)
                {
                    if (member.FirstOrDefault() != null)
                    {
                        var user = db.Users.Where(e => e.Id == member.FirstOrDefault()).FirstOrDefault();
                        if (user != null)
                        {
                            i.Name = user.FirstName + " " + user.LastName;
                        }
                    }
                }

                var first_mess = db.Messages.Where(e => e.DialogId == i.Id).OrderByDescending(p => p.dateTime).FirstOrDefault();
                if(first_mess!=null)
                messages.Add(first_mess);

            }
            var messagesF = messages.OrderByDescending(e => e.dateTime).ToList();
            var resultsF = new List<Dialog>();
            for (int i = 0; i < messagesF.Count; i++)
            {
                int Id = messagesF[i].DialogId;
                var dialog = db.Dialogs.Where(e => e.Id == Id).FirstOrDefault();
                if (dialog != null)
                {
                    resultsF.Add(dialog);
                    UnreadMessages unM = new UnreadMessages();
                    unM.DialogId = dialog.Id;
                    unM.Count = db.Unread.Where(e => e.DialogId == dialog.Id && e.UserId.Contains(id)).Count();
                    unread_messages.Add(unM);
                }
            }

            Messenger mess = new Messenger();
            mess.messages = messagesF;
            mess.dialogs = resultsF;
            mess.unread = unread_messages;
            return mess;
        }
        public class UnreadMessages{
            public int DialogId { get; set; }
            public int Count { get; set; }
        }
        public JsonResult MessagesAjax()
        {
           
           var messenger = GetData();

            return Json(messenger,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMessages(int Id)
        {
            var db = new ApplicationDbContext();

           var result = db.Messages.Where(e => e.DialogId == Id).ToList();
            string userId = User.Identity.GetUserId();
            var toRemove = db.Unread.Where(e => e.UserId == userId && e.DialogId == Id).ToList();
            db.Unread.RemoveRange(toRemove);
            db.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendMessage(DateTime? dateTime,string UserId,int? DialogId,string Content)
        {
            Message m = new Message() { Content = Content, UserId = UserId,DialogId=DialogId.Value};
            if (dateTime == null)
                m.dateTime = DateTime.Now;
            else
                m.dateTime =(DateTime)dateTime;

            var db = new ApplicationDbContext();

            if(db.Dialogs.Where(e=>e.Id==DialogId.Value).FirstOrDefault().Name!=null)
            {
                var user = db.Users.Where(e => e.Id == UserId).FirstOrDefault();
                m.SenderG = user.FirstName + " " + user.LastName;
            }
            var members = db.Members.Where(e => e.DialogId == DialogId.Value && e.UserId != UserId).ToList();
            foreach (var member in members)
            {
                Unread unread = new Unread();
                unread.DialogId = DialogId.Value;
                unread.UserId = member.UserId;
                db.Unread.Add(unread);
            }
            db.Messages.Add(m);
            db.SaveChanges();
            return null;
        }
      
        public ActionResult CreateDialog(string AnotherUserId)
        {
            var db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();

            Dialog dialog = new Dialog();
            db.Dialogs.Add(dialog);
            db.SaveChanges();
            Member member1 = new Member() { DialogId =dialog.Id,UserId=user };
            Member member2 = new Member() { DialogId = dialog.Id, UserId = AnotherUserId };
            member1.IsEnable = true;
            member2.IsEnable = true;
            db.Members.Add(member1);
            db.Members.Add(member2);
            db.SaveChanges();

            return Content(dialog.Id+"");
        }
        public JsonResult GetAllUsers()
        {
            var db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();
            var result = db.Users.ToList();
            result.Remove(db.Users.Where(e => e.Id == user).FirstOrDefault());
            result.Reverse();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateChat(string Name,List<string> AnotherUsersId)
        {
            var db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();

            Dialog dialog = new Dialog();
            dialog.Name =Name;
          
            db.Dialogs.Add(dialog);
           
            db.SaveChanges();
           
            Member member1 = new Member() { DialogId = dialog.Id, UserId = user, IsEnable = true };           
            db.Members.Add(member1);
            for(int i = 0;i<AnotherUsersId.Count;i++)
            {
                Member member = new Member() { DialogId = dialog.Id, UserId=AnotherUsersId[i]};
                member.IsEnable = true;
                db.Members.Add(member);
            }

            db.SaveChanges();
            return Content(dialog.Id + "");
        }
        public ActionResult DisableDialog(int Id)
        {
            var db = new ApplicationDbContext();
            var user = User.Identity.GetUserId();

            var member = db.Members.Where(e => e.UserId == user && e.DialogId == Id).FirstOrDefault();
            member.IsEnable = false;
            db.SaveChanges();

            return null;
        }
        public JsonResult GetDialogInfo(int Id)
        {
            var db = new ApplicationDbContext();
            var result = db.Members.Where(e => e.DialogId == Id && e.IsEnable != false).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditChat(string Name, int Id, List<string> AnotherUsersId)
        {
            var db = new ApplicationDbContext();
            var dialog = db.Dialogs.Where(e=>e.Id==Id).FirstOrDefault();
            dialog.Name = Name;

            var members = db.Members.Where(e => e.DialogId == Id && e.IsEnable != false).ToList();

            db.Members.RemoveRange(members);
            
                for (int j = 0; j < AnotherUsersId.Count; j++)
                {
                Member m = new Member() { DialogId = Id, UserId = AnotherUsersId[j], IsEnable = true };
                db.Members.Add(m);
                }
            Member mI = new Member() { DialogId = Id, UserId = User.Identity.GetUserId(), IsEnable = true };
            db.Members.Add(mI);
            db.SaveChanges();
            return null;
        }
        }
}