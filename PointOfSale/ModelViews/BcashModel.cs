using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class BcashModel
    {
        [Required(ErrorMessage = "Please select a number")]
        public int? bodyId { get; set; }
        //[Required(ErrorMessage ="Please enter transaction number")]
        public string BkashTransactionNo { get; set; }
        [Required(ErrorMessage = "Please enter pay amount")]
        public decimal? BcashPaid { get; set; }
    }
}