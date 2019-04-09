using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Entities
{
    public class Overwatch
    {
        [Key]
        public int Id { get; set; }
        public DateTime? OverWatch { get; set; }
        public string FullNameWatcher { get; set; }
        public bool IsPaidOverWatch { get; set; }
        public decimal? PriceOverWatch { get; set; }
    }
}