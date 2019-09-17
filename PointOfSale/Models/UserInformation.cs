namespace PointOfSale.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInformation")]
    public partial class UserInformation
    {
        [Key]
        public int UserId { get; set; }

        public int? Title { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string EmpId { get; set; }

        [StringLength(50)]
        public string CardNumber { get; set; }

        public int? RoleId { get; set; }

        public int? SiteId { get; set; }

        public int? UnitId { get; set; }

        public int? DeptId { get; set; }

        public int? LineId { get; set; }

        public int? MachineId { get; set; }

        public int? DesignationId { get; set; }

        public string Picture { get; set; }

        public string PictureOriginalName { get; set; }

        public int? UserType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        public int Gender { get; set; }

        [Required]
        [StringLength(100)]
        public string Nationality { get; set; }

        [StringLength(100)]
        public string NationalId { get; set; }

        public string NationalIdBackImg { get; set; }

        public string NationalIdFontImg { get; set; }

        [Required]
        [StringLength(500)]
        public string EmailAddress { get; set; }

        public int Religion { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string MobileNo { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? BasicSalary { get; set; }

        [Required]
        [StringLength(50)]
        public string ParAddress { get; set; }

        [StringLength(50)]
        public string ParAddressLine1 { get; set; }

        [Required]
        [StringLength(50)]
        public string ParCountry { get; set; }

        [StringLength(50)]
        public string ParState { get; set; }

        public int? ParDivisionId { get; set; }

        [StringLength(50)]
        public string ParCity { get; set; }

        [StringLength(50)]
        public string ParArea { get; set; }

        [StringLength(50)]
        public string ParPotalCode { get; set; }

        public bool SamePresentAddress { get; set; }

        [StringLength(50)]
        public string PreAddress { get; set; }

        [StringLength(50)]
        public string PreAddressLine1 { get; set; }

        [StringLength(50)]
        public string PreCountry { get; set; }

        [StringLength(50)]
        public string PreState { get; set; }

        public int? PreDivisionId { get; set; }

        [StringLength(50)]
        public string PreCity { get; set; }

        [StringLength(50)]
        public string PreArea { get; set; }

        [StringLength(50)]
        public string PrePostalCode { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "date")]
        public DateTime? JoinDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public long? UpdatedBy { get; set; }

        public bool? AssginSalary { get; set; }

        public int? SalaryPackageId { get; set; }

        [StringLength(50)]
        public string x1 { get; set; }

        [StringLength(50)]
        public string y1 { get; set; }

        [StringLength(50)]
        public string x2 { get; set; }

        [StringLength(50)]
        public string y2 { get; set; }

        [StringLength(50)]
        public string width { get; set; }

        [StringLength(50)]
        public string height { get; set; }

        public int? HolidayPackId { get; set; }

        public bool? IsCustomePack { get; set; }

        public int? NoOfPaidLeave { get; set; }

        public long? HolidayAssignedBy { get; set; }

        public DateTime? HolidayAssignedDate { get; set; }

        public int? WorkingScheduleId { get; set; }

        public long? WorkScheduleAssignedBy { get; set; }

        public DateTime? WorkScheduleAssignedDate { get; set; }

        public byte[] ImageData { get; set; }
    }
}
