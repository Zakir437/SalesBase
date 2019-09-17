using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews
{
    public class CustomerModelView
    {
        public long? CustomerId { get; set; }
        [Required(ErrorMessage="Please Enter Customer Name")]
        [StringLength(50,MinimumLength=1,ErrorMessage="Invalid")]
        public string Name { get; set; }
        [EmailAddress]
        [Required(ErrorMessage="Please Enter Email Address")]
        [Remote("CustomerEmailExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "your entered email already exists..!!!")]
        public string Email { get; set; }
        [Required(ErrorMessage="Please Enter Phone No.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        public string Phone { get; set; }
        [Required(ErrorMessage="Please Enter Your Address")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter membership number")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Invalid")]
        [Remote("CustomerMembershipNumberExist", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "your entered number already exists..!!!")]
        public string MembershipNumber { get; set; }
        public bool IsCreditAllowed { get; set; }
        public int? CreditLimitId { get; set; }

        public int? CreatedBy { get; set; }


        //Credit Payment 
        public decimal CreditAmount { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CreditPay { get; set; }

        //Debit account refill
        public bool IsDebitAccounts { get; set; }
        public decimal DebitPay { get; set; }
        public decimal AvailableDebit { get; set; }
        public decimal DebitLimit { get; set; }
    }
}