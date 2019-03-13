using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Models.Counterparty
{
    public class CounterpartyDTO
    {
        public ApplicationUser Counterparty { get; set; }
        public List<int> AnalyticsId { get; set; }
        public List<string> PerformersId { get; set; }
        public List<int> SourcesId { get; set; }
    }
}