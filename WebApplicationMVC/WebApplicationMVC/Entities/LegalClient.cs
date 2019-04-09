using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Entities
{
    public class LegalClient
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string EDRPOY { get; set; }
        public string LegalAddress { get; set; }
        public string Position { get; set; }
        public string SubscriberName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}