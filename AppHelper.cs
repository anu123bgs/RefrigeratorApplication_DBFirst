using RefrigeratorApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RefrigeratorApplication
{
    public static class AppHelper
    {
       
        public static List<ProductsInfo> GetAllLiveProducts()
        {
            List<ProductsInfo> productList = new List<ProductsInfo>();
            DateTime today = DateTime.Now.Date; 
            using (RefrigeratorEntities _entity = new RefrigeratorEntities())
            {
                productList = (from p in _entity.Products
                                join c in _entity.ProductTypes on p.ProductTypeId equals c.ProductTypeId
                                where p.ExpiryDate >= today
                               where p.Quantity > 0
                                select new ProductsInfo { ProductName = c.ProductName, Quantity = p.Quantity,
                                    MesUnit = c.MesUnit, ExpiryDate = p.ExpiryDate }).ToList();
            }
            return productList;
        }
        public static void HandleProductExpiry()
        {
            List<ProductsInfo> productList = GetAllLiveProducts();
            StringBuilder expiryMsg = new StringBuilder("Below Products are expiring soon. Please remove them before expiry. \n");
            StringBuilder expiredMsg = new StringBuilder("Below products are removed from stock because of expiry date. Please remove them from Fridge. \n");
            List<ProductsInfo> expiringProductList = new List<ProductsInfo>();
            bool productsExpiring = false;
            foreach (ProductsInfo product in productList) 
            {
                if (product != null)
                {
                    if (product.ExpiryDate <= DateTime.Now.AddDays(1))
                    {
                        expiryMsg.Append($"Product: {product.ProductName} ==> Expiry Date: {product.ExpiryDate.Date} \n");
                        productsExpiring = true;
                    }
                    if (product.ExpiryDate == DateTime.Now.Date)
                    {
                        expiredMsg.Append($"Product: {product.ProductName} ==> Expiry Date: {product.ExpiryDate.Date} \n");
                        expiringProductList.Add(product);
                    }
                }
            }
            if(productsExpiring)
                MessageBox.Show(expiryMsg.ToString(), "Products Expiring", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (expiringProductList.Count() > 0)
            {
                RemoveExpiredProducts(expiringProductList);
                MessageBox.Show(expiredMsg.ToString(), "Removed Expired Products", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private static void RemoveExpiredProducts(List<ProductsInfo> productList)
        {
            using (RefrigeratorEntities entity = new RefrigeratorEntities())
            {
                foreach (ProductsInfo product in productList)
                {
                    int productTypeId = AppHelper.GetProductId(product.ProductName);
                    Product prod = entity.Products.Where(p => p.ProductTypeId == productTypeId)
                        .Where(p => p.ExpiryDate == product.ExpiryDate).FirstOrDefault();
                    entity.Products.Remove(prod);
                    entity.SaveChanges();
                    UpdateHistory(prod, product.Quantity, "Expired");
                }
            }
        }
        public static int GetProductId(string productName)
        {
            using (RefrigeratorEntities entity = new RefrigeratorEntities())
            {
                return (from x in entity.ProductTypes where x.ProductName == productName select x.ProductTypeId).First();
            }
        }        
        public static void UpdateHistory(Product product, double quantity, string action)
        {
            using (RefrigeratorEntities entity = new RefrigeratorEntities())
            {
                Products_log prod = new Products_log();
                prod.ProductTypeId = product.ProductTypeId;
                prod.Quantity = quantity;
                prod.AsOf = product.AsOf;
                prod.ExpiryDate = product.ExpiryDate;
                prod.Action = action.ToUpper();
                entity.Products_log.Add(prod);
                entity.SaveChanges();
            }
        }
        public static void ShowMessage(string msgType)
        {
            string message = string.Empty;
            switch (msgType)
            {
                case "Add":
                    message = "Select a product and enter quantity and then click Add";
                    break;
                case "Consume":
                    message = "Select a row from below grid, update consumed quantity and then click Consume";
                    break;
                case "CheckStock":
                    message = "Select a product and click Check Stock to check available quantity.";
                    break;
            }
            MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }        
    }
}
