using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Sales
{
    public class ProductDistributeItems
    {
        public long? Id { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public decimal? Price { get; set; }
        public string BarCode { get; set; }
        public decimal? Cost { get; set; }
        public string SizeName { get; set; }
        public string Code { get; set; }
        public string Plu { get; set; }
        public int? MQuantity { get; set; }
    }
}