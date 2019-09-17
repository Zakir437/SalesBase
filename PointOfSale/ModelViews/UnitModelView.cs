using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class UnitModelView
    {
        public int? UnitId { get; set; }
        [Required(ErrorMessage = "Please Enter Unit Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("UnitNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Unit Name Already Exist !!!")]
        public string UnitName { get; set; }
        public int? CreatedBy { get; set; }
        public Nullable<int> Status { get; set; }

    }
}