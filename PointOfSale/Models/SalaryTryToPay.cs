namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalaryTryToPay")]
    public partial class SalaryTryToPay
    {
        public long Id { get; set; }

        public long EmpSalaryPaymentId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }
    }
}
