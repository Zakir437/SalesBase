using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews
{
    public class DocumentModel
    {
        public long? DocumentId { get; set; }
        [Required]
        public string Name { get; set; }
        public HttpPostedFile File { get; set; }
        public string FileName { get; set; }
    }
}