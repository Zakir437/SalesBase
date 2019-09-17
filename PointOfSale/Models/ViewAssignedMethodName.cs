namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewAssignedMethodName")]
    public partial class ViewAssignedMethodName
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "date")]
        public DateTime AssignedDate { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string MethodName { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MethodId { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(50)]
        public string AccountName { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InOut { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(50)]
        public string PaymentTypeName { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentTypeId { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Type { get; set; }

        public decimal? Balance { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
