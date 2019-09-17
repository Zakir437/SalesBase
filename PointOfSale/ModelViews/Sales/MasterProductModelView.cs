using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Sales
{
    public class MasterProductModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please enter name")]
        //[Remote("SubProductNameExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This Number already exists..!!!")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Name { get; set; }
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Please enter brand name")]
        public string Brand { get; set; }
        public int? ProductId { get; set; }
        public string QuantityOrName { get; set; }
        public int? MasterUnit { get; set; }
        public int CreatedBy { get; set; }
    }
}