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
namespace WebApplicationMVC.Models
{
    public class CalendarModel
    {
        public CalendarService GetService()
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";

            UserCredential credential;


            using (var stream =
                     new FileStream(HttpContext.Current.Server.MapPath(("~/Content/API/credentials.json")), FileMode.Open, FileAccess.Read))
            {
                String FilePath = HttpContext.Current.Server.MapPath(("~/Content/API/CalendarServiceCredentials.json"));

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "admin",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            // Create Drive API service.
            var service = new CalendarService(new CalendarService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,

            });

            return service;
        }
    }
}