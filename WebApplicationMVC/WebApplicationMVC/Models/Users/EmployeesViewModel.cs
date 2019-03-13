using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Models
{
    public class EmployeesViewModel
    {
        public IEnumerable<ApplicationUser> Managers { get; set; }
        public IEnumerable<ApplicationUser> Counterparties { get; set; }
    }
}