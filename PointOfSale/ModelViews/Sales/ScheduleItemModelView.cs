using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointOfSale.ModelViews.Sales
{
    public class ScheduleItemModelView
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}