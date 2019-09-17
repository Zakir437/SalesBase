using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class PaymentBodyModel
    {
        public int? PaymentBodyId { get; set; }
        [Required(ErrorMessage ="Please select a category")]
        public int PaymentCategoryId { get; set; }
        public string MethodIds { get; set; }
        [Required(ErrorMessage = "Please select a type")]
        public int Inout { get; set; }
        [Required(ErrorMessage = "Please enter payment body name")]
        [StringLength(50,MinimumLength =1,ErrorMessage ="Please enter payment body name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter balance")]
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public int? CreatedBy { get; set; }
        public IEnumerable<int> SelectedMethodIds { get; set; }
    }
}