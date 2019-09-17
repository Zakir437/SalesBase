using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class StockModel
    {
        public int? StockId { get; set; }
        public Nullable<int> ProductId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Required(ErrorMessage = "Please enter quantity of Product")]
        public Nullable<decimal> Quantity { get; set; }
    }
}