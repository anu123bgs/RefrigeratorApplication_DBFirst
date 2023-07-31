using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefrigeratorApplication.Models
{
    public class ProductsInfo
    {
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string MesUnit { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
    public partial class ProductsInfo_log
    {
        public string ProductName { get; set; }
        public string Action { get; set; }
        public double Quantity { get; set; }
        public DateTime AsOf { get; set; }
        public DateTime ExpiryDate { get; set; }        
    }

}
