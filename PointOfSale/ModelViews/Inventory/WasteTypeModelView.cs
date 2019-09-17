using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Inventory
{
    public class WasteTypeModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter type name")]
        [Remote("WasteTypeExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This name already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string TypeName { get; set; }
        [Required(ErrorMessage ="Please select type")]
        public int Type { get; set; }
        public int CreatedBy { get; set; }
    }
}