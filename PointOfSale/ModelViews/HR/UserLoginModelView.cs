using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.HR
{
    public class UserLoginModelView
    {
        public int UserLoginId { get; set; }
        public int UserId { get; set; }
        [Remote("IsUserNameExists", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Username name already exists..!!!")]
        [Required(ErrorMessage="Please Enter Username")]
        public string Username { get; set; }
        [Required(ErrorMessage="Please Enter Password")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]{8,}$", ErrorMessage = "Password should be minimum eight characters, at least one uppercase letter and one number")]
        public string Password { get; set; }
    }
}