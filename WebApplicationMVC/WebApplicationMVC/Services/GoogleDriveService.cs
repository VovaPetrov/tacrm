using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace WebApplicationMVC.Services
{
    public class GoogleDriveService
    {
        HttpServerUtilityBase Server { get; set; }
        public GoogleDriveService(HttpServerUtilityBase server)
        {
            Server = server;
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
    }
}