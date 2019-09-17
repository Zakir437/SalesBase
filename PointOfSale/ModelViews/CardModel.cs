using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class CardModel
    {
        [Required(ErrorMessage ="Please select a card")]
        public int? PaymentBodyId { get; set; }
        //[Required(ErrorMessage ="Please enter transaction number")]
        public string TransactionNo  { get; set; }
        [Required(ErrorMessage = "Please enter pay amount")]
        public decimal? CardPaid { get; set; }
    }
}