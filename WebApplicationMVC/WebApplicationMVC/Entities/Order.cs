using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplicationMVC.Models;
using WebApplicationMVC.Entities;
namespace WebApplicationMVC.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? MetaId { get; set; }
        public Meta Meta { get; set; }
        public bool IsLegalClient { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DirectoryId { get; set; }
        public string Comments { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public int? SourceId { get; set; }
        public Source Source { get; set; }
        public bool? IsPaid { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public int? PriceListId { get; set; }
        public PriceList PriceList { get; set; }
        public int? PropsId { get; set; }
        public Props Props { get; set; }
        public int? CountDays { get; set; }
        public DateTime? DateOfPay { get; set; }
        public DateTime? DateOfTransfer { get; set; }
        public string CommentOfTransfer { get; set; }
        public DateTime? DateOfTakeVerification { get; set; }
        public DateTime? DateOfDirectVerification { get; set; }
        public DateTime? DateOfEndVerification { get; set; }
        public string CommentVerification { get; set; }
        public DateTime? DateOfExpert { get; set; }
        public string CounterpartyId { get; set; }
        public ApplicationUser Counterparty { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
    }
}