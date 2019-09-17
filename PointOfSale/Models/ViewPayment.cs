namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViewPayment")]
    public partial class ViewPayment
    {
        [Key]
        [Column(Order = 0)]
        public long PaymentTransactionId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PaymentId { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string TransactionNo { get; set; }

        public bool? IsCreditPayment { get; set; }

        [StringLength(50)]
        public string OrderNumber { get; set; }

        [StringLength(50)]
        public string ImportNumber { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Type { get; set; }

        public decimal? OrderDueAmount { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MethodId { get; set; }

        [StringLength(50)]
        public string ReturnImportNumber { get; set; }

        [Key]
        [Column(Order = 5)]
        public bool InOut { get; set; }

        [StringLength(50)]
        public string SalaryTransNo { get; set; }

        [StringLength(50)]
        public string PaySheetTransNo { get; set; }

        public bool? Status { get; set; }

        [StringLength(50)]
        public string PaymentBodyName { get; set; }

        [StringLength(50)]
        public string PaymentTypeName { get; set; }

        public int? CashRefundType { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentBodyId { get; set; }

        public long? CreditPaymentId { get; set; }

        [StringLength(50)]
        public string CreditVoucherNo { get; set; }

        [Key]
        [Column(Order = 7)]
        public DateTime Date { get; set; }

        [StringLength(50)]
        public string RefOrderNumber { get; set; }

        public long? RefOrderId { get; set; }

        public long? SupplierId { get; set; }

        public long? CustomerId { get; set; }

        public long? AssociateId { get; set; }

        public long? ReturnImportId { get; set; }

        [StringLength(50)]
        public string WarehouseVoucher { get; set; }
    }
}
