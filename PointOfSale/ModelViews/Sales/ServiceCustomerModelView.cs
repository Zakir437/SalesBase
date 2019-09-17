using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Sales
{
    public class ServiceCustomerModelView
    {
        public long? AssId { get; set; }
        public long? CustomerId { get; set; }
        [Required(ErrorMessage = "Please enter first name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter last name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter mobile no.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Mobile { get; set; }
        public string AlternateMobile { get; set; }
        [DataType(DataType.MultilineText)]
        public string AddressLine1 { get; set; }
        [DataType(DataType.MultilineText)]
        public string AddressLine2 { get; set; }

    }
}