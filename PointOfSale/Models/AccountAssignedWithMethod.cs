namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccountAssignedWithMethod")]
    public partial class AccountAssignedWithMethod
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int MethodId { get; set; }

        public int AssignedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime AssignedDate { get; set; }
    }
}
