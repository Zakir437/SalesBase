namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewStockWaste")]
    public partial class ViewStockWaste
    {
        [Key]
        public long StockWasteId { get; set; }

        [StringLength(50)]
        public string WasteVoucherNo { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }
    }
}