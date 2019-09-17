namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkOrderItem")]
    public partial class WorkOrderItem
    {
        public long Id { get; set; }

        public long WorkOrderId { get; set; }

        public long IndentId { get; set; }

        public int ProductId { get; set; }

        public long DistributeId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public int RequestQty { get; set; }

        public int ReceiveQty { get; set; }

        public int RemainingQty { get; set; }

        public bool? ChangeRequest { get; set; }

        public int? ChangeQty { get; set; }

        public decimal PerItemPrice { get; set; }

        public decimal Price { get; set; }

        [StringLength(100)]
        public string Comment { get; set; }

        public int Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
