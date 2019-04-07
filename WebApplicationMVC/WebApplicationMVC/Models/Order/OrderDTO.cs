using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplicationMVC.Models
{
    public class OrderDTO
    {
        public int? OrderId { get; set; }
        public int? MetaId {get;set;}
        public int StatusId {get;set;}
        public int SourceId {get;set;}
        public int? BranchId {get;set;}
        public int? CountDays {get;set;}
        public DateTime? DateOfPay {get;set;}
        public string[] UsersId {get;set;}     
        public string[] SignatoriesId { get; set; }
        public string CounterpartyId { get; set; }
        public DateTime? DateOfDocument {get;set;}
        public string ClientName { get; set; }
        public string Tel {get;set;}
        public string IPN {get;set;}
        public string Email {get;set;}
        public string Reckv {get;set;}
        public string OwnerName {get;set;}
        public string OwnerTel  {get;set;}
        public string OwnerIPN  {get;set;}
        public string OwnerEmail {get;set;}
        public string OwnerReckv {get;set;}
        public int ReckvId {get;set;}
        public string Comments {get;set;}
        public string Inspector {get;set;}
        public DateTime? InspectionDate {get;set;}
        public bool? PaidOverWatch {get;set;}
        public decimal? InspectionPrice {get;set;}
        public string IsPaid {get;set;}
        public string CommentsOfTransfer {get;set;}
        public DateTime? DateOfTransfer {get;set;}
        public DateTime? DateTakeVerification {get;set;}
        public DateTime? DateDirectVerification {get;set;}
        public DateTime? DateEndVerification {get;set;}
        public string CommentsVerification {get;set;}
        public PayingDTO PayingInfo { get; set; }
        public DateTime? DateOfExpert {get;set;}
    }
}