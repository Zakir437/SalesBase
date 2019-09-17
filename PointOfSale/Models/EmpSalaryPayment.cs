namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EmpSalaryPayment")]
    public partial class EmpSalaryPayment
    {
        public long Id { get; set; }

        [StringLength(50)]
        public string TransactionNo { get; set; }

        public int UserId { get; set; }

        public decimal ActualSalary { get; set; }

        public decimal TotalAmount { get; set; }

        public int TotalPresent { get; set; }

        public int TotalPaidLeave { get; set; }

        public int TotalWorkingDays { get; set; }

        public int TotalHoliday { get; set; }

        public decimal DueAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public int Status { get; set; }

        public int? PaidBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PaidDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime GeneratedDate { get; set; }

        public int GenerateBy { get; set; }

        public int? AssignUserId { get; set; }

        public int? UpdatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UpdatedDate { get; set; }
    }
}
