using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Models
{
    public class TypeReport
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }
    public class AnalyticsModel
    {
        public static TypeReport[] GetTypeReports() {
            TypeReport[] types = {
                new TypeReport{Id=0,TypeName="Вся інформація" },
                new TypeReport { Id=1,TypeName= "Укргазбанк" },
                new TypeReport{ Id=2,TypeName= "Кредобанк" },
                new TypeReport {Id=3, TypeName = "Нотаріус Тарас" } };
            return types;
        }
    }
}