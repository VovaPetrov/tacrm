using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles = "Counterparty")]
    public class AnalyticsCounterpartyController : Controller
    {
        ApplicationDbContext db;
        AnalyticsController analyticsController;
        public AnalyticsCounterpartyController()
        {
            db = new ApplicationDbContext();
            analyticsController = new AnalyticsController();
        }
        // GET: AnalyticsCounterparty
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.User = db.Users.First(e=>e.Id==userId);

            var SourcesCounterparties = db.SourceCounterparties.Where(e => e.CounterpartyId == userId).Select(e => e.SourceId);
            ViewBag.Sources = db.Sources.Where(e => SourcesCounterparties.Contains(e.Id));

            var AnalyticsCounterparties = db.AnalyticsCounterparties.Where(e => e.CounterpartyId == userId).Select(e => e.AnalyticsId);
            ViewBag.Analytics = AnalyticsModel.GetTypeReports().Where(e=>AnalyticsCounterparties.Contains(e.Id));

            var PerformersCounterparties = db.PerformerCounterparties.Where(e => e.CounterpartyId == userId).Select(e => e.PerformerId);
            ViewBag.Performers = db.Users.Where(e => PerformersCounterparties.Contains(e.Id));

            return View();
        }
        [HttpPost]
        public ActionResult GenerateReport(DateTime? date1, DateTime? date2, string Performers, int SourceId, int TypeId)
        {
            if (date1 > date2)
            {
                var tmp = date1.Value;
                date1 = date2.Value;
                date2 = tmp;
            }
            date2 = date2.Value.AddDays(1);
            if (date1 == null)
            {
                date1 = DateTime.Now.AddMonths(-1).AddDays(-1);
            }
            if(date1<DateTime.Now.AddMonths(-1).AddDays(-1))
            {
                date1 = DateTime.Now.AddMonths(-1).AddDays(-1);
            }

            byte[] arr = new byte[0];


            var ordersIds = new List<int>();
            var orders = new List<Order>();
            var userId = User.Identity.GetUserId();
            if (Performers == "0")
            {
                
                var signatories = db.PerformerCounterparties.Where(e => e.CounterpartyId == userId).Select(e=>e.PerformerId).ToList();
                var perfomers = db.SignatoryOrders.Where(e => signatories.Contains(e.SignatoryId)).Select(e => e.OrderId).ToList();
                if (SourceId != -1)
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id) && e.SourceId == SourceId && e.CounterpartyId==userId).ToList();
                else
                {
                    var sources = db.SourceCounterparties.Where(e => e.CounterpartyId == userId).Select(e => e.SourceId).ToList();
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id) && e.SourceId.HasValue && sources.Contains(e.SourceId.Value) && e.CounterpartyId == userId).ToList();
                }
            }           
            else
            {
                var perfomers = db.SignatoryOrders.Where(e => e.SignatoryId==Performers).Select(e => e.OrderId).ToList();
                if (SourceId != -1)
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id) && e.SourceId == SourceId && e.CounterpartyId == userId).ToList();
                else
                {
                    var sources = db.SourceCounterparties.Where(e => e.CounterpartyId == userId).Select(e => e.SourceId).ToList();
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id) && e.SourceId.HasValue && sources.Contains(e.SourceId.Value) && e.CounterpartyId == userId).ToList();
                }
            }

            
                ordersIds = orders.Select(e => e.Id).ToList();
   

            switch (TypeId)
            {
                case 0:
                    arr = analyticsController.GetReport(ordersIds);
                    break;
                case 1:
                    arr = analyticsController.GetReportUkrGas(ordersIds);
                    break;
                case 2:
                    arr = analyticsController.GetReportKredo(ordersIds);
                    break;
                case 3:
                    arr = analyticsController.GetReportNotarius(ordersIds);
                    break;
                default:
                    return RedirectToAction("Index");
            }
            return File(arr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Звіт.xlsx");
        }
    }
}