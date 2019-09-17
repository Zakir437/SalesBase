using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Configuration
{
    public class DebitLimitModelView
    {
        public long? Id { get; set; }
        public long CustomerId { get; set; }
        [Required(ErrorMessage = "Please enter debit limit")]
        public decimal Limit { get; set; }
        public decimal Amount { get; set; }
        public bool? IsAmountEdit { get; set; }
        public int CreatedBy { get; set; }
    }
}