using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Sales
{
    public class RestrictionModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter restriction name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("RestrictionNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "Restriction name already exist !!!")]
        public string Name { get; set; }
        public int CreatedBy { get; set; }
    }
}