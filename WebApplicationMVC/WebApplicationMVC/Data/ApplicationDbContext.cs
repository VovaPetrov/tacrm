using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplicationMVC.Models;

namespace WebApplicationMVC
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<Props> Propses { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ObjectType> Objects { get; set; }
        public DbSet<ObjectList> ObjectLists { get; set; }
        public DbSet<ObjectDesc> ObjectDesces { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Performers> Performerses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Dialog> Dialogs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<TaskElement> TaskElements { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<ObjectValues> ObjectValues { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<DateDocument> DateDocuments { get; set; }
        public DbSet<Unread> Unread { get; set; }
        public DbSet<AnalyticsCounterparty> AnalyticsCounterparties { get; set; }
        public DbSet<SignatoryOrder> SignatoryOrders { get; set; }
        public DbSet<PerformerCounterparty> PerformerCounterparties { get; set; }
        public DbSet<SourceCounterparty> SourceCounterparties { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}