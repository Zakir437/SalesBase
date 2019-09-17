namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewIndentItem")]
    public partial class ViewIndentItem
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IndentId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestQuantity { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReceiveQuantity { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RemainingQuantity { get; set; }

        public decimal? Price { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [StringLength(100)]
        public string Comment { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public bool? ChangeRequest { get; set; }

        public int? ChangeQty { get; set; }

        public int? IndentStatus { get; set; }

        public int? ProcurementStatus { get; set; }

        [StringLength(50)]
        public string IndentVoucher { get; set; }
    }
}
