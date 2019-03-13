using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplicationMVC.Models;
namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : Controller
    {
        ApplicationDbContext db;
        public AnalyticsController()
        {
            db = new ApplicationDbContext();
        }
        private string DateUkrFormat(DateTime dt)
        {
            string day = dt.Day <= 9 ? "0" + dt.Day : dt.Day + "";
            string month = dt.Month <= 9 ? "0" + dt.Month : dt.Month+"";
            return $"{day}.{month}.{dt.Year}";
        }
        // GET: Analytics
        public ActionResult Index()
        {
           
            ViewBag.Sources = db.Sources.ToList();
            ViewBag.Types = AnalyticsModel.GetTypeReports();

            var CounterpartiesId = db.Roles.Where(e => e.Name == "Counterparty").FirstOrDefault().Users.Select(e => e.UserId);
            var Counterparties = db.Users.Where(e => CounterpartiesId.Contains(e.Id)).ToList();
            ViewBag.Counterparties = Counterparties;
            return View(db.Users.ToList());
        }
        public byte[] GetReport(List<int> ordersIds)
        {

            string fileName =System.Web.HttpContext.Current.Server.MapPath("~/Content/word/Report.xlsx");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);
         
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(textByteArray, 0, textByteArray.Length);
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, true))
                {
                    SharedStringTablePart shareStringPart;
                    if (document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    else
                        shareStringPart = document.WorkbookPart.AddNewPart<SharedStringTablePart>();

                   
                    WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.FirstOrDefault();
                    const int startY = 3;
                    

                    var objects = db.ObjectLists.Where(e => ordersIds.Contains(e.OrderId)).ToList();
                   
                        for (int i = 0; i < objects.Count; i++)
                        {
                        int orderId = objects[i].OrderId;
                        var order = db.Orders.Where(e => e.Id == orderId).FirstOrDefault();
                        Cell cell = InsertCellInWorksheet("A", startY + i, worksheetPart);
                        int index = InsertSharedStringItem(order.Name, shareStringPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                        cell = InsertCellInWorksheet("B", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(DateUkrFormat(order.CreatedDate.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);




                        cell = InsertCellInWorksheet("C", startY + i, worksheetPart);
                        if (order.MetaId.HasValue)
                        {
                            var meta = db.Metas.Where(e => e.Id == order.MetaId.Value).FirstOrDefault();
                            if(meta!=null)
                            cell.CellValue = new CellValue(meta.Content);
                        }
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("D", startY + i, worksheetPart);
                        if (order.StatusId.HasValue)
                        {
                            string status = db.Statuses.Where(e => e.Id == order.StatusId.Value).FirstOrDefault().Content;
                            cell.CellValue = new CellValue(status);
                        }
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);


                            cell = InsertCellInWorksheet("E", startY + i, worksheetPart);
                            if (order.SourceId.HasValue)
                            {
                                var source = db.Sources.Where(e => e.Id == order.SourceId.Value).FirstOrDefault();
                                if(source!=null)
                                cell.CellValue = new CellValue(source.SourceName);
                            }
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("F", startY + i, worksheetPart);
                            if (order.BranchId.HasValue)
                            {
                                var branch = db.Branches.Where(e => e.Id == order.BranchId.Value).FirstOrDefault();
                                if(branch!=null)
                                cell.CellValue = new CellValue(branch.BranchName);
                            }
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);


                            var permisions = db.Performerses.Where(e => e.OrderId == order.Id).ToList();
                            StringBuilder strUsers = new StringBuilder("");
                            foreach (var user in permisions)
                            {
                                var User = db.Users.Where(e => e.Id == user.UserId).FirstOrDefault();
                                strUsers.Append($"{User.LastName} {User.FirstName} {User.MiddleName}, ");
                            }
                            if (strUsers.Length > 2)
                                strUsers.Remove(strUsers.Length - 2, 2);
                            cell = InsertCellInWorksheet("G", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(strUsers.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);




                            if (order.PropsId.HasValue)
                            {
                                var Props = db.Propses.Where(e => e.Id == order.PropsId).FirstOrDefault();
                                cell = InsertCellInWorksheet("H", startY + i, worksheetPart);
                                if (Props != null)
                                    cell.CellValue = new CellValue(Props.Content);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                    

                            StringBuilder str = new StringBuilder("");
                            var dateD = db.DateDocuments.Where(e => e.OrderId == orderId).ToList();
                            for (int j = 0; j < dateD.Count; j++)
                            {
                                var date = dateD[j].DateOfDocument;
                                if (date.HasValue)
                                    str.Append(DateUkrFormat(date.Value) + " ");
                            }
                            cell = InsertCellInWorksheet("I", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(str.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("J", startY + i, worksheetPart);
                            if(order.DateOfExpert.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfExpert.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        decimal sum = 0;
                            var prices = new List<Price>();
                            if (order.PriceListId.HasValue)
                            {
                                prices = db.Prices.Where(e => e.PriceListId == order.PriceListId.Value).ToList();
                                foreach (var p in prices)
                                {
                                    sum += p.Value;
                                }
                            }
                            cell = InsertCellInWorksheet("K", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(sum + "");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            string IsPaid = "Ні";
                            if (order.IsPaid.HasValue && order.IsPaid.Value)
                                IsPaid = "Так";
                            cell = InsertCellInWorksheet("L", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(IsPaid);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("M", startY + i, worksheetPart);
                            if (order.DateOfPay.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfPay.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            if (order.PropsId.HasValue)
                            {
                                var Props = db.Propses.Where(e => e.Id == order.PropsId).FirstOrDefault();
                                cell = InsertCellInWorksheet("N", startY + i, worksheetPart);
                                if (Props != null)
                                    cell.CellValue = new CellValue(Props.Content);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }

                            //Client
                            int clientId = order.ClientId;
                            var client = db.Clients.Where(e => e.Id == clientId).FirstOrDefault();
                            if (client != null)
                            {
                                cell = InsertCellInWorksheet("O", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(client.FullName);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String); //nopqr
                                cell = InsertCellInWorksheet("P", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(client.IPN_EDRPOY);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("Q", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(client.Phone);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("R", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(client.Email);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("S", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(client.Props);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                            //Owner
                            int ownerId = order.OwnerId;
                            var owner = db.Owners.Where(e => e.Id == ownerId).FirstOrDefault();
                            if (owner != null)//stuvw
                            {
                                cell = InsertCellInWorksheet("T", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(owner.FullName);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("U", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(owner.IPN_EDRPOY);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("V", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(owner.Phone);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("W", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(owner.Email);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                cell = InsertCellInWorksheet("X", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(owner.Props);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            }

                            int objId = objects[i].ObjectId;
                            var @object = db.Objects.Where(e => e.Id == objId).FirstOrDefault();
                            if (@object != null)
                            {
                                cell = InsertCellInWorksheet("Y", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(@object.Name);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);

                                int objList = objects[i].Id;
                                var values = db.ObjectValues.Where(e => e.ObjectListId == objList).ToList();
                                var desks = db.ObjectDesces.Where(e => e.ObjectTypeId == objId).ToList();

                                cell = InsertCellInWorksheet("Z", startY + i, worksheetPart);
                                StringBuilder strDesk = new StringBuilder("");
                                foreach (var desk in desks)
                                {
                                    var value = values.Where(e => e.ObjectDeskId == desk.Id).FirstOrDefault();
                                    if (value != null)
                                        strDesk.Append(", " + desk.Name + " - " + value.Value);
                                }
                                if (strDesk.Length >= 2)
                                    strDesk.Remove(0, 2);
                                cell.CellValue = new CellValue(strDesk.ToString());
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);

                                StringBuilder strAddress = new StringBuilder("");

                                var obl = desks.Where(e => e.Name.Contains("Область")).FirstOrDefault();
                                if (obl != null)
                                {
                                    var OblValue = values.Where(e => e.ObjectDeskId == obl.Id).FirstOrDefault();
                                    strAddress.Append("Область - " + OblValue.Value);
                                    //
                                    var Reg = desks.Where(e => e.Name.Contains("Район")).FirstOrDefault();
                                    if (Reg != null)
                                    {
                                        var RegValue = values.Where(e => e.ObjectDeskId == Reg.Id).FirstOrDefault();
                                        strAddress.Append(", Район - " + RegValue.Value);
                                    }
                                    //
                                    var Settlement = desks.Where(e => e.Name.Contains("Населений пункт")).FirstOrDefault();
                                    if (Settlement != null)
                                    {
                                        var SetValue = values.Where(e => e.ObjectDeskId == Settlement.Id).FirstOrDefault();
                                        strAddress.Append(", Населений пункт - " + SetValue.Value);
                                    }
                                    var street = desks.Where(e => e.Name.Contains("вул") || e.Name.Contains("Вул")).FirstOrDefault();
                                    if (street != null)
                                    {
                                        int Street = street.Id;
                                        var StreetValue = values.Where(e => e.ObjectDeskId == Street).FirstOrDefault();
                                        strAddress.Append(", Вулиця - " + StreetValue.Value);
                                    }
                                    var build = desks.Where(e => e.Name.Contains("буд") || e.Name.Contains("Буд")).FirstOrDefault();
                                    if (build != null)
                                    {
                                        int Build = build.Id;
                                        var BuildValue = values.Where(e => e.ObjectDeskId == Build).FirstOrDefault();
                                        strAddress.Append(", Будинок - " + BuildValue.Value);
                                    }
                                    var flat = desks.Where(e => e.Name.Contains("кв") || e.Name.Contains("Кв")).FirstOrDefault();
                                    if (flat != null)
                                    {
                                        int Flat = flat.Id;
                                        var FlatValue = values.Where(e => e.ObjectDeskId == Flat).FirstOrDefault();
                                        strAddress.Append(", Квартира - " + FlatValue.Value);
                                    }



                                    cell = InsertCellInWorksheet("AA", startY + i, worksheetPart);
                                    cell.CellValue = new CellValue(strAddress.ToString());
                                    cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                }

                            }
                       

                        cell = InsertCellInWorksheet("AB", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(order.FullNameWatcher ?? "");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AC", startY + i, worksheetPart);
                            if (order.OverWatch.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.OverWatch.Value));
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AD", startY + i, worksheetPart);
                            if (order.PriceOverWatch.HasValue)
                                cell.CellValue = new CellValue(order.PriceOverWatch.Value.ToString());
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AE", startY + i, worksheetPart);
                            if (order.IsPaidOverWatch)
                                cell.CellValue = new CellValue("Так");
                            else
                                cell.CellValue = new CellValue("Ні");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AF", startY + i, worksheetPart);
                            if (order.DateOfTakeVerification.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfTakeVerification.Value));
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AG", startY + i, worksheetPart);
                            if (order.DateOfDirectVerification.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfDirectVerification.Value));
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AH", startY + i, worksheetPart);
                            if (order.DateOfEndVerification.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfEndVerification.Value));
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AI", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(order.CommentVerification);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AJ", startY + i, worksheetPart);
                            if (order.DateOfTransfer.HasValue)
                                cell.CellValue = new CellValue(DateUkrFormat(order.DateOfTransfer.Value));
                            else
                                cell.CellValue = new CellValue("");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AK", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(order.CommentOfTransfer);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AL", startY + i, worksheetPart);
                            cell.CellValue = new CellValue("http://drive.google.com/file/d/" + order.DirectoryId);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("AM", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(order.Comments);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        
                    
                }
                return stream.ToArray();
                }

            
        }
        
        public byte[] GetReportKredo(List<int> ordersIds)
        {
            string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "Content/word/Kredo.xlsx");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(textByteArray, 0, textByteArray.Length);
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, true))
                {
                    SharedStringTablePart shareStringPart;
                    if (document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    else
                        shareStringPart = document.WorkbookPart.AddNewPart<SharedStringTablePart>();


                    WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.FirstOrDefault();
                    const int startY = 2;
                    
                    //var objects = db.ObjectLists.Where(e => ordersIds.Contains(e.OrderId)).ToList();
                    Cell cell;
                    for (int i = 0; i < ordersIds.Count; i++) {
                        int orderId = ordersIds[i];
                        var order = db.Orders.Where(e => e.Id == orderId).FirstOrDefault();
                        cell = InsertCellInWorksheet("A", startY + i, worksheetPart);
                        cell.CellValue = new CellValue((i+1)+"");
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        var Client = db.Clients.Where(e => e.Id == order.ClientId).FirstOrDefault();
                        string ClientOrder = $" ({order.Name})";
                        if (Client != null)
                            ClientOrder =Client.FullName+ClientOrder;
                        cell = InsertCellInWorksheet("B", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(ClientOrder);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("C", startY + i, worksheetPart);
                        if(order.DateOfExpert.HasValue)
                        cell.CellValue = new CellValue(DateUkrFormat(order.DateOfExpert.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        decimal sum = 0;
                        var prices = new List<Price>();
                        if (order.PriceListId.HasValue)
                        {
                            prices = db.Prices.Where(e => e.PriceListId == order.PriceListId.Value).ToList();
                            foreach(var p in prices)
                            {
                                sum += p.Value;
                            }
                        }
                        cell = InsertCellInWorksheet("D", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(sum+"");
                        
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                    }

                    // for recacluation of formula
                    document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                    document.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;
                }
                return stream.ToArray();
            }

        }
        public byte[] GetReportNotarius(List<int> ordersIds)
        {
            string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "Content/word/notarius.xlsx");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);
            var db = new ApplicationDbContext();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(textByteArray, 0, textByteArray.Length);
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, true))
                {
                    SharedStringTablePart shareStringPart;
                    if (document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    else
                        shareStringPart = document.WorkbookPart.AddNewPart<SharedStringTablePart>();

                    WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.FirstOrDefault();
                    const int startY = 2;
                   
                    var objects = db.ObjectLists.Where(e => ordersIds.Contains(e.OrderId)).ToList();
                    Cell cell;
                    for (int i = 0; i < objects.Count; i++)
                    {
                        int orderId = objects[i].OrderId;
                        var order = db.Orders.Where(e => e.Id == orderId).FirstOrDefault();

                        //Client
                        int clientId = order.ClientId;
                        var client = db.Clients.Where(e => e.Id == clientId).FirstOrDefault();
                        if (client != null)
                        {
                            cell = InsertCellInWorksheet("A", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(client.FullName??"");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("B", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(client.IPN_EDRPOY??"");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        //Owner
                        int ownerId = order.OwnerId;
                        var owner = db.Owners.Where(e => e.Id == ownerId).FirstOrDefault();
                        if (owner != null)
                        {
                            cell = InsertCellInWorksheet("C", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(owner.FullName ?? "");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                            cell = InsertCellInWorksheet("D", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(owner.IPN_EDRPOY??"");
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        //Object
                        int objId = objects[i].ObjectId;
                        var @object = db.Objects.Where(e => e.Id == objId).FirstOrDefault();
                        if (@object != null)
                        {
                            cell = InsertCellInWorksheet("E", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(@object.Name);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }

                        cell = InsertCellInWorksheet("F", startY + i, worksheetPart);
                        decimal sum = 0;
                        var prices = db.Prices.Where(e => e.PriceListId == order.PriceListId).ToList();
                        for(int j=0;j<prices.Count;j++)
                        {
                            sum += prices[j].Value;
                        }
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        cell.CellValue = new CellValue(sum.ToString());


                        cell = InsertCellInWorksheet("G", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(order.Name);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("H", startY + i, worksheetPart);
                        var statuse = db.Statuses.Where(e => e.Id == order.StatusId).FirstOrDefault();
                        string statusStr = "";
                        if (statuse != null)
                            statusStr = statuse.Content;
                        cell.CellValue = new CellValue(statusStr);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("I", startY + i, worksheetPart);
                        var source = db.Sources.Where(e => e.Id == order.SourceId).FirstOrDefault();
                        string sourceStr = "";
                        if (source != null)
                            sourceStr = source.SourceName;
                        cell.CellValue = new CellValue(sourceStr);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("J", startY + i, worksheetPart);
                        if (order.DateOfExpert.HasValue)
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfExpert.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                    }

                   

                    decimal globalPrice = 0;
                    var priceLists = db.Orders.Where(e => ordersIds.Contains(e.Id)).Select(e=>e.PriceListId).ToList();
                    for(int j = 0; j < priceLists.Count; j++)
                    {
                        if (!priceLists[j].HasValue)
                            continue;
                        int priceListId = priceLists[j].Value;
                        var prices = db.Prices.Where(e => e.PriceListId ==priceListId).ToList();
                        foreach(var p in prices)
                        {
                            globalPrice += p.Value;
                        }
                    }
                    cell = InsertCellInWorksheet("L", 1, worksheetPart);
                    cell.CellValue = new CellValue(globalPrice.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.String);

                    

                    document.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                    document.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;
                }
                return stream.ToArray();
            }
        }

        public byte[] GetReportUkrGas(List<int> ordersIds)
        {
   
            string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "Content/word/Звіт.xlsx");
            
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);
            var db = new ApplicationDbContext();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(textByteArray, 0, textByteArray.Length);
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, true))
                {
                    SharedStringTablePart shareStringPart;
                    if (document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    else
                        shareStringPart = document.WorkbookPart.AddNewPart<SharedStringTablePart>();


                    WorksheetPart worksheetPart = document.WorkbookPart.WorksheetParts.FirstOrDefault();
                    const int startY = 4;
                    
                    var objects = db.ObjectLists.Where(e => ordersIds.Contains(e.OrderId)).ToList();

                    for (int i = 0; i < objects.Count; i++)
                    {
                        int orderId = objects[i].OrderId;
                        var order = db.Orders.Where(e => e.Id == orderId).FirstOrDefault();

                        Cell cell = InsertCellInWorksheet("A", startY + i, worksheetPart);
                        int index = InsertSharedStringItem((i + 1) + "", shareStringPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        //Client
                        int clientId = order.ClientId;
                        var client = db.Clients.Where(e => e.Id == clientId).FirstOrDefault();
                        if (client != null)
                        {
                            cell = InsertCellInWorksheet("B", startY + i, worksheetPart);
                            index = InsertSharedStringItem(client.FullName ?? "", shareStringPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                            cell = InsertCellInWorksheet("C", startY + i, worksheetPart);
                            index = InsertSharedStringItem(client.IPN_EDRPOY, shareStringPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        }
                        //Owner
                        int ownerId = order.OwnerId;
                        var owner = db.Owners.Where(e => e.Id == ownerId).FirstOrDefault();
                        if (owner != null)
                        {
                            cell = InsertCellInWorksheet("D", startY + i, worksheetPart);
                            index = InsertSharedStringItem(owner.FullName ?? "", shareStringPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                            cell = InsertCellInWorksheet("E", startY + i, worksheetPart);
                            index = InsertSharedStringItem(owner.IPN_EDRPOY, shareStringPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        }
                        //Object
                        int objId = objects[i].ObjectId;
                        var @object = db.Objects.Where(e => e.Id == objId).FirstOrDefault();
                        if (@object != null)
                        {
                            cell = InsertCellInWorksheet("F", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(@object.Name);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            cell = InsertCellInWorksheet("G", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(@object.Name);
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            int objList = objects[i].Id;
                            var values = db.ObjectValues.Where(e => e.ObjectListId == objList).ToList();
                            var desks = db.ObjectDesces.Where(e => e.ObjectTypeId ==objId).ToList();
                            if (desks.Where(e => e.Name.Contains("Область")).FirstOrDefault() != null)
                            {
                                int Obl = desks.Where(e => e.Name == "Область").FirstOrDefault().Id;
                                string OblValue = values.Where(e => e.ObjectDeskId == Obl).FirstOrDefault().Value;
                                cell = InsertCellInWorksheet("H", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(OblValue);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                //
                                var Reg = desks.Where(e => e.Name.Contains("Район")).FirstOrDefault();
                                string RegValue = "";
                                if (Reg != null)
                                {
                                    RegValue = values.Where(e => e.ObjectDeskId == Reg.Id).FirstOrDefault().Value;
                                }
                                cell = InsertCellInWorksheet("I", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(RegValue);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                //
                                var Settlement = desks.Where(e => e.Name.Contains("Населений пункт")).FirstOrDefault();
                                if (Settlement != null)
                                {
                                    string SetValue = values.Where(e => e.ObjectDeskId == Settlement.Id).FirstOrDefault().Value;
                                    var settlementArr = SetValue.Split('.');
                                    if (settlementArr.Length > 1)
                                    {
                                        cell = InsertCellInWorksheet("J", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(settlementArr[0] + ".");
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                        cell = InsertCellInWorksheet("K", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(SetValue.Remove(0, settlementArr[0].Length + 1));
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                    }
                                    else
                                    {
                                        cell = InsertCellInWorksheet("K", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(SetValue);
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                    }
                                }
                                var street = desks.Where(e => e.Name.Contains("вул") || e.Name.Contains("Вул")).FirstOrDefault();
                                if (street != null)
                                {
                                    int Street = street.Id;
                                    string StreetValue = values.Where(e => e.ObjectDeskId == Street).FirstOrDefault().Value;
                                    var StreetArr = StreetValue.Split('.');
                                    if (StreetArr.Length > 1)
                                    {
                                        cell = InsertCellInWorksheet("L", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(StreetArr[0] + ".");
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                        cell = InsertCellInWorksheet("M", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(StreetValue.Remove(0, StreetArr[0].Length + 1));
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                    }
                                    else
                                    {
                                        cell = InsertCellInWorksheet("M", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(StreetValue);
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                    }
                                }
                                var build = desks.Where(e => e.Name.Contains("буд") || e.Name.Contains("Буд")).FirstOrDefault();
                                if (build != null)
                                {
                                    int Build = build.Id;
                                    var BuildValue = values.Where(e => e.ObjectDeskId == Build).FirstOrDefault();
                                    cell = InsertCellInWorksheet("N", startY + i, worksheetPart);
                                    if(BuildValue!=null)
                                    cell.CellValue = new CellValue(BuildValue.Value);
                                    else
                                        cell.CellValue = new CellValue("");
                                    cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                }
                                var flat = desks.Where(e => e.Name.Contains("кв")|| e.Name.Contains("Кв")).FirstOrDefault();
                                if (flat != null)
                                {
                                    int Flat = flat.Id;
                                    var FlatValue = values.Where(e => e.ObjectDeskId == Flat).FirstOrDefault();
                                    if (FlatValue != null)
                                    {
                                        cell = InsertCellInWorksheet("O", startY + i, worksheetPart);
                                        cell.CellValue = new CellValue(FlatValue.Value);
                                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                                    }
                                }
                            }
                        }
                        cell = InsertCellInWorksheet("P", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(DateUkrFormat(order.CreatedDate.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("Q", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(DateUkrFormat(order.CreatedDate.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("R", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(order.Name );
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        if (order.DateOfPay.HasValue)
                        {
                            cell = InsertCellInWorksheet("S", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfPay.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        StringBuilder str = new StringBuilder("");
                        var dateD = db.DateDocuments.Where(e => e.OrderId == orderId).ToList();
                        for (int j = 0; j < dateD.Count; j++)
                        {
                            var date = dateD[j].DateOfDocument;
                            if (date.HasValue)
                                str.Append(DateUkrFormat(date.Value) + " ");
                        }
                        cell = InsertCellInWorksheet("T", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(str.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("U", startY + i, worksheetPart);
                        if (order.DateOfExpert.HasValue)
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfExpert.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("V", startY + i, worksheetPart);
                        if (order.OverWatch.HasValue)                  
                            cell.CellValue = new CellValue(DateUkrFormat(order.OverWatch.Value));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        DateTime dateTakeToWork;
                        List<DateTime> dates = new List<DateTime>();
                        if (order.DateOfPay.HasValue)
                            dates.Add(order.DateOfPay.Value);
                        if (order.OverWatch.HasValue)
                            dates.Add(order.OverWatch.Value);
                        if (order.CreatedDate.HasValue)
                            dates.Add(order.CreatedDate.Value);

                        dateTakeToWork = dates.Max();
                        
                        cell = InsertCellInWorksheet("W", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(DateUkrFormat(dateTakeToWork));
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        var permisions = db.SignatoryOrders.Where(e => e.OrderId == order.Id).Select(e=>e.SignatoryId).ToList();
                        StringBuilder strUsers = new StringBuilder("");
                        foreach (var userId in permisions)
                        {
                            var User = db.Users.Where(e => e.Id == userId).FirstOrDefault();
                            strUsers.Append($"{User.LastName} {User.FirstName} {User.MiddleName}, ");
                        }
                        if(strUsers.Length>2)
                        strUsers.Remove(strUsers.Length - 2, 2);

                        cell = InsertCellInWorksheet("X", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(strUsers.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        if (order.DateOfTakeVerification.HasValue)
                        {
                            cell = InsertCellInWorksheet("Y", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfTakeVerification.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        if (order.DateOfDirectVerification.HasValue)
                        {
                            cell = InsertCellInWorksheet("Z", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfDirectVerification.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        if (order.DateOfEndVerification.HasValue)
                        {
                            cell = InsertCellInWorksheet("AA", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfEndVerification.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        cell = InsertCellInWorksheet("AB", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(order.CommentVerification ?? "");
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        if (order.DateOfTransfer.HasValue)
                        {
                            cell = InsertCellInWorksheet("AC", startY + i, worksheetPart);
                            cell.CellValue = new CellValue(DateUkrFormat(order.DateOfTransfer.Value));
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                        cell = InsertCellInWorksheet("AD", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(order.CommentOfTransfer ?? "");
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        cell = InsertCellInWorksheet("AE", startY + i, worksheetPart);
                        cell.CellValue = new CellValue("ПП \"ТА - Експерт - Сервіс\"");
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        if (order.StatusId.HasValue)
                        {
                            int statusId = order.StatusId.Value;
                            var status = db.Statuses.Where(e => e.Id == statusId).FirstOrDefault();
                            if (status != null)
                            {
                                cell = InsertCellInWorksheet("AF", startY + i, worksheetPart);
                                cell.CellValue = new CellValue(status.Content);
                                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                            }
                        }
                        cell = InsertCellInWorksheet("AG", startY + i, worksheetPart);
                        cell.CellValue = new CellValue(order.Comments);
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                    }
                }
                return stream.ToArray();
            }
        }
        public ActionResult GenerateReport(DateTime? date1, DateTime? date2, string Performers,int SourceId,int TypeId,string CounterpartyId)
        {
            if (date1 > date2)
            {
                var tmp = date1.Value;
                date1 = date2.Value;
                date2 = tmp;
            }
            date2 = date2.Value.AddDays(1);
            byte[] arr=new byte[0];

            
            var ordersIds = new List<int>();
            var orders = new List<Order>();
            if (Performers == "1")
            {
                if (SourceId != -1)
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && e.SourceId == SourceId).ToList();
                else
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2).ToList();
            }
            else
            {
                var perfomers = db.Performerses.Where(e => e.UserId.Contains(Performers)).Select(e => e.OrderId).ToList();
                if (SourceId != -1)
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id) && e.SourceId == SourceId).ToList();
                else
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id)).ToList();
            }

            if (CounterpartyId != "0")
            {
                ordersIds = orders.Where(e => e.CounterpartyId == CounterpartyId).Select(e => e.Id).ToList();
            }
            else
            {
                ordersIds = orders.Select(e => e.Id).ToList();
            }

            switch (TypeId)
            {
                case 0:
                    arr = GetReport(ordersIds);
                    break;
                case 1:
                    arr = GetReportUkrGas(ordersIds);
                    break;
                case 2:
                    arr= GetReportKredo(ordersIds);
                    break;
                case 3:
                    arr = GetReportNotarius(ordersIds);
                    break;
                default:
                    return RedirectToAction("Index");
            }
            return File(arr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Звіт.xlsx");
        }
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a WorkbookPart, inserts a new worksheet.
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        private static Cell InsertCellInWorksheet(string columnName, int rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = Convert.ToUInt32(rowIndex) };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}