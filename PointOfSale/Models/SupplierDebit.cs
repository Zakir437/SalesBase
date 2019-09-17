namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SupplierDebit")]
    public partial class SupplierDebit
    {
        public long Id { get; set; }

        public long SupplierId { get; set; }

        public decimal Amount { get; set; }
    }
}
