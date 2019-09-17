using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class SupplierModel
    {
        public long? SupplierId { get; set; }
        [Required(ErrorMessage = "Please enter supplier Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter e-mail address")]
        [Remote("SupplierEmailExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "your entered email already exists..!!!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter phone no.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Phone { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Required(ErrorMessage ="Please enter supplier since date")]
        [DataType(DataType.Date,ErrorMessage ="Please enter a valid date")]
        public DateTime? SupplierSince { get; set; }
        public int CreatedBy { get; set; }
    }
}