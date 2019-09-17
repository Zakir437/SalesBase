namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewPurchaseOrderTransaction")]
    public partial class ViewPurchaseOrderTransaction
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PurchaseId { get; set; }

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
        public int RequestQty { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReceiveQty { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RemainingQty { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        public int? PurchaseStatus { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        public bool? IsUniqueItem { get; set; }

        public bool? IsUnitWise { get; set; }

        public decimal? ProductPrice { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? ChangeQty { get; set; }

        public bool? ChangeRequest { get; set; }

        [StringLength(101)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public decimal? PeritemCost { get; set; }

        public decimal? Cost { get; set; }
    }
}
