using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Accounting
{
    public class EmpSalaryXmlModelView
    {
        public long Id { get; set; }
        [Required(ErrorMessage ="Please enter salary amount")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage = "Please enter total present")]
        public int TotalPresent { get; set; }
        [Required(ErrorMessage = "Please enter total paid leave")]
        public int TotalPaidLeave { get; set; }
        [Required(ErrorMessage = "Please enter total working days")]
        public int TotalWorkingDays { get; set; }
        [Required(ErrorMessage = "Please enter total Holiday")]
        public int TotalHoliday { get; set; }
        public decimal DueAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public Nullable<int> PaidBy { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.DateTime GeneratedDate { get; set; }
        public int UserId { get; set; }
        public string EmpName { get; set; }
        public string Picture { get; set; }
        public decimal ActualSalary { get; set; }
        public int AssignUserId { get; set; }
        public System.DateTime XmlGenerateDate { get; set; }
        public int XmlGenerateBy { get; set; }
        public int? UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int Status { get; set; }
    }
}