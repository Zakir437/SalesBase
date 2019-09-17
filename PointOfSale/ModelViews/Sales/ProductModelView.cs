using PointOfSale.ModelViews.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointOfSale.ModelViews.Sales
{
    public class ProductModelView
    {
        public int? ProductId { get; set; }
        public int? SubMasterId { get; set; }
        public string TagIds { get; set; }
        [Display(Name="Product Name")]
        [Required(ErrorMessage="Please enter product name")]
        [StringLength(50,MinimumLength =1,ErrorMessage = "Please enter product name")]
        public string ProductName { get; set; }
        [Display(Name ="Category")]
        [Required(ErrorMessage ="Please select category")]
        public int ProductCategoryId { get; set; }
        public int? SubCategoryId { get; set; }

        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }

        public string Code { get; set; }
        [Display(Name ="Barcode")]
        [Required(ErrorMessage ="Please enter barcode")]
        [Remote("BarcodeNumberExists", "RemoteValidaton", HttpMethod = "Post", ErrorMessage = "This Number already exists..!!!")]
        [StringLength(50,MinimumLength =1,ErrorMessage ="Please enter barcode")]
        public string BarCode { get; set; }
        public string PLU { get; set; }
        [Required(ErrorMessage ="Please select unit")]
        public int Unit { get; set; }
        [Required(ErrorMessage = "Please enter price")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Please enter cost")]
        public decimal Cost { get; set; }
        [Display(Name ="Vat Rate")]
        public Nullable<int> VatRateId { get; set; }
        public bool IsVatIncluded { get; set; }
        public bool IsPriceChangeAllow { get; set; }
        public bool IsDiscountAllow { get; set; }
        public bool IsRefundAllow { get; set; }
        public bool IsAfterSaleService { get; set; }
        public int? ServiceTypeId { get; set; }
        public int? ServiceDays { get; set; }
        public bool IsUsingDefaultQuantity { get; set; }
        public bool IsUnitWise { get; set; }
        public bool Isperishable { get; set; }
        public int? ExpireDays { get; set; }
        public bool IsFixed { get; set; }
        public bool Status { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public decimal? MinimalQuantity { get; set; }

        public string QuantityOrName { get; set; }
        public int? MasterUnit { get; set; }

        public int? RestrictionId { get; set; }

        public bool IsPointBased { get; set; }
        public int? Points { get; set; }

        public bool IsUniqueItem { get; set; }
        public int CreatedBy { get; set; }

        public bool IsDynamic { get; set; }
        public bool SizeCheckBox { get; set; }
        public bool ColorCheckbox { get; set; }
        public bool PriceCheckBox { get; set; }
        public bool CostCheckBox { get; set; }
        public bool PluCheckbox { get; set; }
        public bool CodeCheckbox { get; set; }
        public bool MinimumQuantityCheckbox { get; set; }
        public int? SizeType { get; set; }
        
        public List<ProductDistributeItems> DistributeItems { get; set; }
        public string CancelDistributeIds { get; set; }
    }
}