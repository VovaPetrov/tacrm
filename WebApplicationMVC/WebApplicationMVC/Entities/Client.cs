using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string IPN_EDRPOY { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Props { get; set; }
    }
}