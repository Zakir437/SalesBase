using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Inventory
{
    public class DeliveryChargeModelView
    {
        public int? Id { get; set; }
        public int? FromPrice { get; set; }
        public int? ToPrice { get; set; }
        public bool IsParcentile { get; set; }
        [Required(ErrorMessage = "Please enter amount.")]
        public int Amount { get; set; }
        public int CreatedBy { get; set; }
    }
}