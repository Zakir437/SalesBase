namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewWasteProduct")]
    public partial class ViewWasteProduct
    {
        [Key]
        [Column(Order = 0)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long WasteId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string ProductName { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DistributeId { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Quantity { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WasteTypeId { get; set; }

        public long? ASReffId { get; set; }

        public long? WHReffId { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }

        [StringLength(50)]
        public string WasteReffNo { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        [StringLength(50)]
        public string WasteTypeName { get; set; }

        [StringLength(50)]
        public string ASReffNo { get; set; }

        [StringLength(103)]
        public string WHReffNo { get; set; }

        public int? Status { get; set; }

        public long? SupplierId { get; set; }

        [StringLength(50)]
        public string Supplier { get; set; }

        public int? Type { get; set; }
    }
}
