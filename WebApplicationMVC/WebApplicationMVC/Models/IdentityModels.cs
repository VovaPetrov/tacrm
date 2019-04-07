using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebApplicationMVC.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Tel { get; set; }
    }

    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Comments { get; set; }
    }
    public class Member
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DialogId { get; set; }
        public bool IsEnable { get; set; }
    }
    public class Unread
    {
        [Key]
        public int Id { get; set; }
        public int DialogId { get; set; }
        public string UserId { get; set; }
    }
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public int DialogId { get; set; }
        public DateTime dateTime { get; set; }
        public string SenderG { get; set; }
    }
    public class Dialog
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class TaskElement
    {
        [Key]
        public int Id { get; set; }
        public int MissionId { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; }
    }
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? MetaId { get; set; }
        public int ClientId { get; set; }
        public int OwnerId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DirectoryId { get; set; }
        public string Comments { get; set; }
        public int? BranchId { get; set; }
        public int? SourceId { get; set; }
        public bool? IsPaid { get; set; }
        public int? StatusId { get; set; }
        public DateTime? OverWatch { get; set; }
        public string FullNameWatcher { get; set; }
        public bool IsPaidOverWatch { get; set; }
        public decimal? PriceOverWatch { get; set; }
        public int? PriceListId { get; set; }
        public int? PropsId { get; set; }
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
    }
    public class SignatoryOrder
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string SignatoryId { get; set; }
    }
    public class DateDocument
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime? DateOfDocument { get; set; }
    }
    public class Status
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
    }
    public class Meta
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
    }
    public class PriceList
    {
        [Key]
        public int Id { get; set; }
    }
    public class Price
    {
        [Key]
        public int Id { get; set; }
        public decimal Value { get; set; }
        public string Appointment { get; set; }
        public int PriceListId { get; set; }
        [DefaultValue("false")]
        public bool IsPaid { get; set; }
        public DateTime? Date { get; set; }
    }
    public class Props
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
    }
    public class Performers
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }

    }
    public class Source
    {
        [Key]
        public int Id { get; set; }
        public string SourceName { get; set; }
    }
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string BranchName { get; set; }
    }
    public class Owner
    {
        [Key]
        public int Id { get; set; }
        public string IPN_EDRPOY { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Props { get; set; }
    }
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
    public class ObjectType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ObjectDesc
    {
        [Key]
        public int Id { get; set; }
        public int ObjectTypeId { get; set; }
        public string Name { get; set; }
    }
    public class ObjectValues
    {
        [Key]
        public int Id { get; set; }
        public int ObjectListId { get; set; }
        public int ObjectDeskId { get; set; }
        public string Value { get; set; }
    }
    public class ObjectList
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ObjectId { get; set; }

    }
    public class AnalyticsCounterparty
    {
        [Key]
        public int Id { get; set; }
        public string CounterpartyId { get; set; }
        public int AnalyticsId { get; set; }
    }
    public class PerformerCounterparty
    {
        [Key]
        public int Id { get; set; }
        public string CounterpartyId { get; set; }
        public string PerformerId { get; set; }
    }
    public class SourceCounterparty
    {
        [Key]
        public int Id { get; set; }
        public string CounterpartyId { get; set; }
        public int SourceId { get; set; }
    }
}