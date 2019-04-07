using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Models { 
    public class PayingDTO
    {
        public int?[] IdArr { get; set; }
        public bool[] IsPaidArr { get; set; }
        public DateTime?[] PayDateArr { get; set; }
        public string[] AppoArr { get; set; }
        public decimal[] PriceArr { get; set; }
    }
}