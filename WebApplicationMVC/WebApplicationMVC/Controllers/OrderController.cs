using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Web.Mvc;
using WebApplicationMVC.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using Microsoft.AspNet.Identity;
using Google.Apis.Download;
using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationMVC.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class OrderController : Controller
    {
        ApplicationDbContext db;
        public OrderController()
        {
            db = new ApplicationDbContext();
        }
        public DriveService GetService()
        {
            string[] Scopes = { DriveService.Scope.Drive };
            string ApplicationName = "Drive API .NET Quickstart";
            UserCredential credential;
            using (var stream = new FileStream(Server.MapPath(("~/Content/API/DriveCredentials.json")), FileMode.Open, FileAccess.Read))
            {
                String FilePath = Server.MapPath(("~/Content/API/DriveServiceCredentials.json"));

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }
            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var UserApp = db.Users.Where(e => e.Id.Contains(userId)).FirstOrDefault();
            var UserRole = UserApp.Roles.FirstOrDefault();
            var result = new List<Order>();
            if (UserRole.RoleId.Contains("1"))
            {
                result = db.Orders.ToList();
            }
            else
            {
                var perfomerses = db.Performerses.Where(e => string.Compare(e.UserId, userId) == 0).Select(e => e.OrderId);
                result = db.Orders.Where(e => perfomerses.Contains(e.Id)).ToList();
            }
            int[] StatusRange1 = { 4, 5, 6, 7 };
            int[] StatusRange2 = { 1, 2, 3 };
            int[] StatusRange3 = { 8 };
            int[] StatusRange4 = { 9 };
            int[] StatusRange5 = { 10 };
            var range1 = result.Where(e => StatusRange1.Contains(e.StatusId.Value));
            var range2 = result.Where(e => StatusRange2.Contains(e.StatusId.Value));
            var range3 = result.Where(e => StatusRange3.Contains(e.StatusId.Value));
            var range4 = result.Where(e => StatusRange4.Contains(e.StatusId.Value));
            var range5 = result.Where(e => StatusRange5.Contains(e.StatusId.Value));
            var finalResult = new List<Order>();
            finalResult.AddRange(range1);
            finalResult.AddRange(range2);
            finalResult.AddRange(range3);
            finalResult.AddRange(range4);
            finalResult.AddRange(range5);

            return View(finalResult);
        }
        public async Task<RedirectResult> CreateCopy(int? orderId)
        {
            int id;
            var order = db.Orders.FirstOrDefault(e => e.Id == orderId.Value);
            var newOrder = new Order() {
                IsPaid = order.IsPaid,
                IsPaidOverWatch = order.IsPaidOverWatch,
                MetaId = order.MetaId,
                BranchId = order.BranchId,
                ClientId=order.ClientId,
                CommentOfTransfer = order.CommentOfTransfer,
                Comments = order.Comments,
                CommentVerification = order.CommentVerification,
                CountDays = order.CountDays,
                CreatedDate = order.CreatedDate,
                DateOfDirectVerification = order.DateOfDirectVerification,
                DateOfEndVerification = order.DateOfEndVerification,
                DateOfExpert = order.DateOfExpert,
                DateOfPay = order.DateOfPay,
                DateOfTakeVerification = order.DateOfTakeVerification,
                DateOfTransfer = order.DateOfTransfer,
                FullNameWatcher = order.FullNameWatcher,
                OverWatch = order.OverWatch,
                OwnerId = order.OwnerId,
                PriceListId = order.PriceListId,
                PriceOverWatch = order.PriceOverWatch,
                PropsId = order.PropsId,
                SourceId = order.SourceId,
                StatusId = order.StatusId,  
                CounterpartyId = order.CounterpartyId,
            };

            string month = DateTime.Now.Month <= 9 ? "0" + DateTime.Now.Month : DateTime.Now.Month + "";
            var lastOrder = db.Orders.Where(e => e.CreatedDate.Value.Month == DateTime.Now.Month && e.CreatedDate.Value.Year == DateTime.Now.Year).ToList().LastOrDefault();
            int lastCount;
            if (lastOrder == null)
            {
                lastCount = 1;
            }
            else
            {
                lastCount = Convert.ToInt32(lastOrder.Name.Substring(6));
            }

            string Name = DateTime.Now.Year + "" + month + String.Format("{0:0000}", lastCount + 1);
            newOrder.Name = Name;

            db.Orders.Add(newOrder);
            await db.SaveChangesAsync();
            id = newOrder.Id;

            var signId = db.SignatoryOrders.Where(e => e.OrderId == order.Id).Select(e => e.SignatoryId);
            foreach (var i in signId)
            {
                SignatoryOrder performers = new SignatoryOrder() { OrderId = id, SignatoryId = i };
                db.SignatoryOrders.Add(performers);
            }

            var usersId = db.Performerses.Where(e=>e.OrderId==order.Id).Select(e=>e.UserId);
            foreach (var i in usersId)
            {
                Performers performers = new Performers() { OrderId = id, UserId = i };
                db.Performerses.Add(performers);
            }
            var dateDocuments = db.DateDocuments.Where(e => e.OrderId == order.Id).Select(e => e.DateOfDocument);
            foreach (var i in dateDocuments)
            {
                DateDocument dt = new DateDocument() { DateOfDocument = i, OrderId = id };
                db.DateDocuments.Add(dt);
            }
            db.SaveChanges();

            try
            {
                var objectLists = db.ObjectLists.Where(e => e.OrderId == order.Id).ToList();
                foreach (var l in objectLists)
                {
                    var objList = new ObjectList() { ObjectId = l.ObjectId, OrderId = id };
                    db.ObjectLists.Add(objList);
                    db.SaveChanges();
                    var values = db.ObjectValues.Where(e => e.ObjectListId == l.Id).ToList();
                    foreach (var v in values)
                    {
                        db.ObjectValues.Add(new ObjectValues() { ObjectListId = objList.Id, ObjectDeskId=v.ObjectDeskId, Value = v.Value });
                    }
                    db.SaveChanges();
                }
            }
            catch { }

            return new RedirectResult("/Order/Get?id=" + id);
        }
        public FileResult GetContract(int? orderId)
        {
            var order = db.Orders.Where(e => e.Id == orderId).FirstOrDefault();

            // Получаем массив байтов из нашего файла
            string fileName = Server.MapPath("~/Content/word/contract.docx");
            var money = new MoneyToStr("UAH", "UKR", "PER10");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);
            // Массив данных
            System.Text.StringBuilder str = new System.Text.StringBuilder("");
            var objects = db.ObjectLists.Where(e => e.OrderId == order.Id).ToList();
            List<string> objDesks = new List<string>();
            foreach (var obj in objects)
            {
                var ob = db.Objects.Where(e => e.Id == obj.ObjectId).FirstOrDefault();
                str.Append("-" + ob.Name + " ");
                var desks = db.ObjectDesces.Where(e => e.ObjectTypeId == ob.Id).ToList();
                foreach (var desk in desks)
                {
                    var value = db.ObjectValues.Where(e => e.ObjectDeskId == desk.Id && e.ObjectListId == obj.Id).FirstOrDefault();
                    str.Append(" " + value.Value + ", ");
                }
                if(str.Length>2)
                str = str.Remove(str.Length - 1, 1);
                objDesks.Add(str.ToString());
                str.Clear();
            }
            decimal sum = 0;
            var prices = db.Prices.Where(e => e.PriceListId == order.PriceListId).ToList();
            for (int j = 0; j < prices.Count; j++)
            {
                sum += prices[j].Value;
            }
            var Date = order.CreatedDate.Value;
            string month = GetMonth(Date.Month);
            string day = Date.Day <= 9 ? "0" + Date.Day : "" + Date.Day;
            string DateStr = $"\"{day}\" {month} {Date.Year}р";
            var recv = db.Propses.Where(e => e.Id == order.PropsId).FirstOrDefault();
            string Meta = db.Metas.Where(e => e.Id == order.MetaId).FirstOrDefault().Content;
            // Начинаем работу с потоком
            using (MemoryStream stream = new MemoryStream())
            {
                // Записываем в поток наш word-файл
                stream.Write(textByteArray, 0, textByteArray.Length);
                // Открываем документ из потока с возможностью редактирования
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    // Ищем все закладки в документе
                    var bookMarks = FindBookmarks(doc.MainDocumentPart.Document);
                    var client = db.Clients.Where(e => e.Id == order.ClientId).FirstOrDefault();
                    foreach (var end in bookMarks)
                    {
                        if (end.Key == "OrderId")
                        {
                            var textElement = new Text(order.Name);
                            var runElement = new Run(textElement);

                            end.Value.InsertBeforeSelf(runElement);
                        }
                        if (order.CountDays.HasValue && (end.Key == "CountDays" || end.Key == "CountDays2"))
                        {
                            var textElement = new Text(order.CountDays.Value + "");
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "IPN" || end.Key == "IPN2")
                        {
                            var textElement = new Text(client.IPN_EDRPOY);
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "ClientName" || end.Key == "ClientName2")
                        {
                            var textElement = new Text(client.FullName);
                            var runElement = new Run(textElement);
                            end.Value.InsertBeforeSelf(runElement);
                        }
                        if (end.Key == "ClientRecv")
                        {
                            var textElement = new Text(client.Props);
                            var runElement = new Run(textElement);
                            end.Value.InsertBeforeSelf(runElement);
                        }
                        if (end.Key == "DateExpert")
                        {
                            DateTime dateExpert;
                            if (order.OverWatch.HasValue)
                            {
                                dateExpert = order.OverWatch.Value.Date;
                            }
                            else {
                                dateExpert = order.CreatedDate.Value.Date;
                            }
                            string dayExpert = dateExpert.Day <= 9 ? "0" + dateExpert.Day : "" + dateExpert.Day;
                            string monthExpert = dateExpert.Month <= 9 ? "0" + dateExpert.Month : "" + dateExpert.Month;
                            string strDateExpert = dayExpert + "." + monthExpert + "." + dateExpert.Year;

                            var textElement = new Text(strDateExpert);
                            var runElement = new Run(textElement);
                            end.Value.InsertBeforeSelf(runElement);
                        }
                        if (end.Key == "Date")
                        {
                            var textElement = new Text(DateStr);

                            var runElement = new Run(textElement);

                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "Recv" || end.Key == "Recv2")
                        {
                            if (recv != null)
                            {
                                var textElement = new Text(recv.Content);
                                var runElement = new Run(textElement);
                                end.Value.InsertAfterSelf(runElement);
                            }
                        }
                        if (end.Key == "Meta")
                        {
                            var textElement = new Text(Meta);
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "Sum")
                        {
                            string Sum = money.convertValue(Convert.ToDouble(sum));

                            var textElement = new Text(sum + " (" + Sum + ")");
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "Objects")
                        {

                            var runElement = new Run();
                            runElement.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                            foreach (var tmp in objDesks)
                            {
                                runElement.AppendChild(new Text(tmp));
                                runElement.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                            }
                            runElement.LastChild.Remove();
                            end.Value.InsertAfterSelf(runElement);
                        }
                    }
                }
                var bytes = stream.ToArray();
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Договір.docx");
            }

        }
        public string GetMonth(int monthInt) {
            string month = "";
            switch (monthInt)
            {
                case 1:
                    month = "січня";
                    break;
                case 2:
                    month = "лютого";
                    break;
                case 3:
                    month = "березня";
                    break;
                case 4:
                    month = "квітня";
                    break;
                case 5:
                    month = "травня";
                    break;
                case 6:
                    month = "червня";
                    break;
                case 7:
                    month = "липня";
                    break;
                case 8:
                    month = "серпня";
                    break;
                case 9:
                    month = "вересня";
                    break;
                case 10:
                    month = "жовтня";
                    break;
                case 11:
                    month = "листопада";
                    break;
                case 12:
                    month = "грудня";
                    break;
            }
            return month;
        }
        private static Dictionary<string, BookmarkEnd> FindBookmarks(OpenXmlElement documentPart, Dictionary<string, BookmarkEnd> outs = null, Dictionary<string, string> bStartWithNoEnds = null)
        {
            if (outs == null) { outs = new Dictionary<string, BookmarkEnd>(); }
            if (bStartWithNoEnds == null) { bStartWithNoEnds = new Dictionary<string, string>(); }

            // Проходимся по всем элементам на странице Word-документа
            foreach (var docElement in documentPart.Elements())
            {
                // BookmarkStart определяет начало закладки в рамках документа
                // маркер начала связан с маркером конца закладки
                if (docElement is BookmarkStart)
                {
                    var bookmarkStart = docElement as BookmarkStart;
                    // Записываем id и имя закладки
                    bStartWithNoEnds.Add(bookmarkStart.Id, bookmarkStart.Name);
                }

                // BookmarkEnd определяет конец закладки в рамках документа
                if (docElement is BookmarkEnd)
                {
                    var bookmarkEnd = docElement as BookmarkEnd;
                    foreach (var startName in bStartWithNoEnds)
                    {
                        // startName.Key как раз и содержит id закладки
                        // здесь проверяем, что есть связь между началом и концом закладки
                        if (bookmarkEnd.Id == startName.Key)
                            // В конечный массив добавляем то, что нам и нужно получить
                            outs.Add(startName.Value, bookmarkEnd);
                    }
                }
                // Рекурсивно вызываем данный метод, чтобы пройтись по всем элементам
                // word-документа
                FindBookmarks(docElement, outs, bStartWithNoEnds);
            }

            return outs;
        }
        public FileResult GetAccount(int? OrderId)
        {
            var order = db.Orders.Where(e => e.Id == OrderId).FirstOrDefault();
            var money = new MoneyToStr("UAH", "UKR", "PER10");
            string fileName = Server.MapPath("~/Content/word/account.docx");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);
            // Массив данных    
            decimal sum = 0;
            var prices = db.Prices.Where(e => e.PriceListId == order.PriceListId).ToList();
            for (int j = 0; j < prices.Count; j++)
            {
                sum += prices[j].Value;
            }
            var day = order.CreatedDate.Value.Day <= 9 ? "0" + order.CreatedDate.Value.Day : "" + order.CreatedDate.Value.Day;
            string month = GetMonth(order.CreatedDate.Value.Month);
            string DateStr = $"\"{day}\" {month} { order.CreatedDate.Value.Year}р";
            var recv = db.Propses.Where(e => e.Id == order.PropsId).FirstOrDefault();

            System.IO.DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/Content/Upload"));
            var files = directory.GetFiles();
            foreach(var file in files)
            {
                file.Delete();
            }
            directory = new DirectoryInfo(Server.MapPath("~/Content/zips"));
            files = directory.GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }

            for (int h = 0; h < prices.Count; h++)
            {
                // Начинаем работу с потоком
                using (MemoryStream stream = new MemoryStream())
                {
                    // Записываем в поток наш word-файл
                    stream.Write(textByteArray, 0, textByteArray.Length);
                    // Открываем документ из потока с возможностью редактирования
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                    {
                        // Ищем все закладки в документе
                        var bookMarks = FindBookmarks(doc.MainDocumentPart.Document);
                        var client = db.Clients.Where(e => e.Id == order.ClientId).FirstOrDefault();
                        foreach (var end in bookMarks)
                        {
                            var runElement = new Run();
                            if (end.Key == "OrderId")
                            {
                                var textElement = new Text(order.Name + "P"+(h+1));


                                RunProperties proper = new RunProperties(new RunFonts()
                                {
                                    Ascii = "Times New Roman",
                                    ComplexScript = "Times New Roman",
                                    HighAnsi = "Times New Roman"
                                },
                                new FontSize { Val = new StringValue("28") }, new Bold { Val = new OnOffValue() { Value = true } });
                                runElement.Append(proper);
                                runElement.Append(textElement);
                                end.Value.InsertAfterSelf(runElement);

                            }

                            if (end.Key == "ClientName")
                            {
                                var textElement = new Text(client.FullName);

                                RunProperties proper = new RunProperties(new RunFonts()
                                {
                                    Ascii = "Times New Roman",
                                    ComplexScript = "Times New Roman",
                                    HighAnsi = "Times New Roman"
                                },
                                new FontSize { Val = new StringValue("28") });

                                runElement.Append(proper);
                                runElement.Append(textElement);
                                end.Value.InsertAfterSelf(runElement);
                            }
                            if (end.Key == "Date")
                            {
                                var textElement = new Text(DateStr);

                                RunProperties proper = new RunProperties(new RunFonts()
                                {
                                    Ascii = "Times New Roman",
                                    ComplexScript = "Times New Roman",
                                    HighAnsi = "Times New Roman"
                                },
                                new FontSize { Val = new StringValue("20") }, new Bold { Val = new OnOffValue() { Value = true } });

                                runElement.Append(proper);
                                runElement.Append(textElement);
                                end.Value.InsertAfterSelf(runElement);
                            }
                            if (end.Key == "Recv")
                            {
                                if (recv != null)
                                {
                                    var textElement = new Text(recv.Content);
                                    RunProperties proper = new RunProperties(new RunFonts()
                                    {
                                        Ascii = "Times New Roman",
                                        ComplexScript = "Times New Roman",
                                        HighAnsi = "Times New Roman"
                                    },
                                new FontSize { Val = new StringValue("24") });

                                    runElement.Append(proper);
                                    runElement.Append(textElement);
                                    end.Value.InsertAfterSelf(runElement);
                                }
                            }

                            if (end.Key == "Sum")
                            {
                                string Sum = money.convertValue(Convert.ToDouble(prices[h].Value));

                                var textElement = new Text(prices[h].Value + " (" + Sum + ")");
                                runElement.AppendChild(textElement);
                                var runProp = new RunProperties();

                                var runFont = new RunFonts { Ascii = "Times New Roman", ComplexScript = "Times New Roman", HighAnsi = "Times New Roman" };

                                // 48 half-point font size
                                var size = new FontSize { Val = new StringValue("28") };

                                runProp.Append(runFont);
                                runProp.Append(size);

                                runElement.PrependChild(runProp);

                                end.Value.InsertAfterSelf(runElement);
                            }
                            if (end.Key == "Table")
                            {


                                Body bod = doc.MainDocumentPart.Document.Body;
                                Table table = bod.Descendants<Table>().ToArray()[1];
                                var runProp = new RunProperties(new Bold { Val = new OnOffValue() { Value = true } });
                                var runFont = new RunFonts { Ascii = "Times New Roman", ComplexScript = "Times New Roman", HighAnsi = "Times New Roman" };
                                var size = new FontSize { Val = new StringValue("28") };

                                runProp.Append(runFont);
                                runProp.Append(size);
                                TableCellProperties tcProp = new TableCellProperties(new DocumentFormat.OpenXml.Wordprocessing.Shading()
                                {
                                    Color = "auto",
                                    Fill = "F2F2F2",
                                    Val = ShadingPatternValues.Clear
                                });
                                tcProp.TableCellMargin = new TableCellMargin() { BottomMargin = new BottomMargin() { Width = "320" } };
                                TableCellProperties tcPropEnd = new TableCellProperties(new DocumentFormat.OpenXml.Wordprocessing.Shading()
                                {
                                    Color = "auto",
                                    Fill = "D3D3D3",
                                    Val = ShadingPatternValues.Clear
                                });

                               
                                    TableRow trR = new TableRow();

                                    TableCell tcR1 = new TableCell();
                                    tcR1.Append((TableCellProperties)tcProp.Clone());
                                    var runTc1 = new Run();
                                    runTc1.AppendChild((RunProperties)runProp.Clone());
                                    runTc1.AppendChild(new Text("1"));
                                    tcR1.Append(new Paragraph(runTc1));
                                    trR.Append(tcR1);

                                    TableCell tcR2 = new TableCell();
                                    tcR2.Append((TableCellProperties)tcProp.Clone());
                                    var runTc2 = new Run();
                                    runTc2.AppendChild((RunProperties)runProp.Clone());
                                    runTc2.AppendChild(new Text(prices[h].Appointment));
                                    tcR2.Append(new Paragraph(runTc2));
                                    trR.Append(tcR2);

                                    TableCell tcR3 = new TableCell();
                                    tcR3.Append((TableCellProperties)tcProp.Clone());
                                    var runTc3 = new Run();
                                    runTc3.AppendChild((RunProperties)runProp.Clone());
                                    runTc3.AppendChild(new Text(prices[h].Value.ToString()));
                                    tcR3.Append(new Paragraph(runTc3));
                                    trR.Append(tcR3);
                                    table.AppendChild(trR);
                                
                                TableRow trEnd = new TableRow();
                                TableCell tcRAll = new TableCell(new Paragraph(new Run(new Text(""))));
                                tcRAll.Append((TableCellProperties)tcPropEnd.Clone());
                                var runtcrc2 = new Run();
                                runtcrc2.AppendChild((RunProperties)runProp.Clone());
                                runtcrc2.AppendChild(new Text("Разом"));
                                TableCell tcr2 = new TableCell(new Paragraph(runtcrc2));
                                tcr2.Append((TableCellProperties)tcPropEnd.Clone());
                                var runtcrc3 = new Run();
                                runtcrc3.AppendChild((RunProperties)runProp.Clone());
                                runtcrc3.AppendChild(new Text(prices[h].Value+""));
                                TableCell tcr3 = new TableCell(new Paragraph(runtcrc3));
                                tcr3.Append((TableCellProperties)tcPropEnd.Clone());
                                trEnd.Append(tcRAll); trEnd.Append(tcr2); trEnd.Append(tcr3);
                                table.Append(trEnd);
                            }
                        }
                    }
                    var bytes = stream.ToArray();
                    System.IO.File.WriteAllBytes(Server.MapPath("~/Content/Upload") +@"\Рахунок"+(h+1)+".docx", bytes);
                }

                  
               
               
            }
            ZipFile.CreateFromDirectory(Server.MapPath("~/Content/Upload"), Server.MapPath("~/Content/zips/files.zip"), CompressionLevel.Optimal, false);
           
            return File(Server.MapPath("~/Content/zips/files.zip"), "application/zip", "Рахунки"+order.Name+".zip");
        }
        public ActionResult DelOrder(int? OrderId)
        {
     
            var order = db.Orders.Where(e => e.Id == OrderId.Value).FirstOrDefault();

            var prices = db.Prices.Where(e => e.PriceListId == order.PriceListId).ToArray();
            db.Prices.RemoveRange(prices);
            var priceList = db.PriceLists.Where(e => e.Id == order.PriceListId).FirstOrDefault();
            if (priceList != null)
            {
                db.PriceLists.Remove(priceList);
            }
            var performerses = db.Performerses.Where(e => e.OrderId == order.Id).ToArray();
            db.Performerses.RemoveRange(performerses);
            var objects = db.ObjectLists.Where(e => e.OrderId == order.Id).ToList();
            var objectsIds = objects.Select(e => e.Id);
            var values = db.ObjectValues.Where(e => objectsIds.Contains(e.ObjectListId)).ToList();
            db.ObjectValues.RemoveRange(values);
            db.ObjectLists.RemoveRange(objects);
            var docs = db.DateDocuments.Where(e => e.OrderId == order.Id).ToList();
            db.DateDocuments.RemoveRange(docs);
            var service = GetService();
            try
            {
                service.Files.Delete(order.DirectoryId).Execute();
            }
            catch { }

            db.Orders.Remove(order);
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public FileResult GetAct(int? OrderId)
        {
     
            var order = db.Orders.Where(e => e.Id == OrderId).FirstOrDefault();
            var money = new MoneyToStr("UAH", "UKR", "PER10");
            string fileName = Server.MapPath("~/Content/word/Акт.docx");
            byte[] textByteArray = System.IO.File.ReadAllBytes(fileName);

            decimal sum = 0;
            var prices = db.Prices.Where(e => e.PriceListId == order.PriceListId).ToList();
            for (int j = 0; j < prices.Count; j++)
            {
                sum += prices[j].Value;
            }
            var Date = order.CreatedDate.Value;
            var dateOfEndWork = AddWorkDays(Date, (order.CountDays ?? 10));
            string month = GetMonth(dateOfEndWork.Month);
            string day = dateOfEndWork.Day <= 9 ? "0" + dateOfEndWork.Day : dateOfEndWork.Day + "";
            string DateStr = $"\"{day}\" {month} {dateOfEndWork.Year}р";
            var recv = db.Propses.Where(e => e.Id == order.PropsId).FirstOrDefault();
            string Meta = db.Metas.Where(e => e.Id == order.MetaId).FirstOrDefault().Content;
            // Начинаем работу с потоком
            using (MemoryStream stream = new MemoryStream())
            {
                // Записываем в поток наш word-файл
                stream.Write(textByteArray, 0, textByteArray.Length);
                // Открываем документ из потока с возможностью редактирования
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    // Ищем все закладки в документе
                    var bookMarks = FindBookmarks(doc.MainDocumentPart.Document);
                    var client = db.Clients.Where(e => e.Id == order.ClientId).FirstOrDefault();
                    foreach (var end in bookMarks)
                    {
                        if (end.Key == "IPN" || end.Key == "IPN2")
                        {
                            var textElement = new Text(client.IPN_EDRPOY);
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "ClientName" || end.Key == "ClientName2")
                        {
                            var textElement = new Text(client.FullName);
                            var runElement = new Run(textElement);
                            end.Value.InsertBeforeSelf(runElement);
                        }
                        if (end.Key == "Date")
                        {
                            var textElement = new Text(DateStr);
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "Recv")
                        {
                            if (recv != null)
                            {
                                var textElement = new Text(recv.Content);
                                var runElement = new Run(textElement);
                                end.Value.InsertAfterSelf(runElement);
                            }
                        }
                        if (end.Key == "Sum")
                        {
                            string Sum = money.convertValue(Convert.ToDouble(sum));
                            var textElement = new Text(sum + " (" + Sum + ")");
                            var runElement = new Run(textElement);
                            end.Value.InsertAfterSelf(runElement);
                        }
                        if (end.Key == "Table")
                        {
                            var runElement = new Run();
                            Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();
                            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    },
                    new BottomBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    },
                    new LeftBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    },
                    new RightBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                        new EnumValue<BorderValues>(BorderValues.BasicThinLines),
                        Size = 2
                    }
                )
            );

                            // Append the TableProperties object to the empty table.
                            table.AppendChild<TableProperties>(tblProp);
                            TableRow tr = new TableRow();

                            TableCell tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text("№"))));
                            tr.Append(tc1);
                            TableCell tc2 = new TableCell();
                            tc2.Append(new Paragraph(new Run(new Text("Вид послуги"))));
                            tr.Append(tc2);
                            TableCell tc3 = new TableCell();
                            tc3.Append(new Paragraph(new Run(new Text("Предмет послуги"))));
                            tr.Append(tc3);
                            TableCell tc4 = new TableCell();
                            tc4.Append(new Paragraph(new Run(new Text("Дата замовлення"))));
                            tr.Append(tc4);
                            TableCell tc5 = new TableCell();
                            tc5.Append(new Paragraph(new Run(new Text("Дата виконання"))));
                            tr.Append(tc5);
                            TableCell tc6 = new TableCell();
                            tc6.Append(new Paragraph(new Run(new Text("Вартість послуги"))));
                            tr.Append(tc6);
                            table.Append(tr);

                            int j;
                            for (j = 0; j < prices.Count; j++)
                            {
                                TableRow trR = new TableRow();
                                TableCell tcR1 = new TableCell(new Paragraph(new Run(new Text((j + 1) + ""))));
                                trR.Append(tcR1);
                                TableCell tcR2 = new TableCell(new Paragraph(new Run(new Text("Незалежна оцінка майна"))));
                                trR.Append(tcR2);
                                TableCell tcR3 = new TableCell(new Paragraph(new Run(new Text(prices[j].Appointment))));
                                trR.Append(tcR3);
                                string dsDay = Date.Day <= 9 ? "0" + Date.Day : Date.Day + "";
                                string dsMonth = Date.Month <= 9 ? "0" + Date.Month : Date.Month + "";
                                TableCell tcR4 = new TableCell(new Paragraph(new Run(new Text($"{dsDay}.{dsMonth}.{Date.Year}"))));
                                trR.Append(tcR4);
                                string deDay = dateOfEndWork.Day <= 9 ? "0" + dateOfEndWork.Day : "" + dateOfEndWork.Day;
                                string deMonth = dateOfEndWork.Month <= 9 ? "0" + dateOfEndWork.Month : "" + dateOfEndWork.Month;
                                TableCell tcR5 = new TableCell(new Paragraph(new Run(new Text($"{deDay}.{deMonth}.{dateOfEndWork.Year}"))));
                                trR.Append(tcR5);

                                TableCell tcR6 = new TableCell(new Paragraph(new Run(new Text(prices[j].Value + ""))));
                                trR.Append(tcR6);
                                table.Append(trR);
                            }
                            TableRow trEnd = new TableRow();
                            TableCell tcRAll = new TableCell(new Paragraph(new Run(new Text("Всього"))));
                            TableCell tcr2 = new TableCell(new Paragraph(new Run(new Text(""))));
                            TableCell tcr3 = new TableCell(new Paragraph(new Run(new Text(""))));
                            TableCell tcr4 = new TableCell(new Paragraph(new Run(new Text(""))));
                            TableCell tcr5 = new TableCell(new Paragraph(new Run(new Text(""))));
                            TableCell tcRSum = new TableCell(new Paragraph(new Run(new Text(sum + ""))));
                            trEnd.Append(tcRAll); trEnd.Append(tcr2); trEnd.Append(tcr3); trEnd.Append(tcr4); trEnd.Append(tcr5); trEnd.Append(tcRSum);
                            table.Append(trEnd);
                            runElement.AppendChild(table);
                            end.Value.InsertAfterSelf(runElement);
                        }
                    }
                }
                var bytes = stream.ToArray();
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Акт.docx");
            }

        }
        public DateTime AddWorkDays(DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }
        [HttpPost]
        public ActionResult Edit(OrderDTO model)
        {
            var objList = db.ObjectLists.Where(e => e.OrderId == model.OrderId);
            var objListId = db.ObjectLists.Where(e => e.OrderId == model.OrderId).Select(e => e.Id);
            var objValues = db.ObjectValues.Where(e => objListId.Contains(e.ObjectListId));
            db.ObjectValues.RemoveRange(objValues);
            db.ObjectLists.RemoveRange(objList);

            var order = db.Orders.Where(e => e.Id == model.OrderId).FirstOrDefault();
            order.MetaId = model.MetaId.HasValue ? model.MetaId.Value : 0;
            order.StatusId = model.StatusId;
            order.BranchId = model.BranchId;
            order.SourceId = model.SourceId;
            order.PropsId = model.ReckvId;
            order.Comments = model.Comments;

            if (model.CountDays.HasValue)
                order.CountDays = model.CountDays.Value;

            var UsersDel = db.Performerses.Where(e => e.OrderId == model.OrderId).ToList();
            db.Performerses.RemoveRange(UsersDel);

            foreach (var i in model.UsersId) {
                Performers performers = new Performers() { OrderId = model.OrderId.Value, UserId = i };
                db.Performerses.Add(performers);
            }

            var SignatoriesDel = db.SignatoryOrders.Where(e => e.OrderId == model.OrderId).ToList();
            db.SignatoryOrders.RemoveRange(SignatoriesDel);
            List<string> SignatoriesId = new List<string>();
            if (model.SignatoriesId != null)
            {
                SignatoriesId = model.SignatoriesId.Distinct().ToList();
            }
            if (SignatoriesId.Count == 0)
            { 
                foreach (var i in model.UsersId)
                {
                    SignatoryOrder performers = new SignatoryOrder() { OrderId = model.OrderId.Value, SignatoryId = i };
                    db.SignatoryOrders.Add(performers);
                }
            }
            else
            {
                foreach (var i in SignatoriesId)
                {
                    SignatoryOrder performers = new SignatoryOrder() { OrderId = model.OrderId.Value, SignatoryId = i };
                    db.SignatoryOrders.Add(performers);
                }
            }

            order.CounterpartyId = model.CounterpartyId;
            //DateOfDocument
            DateDocument date;
            if (model.DateOfDocument.HasValue && order.CreatedDate != model.DateOfDocument.Value)
            {
                date = new DateDocument() { OrderId = order.Id, DateOfDocument = model.DateOfDocument.Value };
                db.DateDocuments.Add(date);
            }
            order.DateOfPay = model.DateOfPay;

            Client newClient = new Client() { IPN_EDRPOY = model.IPN, Email = model.Email, FullName = model.ClientName, Phone = model.Tel, Props = model.Reckv };
            db.Clients.Add(newClient);

            Owner owner = new Owner() { FullName = model.OwnerName, IPN_EDRPOY = model.OwnerIPN, Email = model.OwnerEmail, Props = model.OwnerReckv, Phone = model.OwnerTel };
            db.Owners.Add(owner);
            db.SaveChanges();

            order.ClientId = newClient.Id;
            order.OwnerId = owner.Id;


            order.CommentOfTransfer = model.CommentsOfTransfer;
            order.DateOfTransfer = model.DateOfTransfer;
            order.DateOfTakeVerification = model.DateTakeVerification;
            order.DateOfEndVerification = model.DateEndVerification;
            order.DateOfDirectVerification = model.DateDirectVerification;
            order.CommentVerification = model.CommentsVerification;
            order.DateOfExpert = model.DateOfExpert;

            order.FullNameWatcher = model.Inspector;
            if (model.InspectionDate.HasValue)
                order.OverWatch = model.InspectionDate.Value;
            if (model.InspectionPrice.HasValue)
                order.PriceOverWatch = model.InspectionPrice.Value;
            order.IsPaidOverWatch = model.PaidOverWatch ?? false;

            if (model.IsPaid != null)
                if (model.IsPaid == "true")
                    order.IsPaid = true;
                else
                    order.IsPaid = false;

            PriceList list = new PriceList();
            db.PriceLists.Add(list);
            db.SaveChanges();
            order.PriceListId = list.Id;

            if (model.AppoArr != null)
                for (int i = 0; i < model.AppoArr.Length; i++)
                {
                    Price price = new Price() { PriceListId = list.Id, Value = model.PriceArr[i], Appointment = model.AppoArr[i] };
                    db.Prices.Add(price);
                }

            db.SaveChanges();
            return Content(order.Id + "");
        }
        [HttpPost]
        public ActionResult Add(OrderDTO model)
        {
            
            Order order = new Order();
         
            int orderId;
            order.MetaId = model.MetaId.HasValue ? model.MetaId.Value : 0;
            order.StatusId = model.StatusId;
            order.SourceId = model.SourceId;
            if (model.BranchId != null)
                order.BranchId = model.BranchId.Value;
            order.PropsId = model.ReckvId;
            order.Comments = model.Comments;
            order.CreatedDate = DateTime.Now.AddHours(1);
            string month = order.CreatedDate.Value.Month <= 9 ? "0" + order.CreatedDate.Value.Month : order.CreatedDate.Value.Month + "";
            var lastOrder = db.Orders.Where(e => e.CreatedDate.Value.Month == order.CreatedDate.Value.Month && e.CreatedDate.Value.Year == order.CreatedDate.Value.Year).ToList().LastOrDefault();
            int lastCount;
            if (lastOrder == null)
            {
                lastCount = 1;
            }
            else
            {
                lastCount = Convert.ToInt32(lastOrder.Name.Substring(6));
            }

            string Name = order.CreatedDate.Value.Year + "" + month + String.Format("{0:0000}", lastCount + 1);
            order.Name = Name;

            if (model.CountDays.HasValue)
                order.CountDays = model.CountDays.Value;
            
            order.DateOfExpert = model.DateOfExpert;

            db.Orders.Add(order);
            db.SaveChanges();

            orderId = order.Id;

            string folder = CreateFolder("Замовлення: " + order.Name);
            order.DirectoryId = folder;

            var usersId = model.UsersId.Distinct().ToArray();
            foreach (var i in usersId) {
                Performers performers = new Performers() { OrderId = orderId, UserId = i };
                db.Performerses.Add(performers);
            }

            List<string> SignatoriesId = new List<string>();
            if (model.SignatoriesId != null)
            {
                SignatoriesId = model.SignatoriesId.Distinct().ToList();
            }
            if (SignatoriesId.Count == 0)
            {
                foreach (var i in usersId)
                {
                    SignatoryOrder performers = new SignatoryOrder() { OrderId = orderId, SignatoryId = i };
                    db.SignatoryOrders.Add(performers);
                }
            }
            else
            {
                foreach (var i in SignatoriesId)
                {
                    SignatoryOrder performers = new SignatoryOrder() { OrderId = orderId, SignatoryId = i };
                    db.SignatoryOrders.Add(performers);
                }
            }

            order.CounterpartyId = model.CounterpartyId;

            //DateOfDocument
            DateDocument date;
            if (model.DateOfDocument.HasValue)
                date = new DateDocument() { OrderId = order.Id, DateOfDocument = model.DateOfDocument.Value };
            else
                date = new DateDocument() { OrderId = order.Id, DateOfDocument = DateTime.Now.Date };
            db.DateDocuments.Add(date);
            order.DateOfPay = model.DateOfPay;
            //Client

            Client client = new Client() { FullName = model.ClientName, IPN_EDRPOY = model.IPN, Email = model.Email, Props = model.Reckv, Phone = model.Tel };
            db.Clients.Add(client);
            db.SaveChanges();
            order.ClientId = client.Id;

            //Owner

            Owner owner = new Owner() { FullName = model.OwnerName, IPN_EDRPOY = model.OwnerIPN, Email = model.OwnerEmail, Props = model.OwnerReckv, Phone = model.OwnerTel };
            db.Owners.Add(owner);
            db.SaveChanges();
            order.OwnerId = owner.Id;

            order.CommentOfTransfer = model.CommentsOfTransfer;
            order.DateOfTransfer = model.DateOfTransfer;
            order.DateOfTakeVerification = model.DateTakeVerification;
            order.DateOfEndVerification = model.DateEndVerification;
            order.DateOfDirectVerification = model.DateDirectVerification;
            order.CommentVerification = model.CommentsVerification;
            order.FullNameWatcher = model.Inspector ?? "";
            order.OverWatch = model.InspectionDate;
            if (model.InspectionPrice.HasValue)
                order.PriceOverWatch = model.InspectionPrice.Value;


            if (model.IsPaid != null)
                if (model.IsPaid == "true")
                    order.IsPaid = true;
                else
                    order.IsPaid = false;

            PriceList list = new PriceList();
            db.PriceLists.Add(list);
            db.SaveChanges();
            order.PriceListId = list.Id;

            if (model.AppoArr != null)
            {
                for (int i = 0; i < model.AppoArr.Length; i++)
                {
                    Price price = new Price() { PriceListId = list.Id, Value = model.PriceArr[i], Appointment = model.AppoArr[i] };
                    db.Prices.Add(price);
                }
            }

            db.SaveChanges();
            return Content(order.Id + "");
        }

        public ActionResult Add()
        {
            ViewBag.Page = "Створити замовлення";
            var db = new ApplicationDbContext();
            ViewBag.Meta = db.Metas.ToList();
            ViewBag.Status = db.Statuses.ToList();
            ViewBag.Source = db.Sources.ToList();
            ViewBag.Object = db.Objects.ToList();
            ViewBag.Props = db.Propses.ToList();
            ViewBag.Branches = db.Branches.Where(e => e.SourceId == db.Sources.FirstOrDefault().Id).ToList();

            bool IsUserAdmin = User.IsInRole("Admin");
            ViewBag.IsUserAdmin = IsUserAdmin;

            var CounterpartiesId = db.Roles.Where(e => e.Name == "Counterparty").FirstOrDefault().Users.Select(e => e.UserId);
            var Counterparties = db.Users.Where(e => CounterpartiesId.Contains(e.Id)).ToList();
            ViewBag.Counterparties = Counterparties;

            ViewBag.Managers = db.Users.Where(e=> !CounterpartiesId.Contains(e.Id) && e.EmailConfirmed);

            return View();
        }
        public ActionResult Get(int Id)
        {
            ViewBag.Page = "Редагувати замовлення";
            var db = new ApplicationDbContext();
            bool IsUserAdmin = User.IsInRole("Admin");
            ViewBag.IsUserAdmin = IsUserAdmin;
            string userId = User.Identity.GetUserId();

            ViewBag.Meta = db.Metas.ToList();
            ViewBag.Status = db.Statuses.ToList();
            ViewBag.Source = db.Sources.ToList();
            ViewBag.Object = db.Objects.ToList();
            ViewBag.Props = db.Propses.ToList();

            Order result = null;
            if (IsUserAdmin)
                result = db.Orders.Where(e => e.Id == Id).FirstOrDefault();
            else
            {
                var perfomerses = db.Performerses.Where(e => string.Compare(e.UserId, userId) == 0 && e.OrderId == Id).Select(e => e.OrderId);
                result = db.Orders.Where(e => perfomerses.Contains(e.Id) && e.Id == Id).FirstOrDefault();
            }

            if (result == null)
                return new RedirectResult("/Order/Index");

            ViewBag.MetaId = result.MetaId;
            ViewBag.Id = result.Id;
            ViewBag.StatusId = result.StatusId;
            ViewBag.SourceId = result.SourceId;
            ViewBag.BranchId = result.BranchId;
            ViewBag.CountDays = result.CountDays;
            var firstSources = db.Sources.FirstOrDefault();
            ViewBag.Branches = db.Branches.Where(e => e.SourceId == (result.SourceId ?? firstSources.Id)).ToList();
            ViewBag.Users = db.Performerses.Where(e => e.OrderId == Id).Select(e => e.UserId).ToList();
            ViewBag.Client = db.Clients.Where(e => e.Id == result.ClientId).FirstOrDefault();
            ViewBag.Owner = db.Owners.Where(e => e.Id == result.OwnerId).FirstOrDefault();
            ViewBag.Overwiever = result.FullNameWatcher;
            ViewBag.DatesDocument = db.DateDocuments.Where(e => e.OrderId == result.Id).ToList();
            System.Text.StringBuilder tmpDate = new System.Text.StringBuilder("");
            if (result.OverWatch.HasValue)
            {
                tmpDate.Append(result.OverWatch.Value.Year + "");
                tmpDate.Append("-" + (result.OverWatch.Value.Month <= 9 ? "0" + result.OverWatch.Value.Month : result.OverWatch.Value.Month + ""));
                tmpDate.Append("-" + (result.OverWatch.Value.Day <= 9 ? "0" + result.OverWatch.Value.Day : result.OverWatch.Value.Day + ""));
            }
            ViewBag.DateTimeOverWatch = tmpDate;
            if (result.PriceOverWatch.HasValue)
                ViewBag.PriceOverWatch = (int)result.PriceOverWatch.Value;
            ViewBag.IsPaidOverWatch = result.IsPaidOverWatch;

            ViewBag.Comments = result.Comments;
            ViewBag.IsPaid = result.IsPaid;

            if (result.PropsId != null)
                ViewBag.PropsId = result.PropsId.Value;
            else
                ViewBag.PropsId = null;

            var prices = db.Prices.Where(e => e.PriceListId == result.PriceListId).ToList();
            ViewBag.Payments = prices;
            int sum = 0;
            for (int i = 0; i < prices.Count; i++)
            {
                sum += (int)prices[i].Value;
            }
            ViewBag.Sum = sum;

            var objList = db.ObjectLists.Where(e => e.OrderId == Id);
            var objListId = db.ObjectLists.Where(e => e.OrderId == Id).Select(e => e.ObjectId);
            var objTypes = db.Objects.Where(e => objListId.Contains(e.Id));
            ViewBag.objTypes = objTypes.ToList();
            ViewBag.objList = objList.ToList();

            var service = GetService();
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Q = $"'{result.DirectoryId}' in parents";
            listRequest.Fields = "nextPageToken, files(id, name)";
            try
            {
                IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                    .Files;
                ViewBag.Files = files;
            }
            catch
            {
                ViewBag.Files = new List<Google.Apis.Drive.v3.Data.File>();
            }

            var CounterpartiesId = db.Roles.Where(e => e.Name == "Counterparty").FirstOrDefault().Users.Select(e => e.UserId);
            var Counterparties = db.Users.Where(e => CounterpartiesId.Contains(e.Id)).ToList();
            ViewBag.Counterparties = Counterparties;
            ViewBag.CounterpartyId = result.CounterpartyId;

            ViewBag.Managers = db.Users.Where(e => !CounterpartiesId.Contains(e.Id) && e.EmailConfirmed);
            ViewBag.Signatories = db.SignatoryOrders.Where(e=>e.OrderId==result.Id).Select(e=>e.SignatoryId).ToList();
            return View(result);
        }
        public ActionResult CreateObjList(int OrderId, int ObjId)
        {
            var db = new ApplicationDbContext();
            ObjectList list = new ObjectList() { ObjectId = ObjId, OrderId = OrderId };
            db.ObjectLists.Add(list);
            db.SaveChanges();
            return Content(list.Id + "");
        }
        public ActionResult ObjectValues(int ObjListId, int ObjectDeskId, string Value)
        {
            var db = new ApplicationDbContext();
            ObjectValues values = new ObjectValues() { ObjectDeskId = ObjectDeskId, ObjectListId = ObjListId, Value = Value };
            db.ObjectValues.Add(values);
            db.SaveChanges();
            return Content("Success");
        }
        public JsonResult GetBranches(int SourceId)
        {
            var db = new ApplicationDbContext();
            var result = db.Branches.Where(e => e.SourceId == SourceId).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetObjectType(int Id)
        {
            var db = new ApplicationDbContext();
            var result = db.ObjectDesces.Where(e => e.ObjectTypeId == Id).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string CreateFolder(string name)
        {
            var service = GetService();
            var list = service.Files.List();
            list.Q = "name contains 'CRM'";
            var files = list.Execute();
            string Directory = "1BMCcePzGKZ4XiRgGkmh2eh4ZuI3o16EV";
            if (files.Files.Count >= 1)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = name,
                    Parents = new List<string>
                    {
                       Directory
                    }
                    ,
                    MimeType = "application/vnd.google-apps.folder"
                };



                var request = service.Files.Create(fileMetadata);
                request.Fields = "id,name";
                var f = request.Execute();
                return f.Id;
            }
            else
            {
                var fileMetadataCRM = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = "CRM",
                    MimeType = "application/vnd.google-apps.folder"

                };
                var requestCRM = service.Files.Create(fileMetadataCRM);
                requestCRM.Fields = "id";
                var fileCRM = requestCRM.Execute();

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = name,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string>
                    {
                       fileCRM.Id
                    }
                };
                var request = service.Files.Create(fileMetadata);
                request.Fields = "id";
                var file = request.Execute();

                return file.Id;
            }

        }
        public JsonResult UploadFile(int Order)
        {
            var service = GetService();
            var db = new ApplicationDbContext();
            string folderId = db.Orders.Where(e => e.Id == Order).FirstOrDefault().DirectoryId;


            var file = this.Request.Files[0] as HttpPostedFileBase;

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                Parents = new List<string>
                    {
                       folderId
                    }
            };

            FilesResource.CreateMediaUpload request;

            request = service.Files.Create(fileMetadata, file.InputStream, "");
            request.Fields = "id,name";
            request.Upload();

            return Json(new { Id = request.ResponseBody.Id, Name = request.ResponseBody.Name }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFolder(int Order)
        {
            var service = GetService();
            var db = new ApplicationDbContext();
            var fileId = db.Orders.Where(e => e.Id == Order).Select(e => e.DirectoryId).FirstOrDefault();
            var file = service.Files.Get(fileId).Execute();

            return new RedirectResult("https://drive.google.com/drive/u/0/folders/" + fileId);
        }
        public ActionResult DeleteFile(string FileId)
        {
            var service = GetService();
            service.Files.Delete(FileId).Execute();
            return Content("Success");
        }
        public ActionResult Search(string Value)
        {
            var db = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            var perfomerses = db.Performerses.Where(e => string.Compare(e.UserId, userId) == 0).Select(e => e.OrderId);
            List<Order> allOrders;
            if (User.IsInRole("Admin"))
            {
                allOrders = db.Orders.ToList();
            }
            else
            {
                allOrders = db.Orders.Where(e => perfomerses.Contains(e.Id)).ToList();
            }
            var result = new List<Order>();



            var Ids = allOrders.Where(e => e.Name == Value).ToList();
            if (Ids != null)
            {
                result.AddRange(Ids);
            }


            var Metas = db.Metas.Where(e => e.Content.Contains(Value) == true).FirstOrDefault();
            if (Metas != null)
            {
                var tmp = allOrders.Where(e => e.MetaId == Metas.Id);
                result.AddRange(tmp);
            }

            var Sources = db.Sources.Where(e => e.SourceName.Contains(Value) == true).FirstOrDefault();
            if (Sources != null)
            {
                var tmp = allOrders.Where(e => e.SourceId == Sources.Id);
                result.AddRange(tmp);
            }

            var Branches = db.Branches.Where(e => e.BranchName.Contains(Value) == true).FirstOrDefault();
            if (Branches != null)
            {
                var tmp = allOrders.Where(e => e.BranchId == Branches.Id);
                result.AddRange(tmp);
            }

            var Statuses = db.Statuses.Where(e => e.Content.Contains(Value) == true).FirstOrDefault();
            if (Statuses != null)
            {
                var tmp = allOrders.Where(e => e.StatusId == Statuses.Id);
                result.AddRange(tmp);
            }

            var Clients = db.Clients.Where(e => e.IPN_EDRPOY.Contains(Value) || e.FullName.Contains(Value) || e.Email.Contains(Value) || e.Phone.Contains(Value) || e.Props.Contains(Value)).Select(e => e.Id);
            if (Clients != null)
            {
                var tmp = allOrders.Where(e => Clients.Contains(e.ClientId));
                result.AddRange(tmp);
            }
            return View("Index", result);
        }
        public FileResult GetReports(DateTime? date1, DateTime? date2)
        {
            if (date1 > date2)
            {
                var tmp = date1.Value;
                date1 = date2.Value;
                date2 = tmp;
            }
            date2 = date2.Value.AddDays(1);

            var userId = User.Identity.GetUserId();
            var ordersIds = new List<int>();
            var orders = new List<Order>();
            var roleId = db.Roles.Where(e => e.Name == "Admin").FirstOrDefault().Users.FirstOrDefault();
            if (roleId.UserId==userId)//Is it a Admin
            {
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2).ToList();
            }
            else
            {
                var perfomers = db.Performerses.Where(e => e.UserId.Contains(userId)).Select(e => e.OrderId).ToList();
                    orders = db.Orders.Where(e => e.CreatedDate >= date1 && e.CreatedDate <= date2 && perfomers.Contains(e.Id)).ToList();
            }
            ordersIds = orders.Select(e => e.Id).ToList();

            byte[] arr = new AnalyticsController().GetReport(ordersIds);
            return File(arr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Персональний звіт.xlsx");
        }
       
    }
}