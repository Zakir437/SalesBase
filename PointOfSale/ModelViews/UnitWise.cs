using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class UnitWise
    {
        [Required(ErrorMessage ="Please enter weight/price")]
        public decimal? UnitAmount { get; set; }
    }
}