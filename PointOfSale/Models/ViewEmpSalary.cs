namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewEmpSalary")]
    public partial class ViewEmpSalary
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal TotalAmount { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TotalPresent { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TotalPaidLeave { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TotalWorkingDays { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TotalHoliday { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal DueAmount { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal PaidAmount { get; set; }

        public int? PaidBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PaidDate { get; set; }

        [Key]
        [Column(Order = 8, TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Key]
        [Column(Order = 9, TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Key]
        [Column(Order = 10, TypeName = "date")]
        public DateTime GeneratedDate { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [Key]
        [Column(Order = 12)]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Key]
        [Column(Order = 13)]
        [StringLength(50)]
        public string LastName { get; set; }

        public string Picture { get; set; }

        [Key]
        [Column(Order = 14)]
        public decimal ActualSalary { get; set; }

        public int? AssignUserId { get; set; }

        [StringLength(50)]
        public string TransactionNo { get; set; }

        [Key]
        [Column(Order = 15)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }
    }
}
