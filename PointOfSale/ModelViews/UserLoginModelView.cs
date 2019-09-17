using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class UserLoginModelView
    {
        public int UserLoginId { get; set; }
        public int UserId { get; set; }
        [Remote("UserNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Username name already exists..!!!")]
        [Required(ErrorMessage="Please Enter Username")]
        public string Username { get; set; }
        [Required(ErrorMessage="Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}