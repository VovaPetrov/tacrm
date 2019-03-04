using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles="Admin")]
    public class ConfigController : Controller
    {
        // GET: Config
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var Objects = db.Objects.ToList();
            ViewBag.Objects = Objects;
            var Props = db.Propses.ToList();
            ViewBag.Props = Props;
            var Sources = db.Sources.ToList();
            ViewBag.Source = Sources;
            var Branches = db.Branches.ToList();
            ViewBag.Branches = Branches;
            var Metas = db.Metas.ToList();
            ViewBag.Metas = Metas;
            return View();
        }
        public ActionResult AddMeta(string Meta)
        {
            var db = new ApplicationDbContext();
            var meta = new Meta();
            meta.Content = Meta;
            db.Metas.Add(meta);
            db.SaveChanges();
            return Content(meta.Id + "");
        }
        public ActionResult AddBranch(int SourceId,string BranchName)
        {
            var db = new ApplicationDbContext();
            Branch branch = new Branch() { BranchName=BranchName, SourceId=SourceId};
            db.Branches.Add(branch);
            db.SaveChanges();
            return Content(branch.Id+"");
        }
        public ActionResult DelMeta(int Id)
        {
            var db = new ApplicationDbContext();
            var orders = db.Orders.Where(e => e.MetaId == Id).ToList();
            foreach (var ord in orders)
            {
                ord.MetaId = null;
            }
            var meta = db.Metas.Where(e => e.Id == Id).FirstOrDefault();
            db.Metas.Remove(meta);
            db.SaveChanges();
            return null;
        }
        public ActionResult DelBranch(int Id)
        {
            var db = new ApplicationDbContext();
            var orders = db.Orders.Where(e => e.BranchId == Id).ToList();
            foreach(var ord in orders)
            {
                ord.BranchId = null;
            }
            var branch = db.Branches.Where(e => e.Id == Id).FirstOrDefault();
            db.Branches.Remove(branch);
            db.SaveChanges();
            return null;
        }
        public ActionResult DelSource(int Id)
        {
            var db = new ApplicationDbContext();
            var branches = db.Branches.Where(e => e.SourceId == Id).ToList();
            var branchesId = db.Branches.Where(e => e.SourceId == Id).Select(e=>e.Id).ToList();
            var sources = db.Sources.Where(e => e.Id == Id).ToList();
            var orders = db.Orders.Where(e => branchesId.Contains(e.BranchId.Value));
            foreach(var ord in orders)
            {
                ord.BranchId = null;
                ord.SourceId = null;
            }
            db.Branches.RemoveRange(branches);
            db.Sources.RemoveRange(sources);
            db.SaveChanges();
            return null;
        }
        public ActionResult AddSource(string SourceName)
        {
            var db = new ApplicationDbContext();
            Source source = new Source() { SourceName = SourceName };
            db.Sources.Add(source);
            db.SaveChanges();
            return Content(source.Id+"");
        }
        public ActionResult DelProp(int Id)
        {
            var db = new ApplicationDbContext();
            var orders = db.Orders.Where(e => e.PropsId == Id).ToList();
            foreach(var ord in orders)
            {
                ord.PropsId = null;
            }
            var Prop = db.Propses.Where(e => e.Id == Id).FirstOrDefault();
            db.Propses.Remove(Prop);
            db.SaveChanges();
            return null;
        }
        public ActionResult AddProp(string text)
        {
            var db = new ApplicationDbContext();
            Props props = new Props() { Content = text };
            db.Propses.Add(props);
            db.SaveChanges();
            return Content(props.Id+"");
        }
        [HttpPost]
        public ActionResult AddObjectType(string Name)
        {
            var db = new ApplicationDbContext();
            ObjectType @object = new ObjectType() { Name = Name };
            db.Objects.Add(@object);
            db.SaveChanges();
            return Content(@object.Id+"");
        }
        [HttpPost]
        public ActionResult DelObjectType(int Id)
        {
            var db = new ApplicationDbContext();
            var obj = db.Objects.Where(e => e.Id == Id).FirstOrDefault();

            var objLists = db.ObjectLists.Where(e => e.ObjectId == obj.Id).ToList();
            var objListsIds = objLists.Select(e => e.Id);
            var objValues = db.ObjectValues.Where(e => objListsIds.Contains(e.ObjectListId)).ToList();
            db.ObjectValues.RemoveRange(objValues);
            db.ObjectLists.RemoveRange(objLists);

            var objDesk = db.ObjectDesces.Where(e => e.ObjectTypeId == obj.Id).ToList();
            db.ObjectDesces.RemoveRange(objDesk);
            db.Objects.Remove(obj);
            db.SaveChanges();
            return null;
        }
        public JsonResult GetDesk(int Id)
        {
            var db = new ApplicationDbContext();
            var result = db.ObjectDesces.Where(e => e.ObjectTypeId == Id).ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditDesk(int? Obj, IEnumerable<int> DeskIds, IEnumerable<string> DeskNames)
        {
            var db = new ApplicationDbContext();
            var DeskIdsArray = DeskIds.ToList();
            var DeskNamesArray = DeskNames.ToList();
            for (int i = 0; i < DeskIdsArray.Count; i++)
            {
                if (DeskIdsArray[i] == 0)
                {
                    string name = DeskNamesArray[i];
                    ObjectDesc desk = new ObjectDesc { Name = name, ObjectTypeId = Obj.Value };
                    db.ObjectDesces.Add(desk);
                }
                else
                {
                    int Id = DeskIdsArray[i];
                    var desk = db.ObjectDesces.Where(e => e.Id == Id).FirstOrDefault();
                    if (desk != null)
                        desk.Name = DeskNamesArray[i];
                }
            }
            var removeDesks = db.ObjectDesces.Where(e => !DeskIdsArray.Contains(e.Id) && e.ObjectTypeId == Obj.Value).ToList();
            db.ObjectDesces.RemoveRange(removeDesks);
            db.SaveChanges();
            return null;
        }
    }
}