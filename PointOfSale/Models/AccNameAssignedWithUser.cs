namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccNameAssignedWithUser")]
    public partial class AccNameAssignedWithUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int UserType { get; set; }

        public int AccId { get; set; }

        public int AssignedBy { get; set; }

        public DateTime AssignedDate { get; set; }
    }
}
