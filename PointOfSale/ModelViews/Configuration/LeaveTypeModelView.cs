using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Configuration
{
    public class LeaveTypeModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please Enter Leave Type Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("LeaveTypeNameExists", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "your entered Name already exists..!!!")]
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
    }
}