using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class UserRegisterModelView
    {
        public int? Id { get; set; }
        [Display(Name="First Name")]
        [Required(ErrorMessage="Plase Enter First Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Plase Enter Last Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string LastName { get; set; }
        [Display(Name="Username")]
        [Required(ErrorMessage="Please Enter Username")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("UserNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Username name already exists..!!!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(4, MinimumLength = 4, ErrorMessage="Password must be 4 characters long")]
        [DataType(DataType.Password)]
        [Remote("PasswordExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This password already exists..!!!")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "ID")]
        [Required(ErrorMessage = "Please Enter ID")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string UserId { get; set; }
    }
}