using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace WebApplicationMVC.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Значення {0} має містити символів не менше: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новий пароль і його підтвердження не збігаються.")]
        public string ConfirmPassword { get; set; }
    }    
}