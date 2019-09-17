using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class CheckModel
    {
        [Required(ErrorMessage = "Please select a terminal")]
        public int? CheckBodyId { get; set; }
        public string CheckTransactionNo { get; set; }
        [Required(ErrorMessage = "Please enter pay amount")]
        public decimal? CheckAmount { get; set; }
    }
}