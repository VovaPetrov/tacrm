﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplicationMVC.Entities
{
    public class OrderOwner
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int OwnerId { get; set; }
    }
}