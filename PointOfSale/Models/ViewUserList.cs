namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ViewUserList
    {
        [Key]
        [Column(Order = 0)]
        public int UserId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Gender { get; set; }

        [StringLength(100)]
        public string Religion { get; set; }

        [StringLength(100)]
        public string Nationalitiy { get; set; }

        public string Picture { get; set; }

        public string PictureOriginalName { get; set; }

        [StringLength(100)]
        public string NationalId { get; set; }

        public string NationalIdBackImg { get; set; }

        public string NationalIdFontImg { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(500)]
        public string EmailAddress { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(50)]
        public string MobileNo { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(50)]
        public string ParAddress { get; set; }

        [StringLength(50)]
        public string ParAddressLine1 { get; set; }

        [StringLength(100)]
        public string ParCountry { get; set; }

        [StringLength(50)]
        public string ParDivision { get; set; }

        [StringLength(50)]
        public string ParState { get; set; }

        [StringLength(50)]
        public string ParCity { get; set; }

        [StringLength(50)]
        public string ParArea { get; set; }

        [StringLength(50)]
        public string ParPotalCode { get; set; }

        [Key]
        [Column(Order = 7)]
        public bool SamePresentAddress { get; set; }

        [StringLength(50)]
        public string PreAddress { get; set; }

        [StringLength(50)]
        public string PreAddressLine1 { get; set; }

        [StringLength(100)]
        public string PreCountry { get; set; }

        [StringLength(50)]
        public string PreDivision { get; set; }

        [StringLength(50)]
        public string PreState { get; set; }

        [StringLength(50)]
        public string PreCity { get; set; }

        [StringLength(50)]
        public string PreArea { get; set; }

        [StringLength(50)]
        public string PrePostalCode { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Status { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime CreatedDate { get; set; }

        [StringLength(101)]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(101)]
        public string UpdatedBy { get; set; }

        public int? UserType { get; set; }

        public int? WorkingScheduleId { get; set; }

        public int? SalaryPackageId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? JoinDate { get; set; }

        public int? HolidayPackId { get; set; }
    }
}
