using RefrigeratorApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Objects.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RefrigeratorApplication
{
    public partial class frmRefrigerator : Form
    {
        public frmRefrigerator()
        {
            InitializeComponent();
            this.Load += FrmRefrigerator_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
            _entity = new RefrigeratorEntities();
        }
        private void FrmRefrigerator_Load(object sender, EventArgs e)
        {
            cbProducts.Items.AddRange(_entity.ProductTypes.Select(x => x.ProductName).ToArray());
            AppHelper.HandleProductExpiry();
            DisplayProducts();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string selectedProd = string.Empty;
            if (cbProducts.SelectedIndex < 0 || string.IsNullOrEmpty(tbQuantity.Text))
                AppHelper.ShowMessage("Add");
            else
            {
                string productName = cbProducts.SelectedItem.ToString();
                int productId = AppHelper.GetProductId(productName);
                double quantity = double.Parse(tbQuantity.Text);
                Product prod = new Product();
                prod.ProductTypeId = productId;
                prod.Quantity = quantity;
                prod.AsOf = DateTime.Now.Date;
                prod.ExpiryDate = dateTimePicker1.Value;
                _entity.Products.Add(prod);
                _entity.SaveChanges();
                AppHelper.UpdateHistory(prod, quantity, "Added");
                ClearFields();
            }
            DisplayProducts();
        }
        private void btnConsume_Click(object sender, EventArgs e)
        {
            string selectedProd = string.Empty;
            if (cbProducts.SelectedIndex < 0 || string.IsNullOrWhiteSpace(tbQuantity.Text))
                AppHelper.ShowMessage("Consume");
            else
            {
                string productName = cbProducts.SelectedItem.ToString();
                double consumed = double.Parse(tbQuantity.Text);
                DateTime expiry = Convert.ToDateTime(dateTimePicker1.Text);
                int productTypeId = AppHelper.GetProductId(productName);
                Product prod = _entity.Products.Where(p => p.ProductTypeId == productTypeId)
                    .Where(p => p.ExpiryDate == expiry).FirstOrDefault();
                double remainingQty = prod.Quantity - consumed;
                prod.ProductTypeId = productTypeId;
                prod.Quantity = remainingQty;
                prod.ExpiryDate = dateTimePicker1.Value;
                if (remainingQty == 0)
                    _entity.Products.Remove(prod);
                _entity.SaveChanges();
                AppHelper.UpdateHistory(prod, consumed, "Consumed");
                ClearFields();
            }
            DisplayProducts();
        }
        private void btnStockCheck_Click(object sender, EventArgs e)
        {            
            string selectedProd = string.Empty;
            if (cbProducts.SelectedIndex < 0)
            {
                AppHelper.ShowMessage("CheckStock");
                DisplayProducts();
            }
            else
            {
                selectedProd = cbProducts.SelectedItem.ToString();
                List<ProductsInfo> productList = (from x in AppHelper.GetAllLiveProducts() 
                                                  where x.ProductName == selectedProd select x).ToList();
                dataGridView1.DataSource = productList;                
            }
        }
        private void btnCheckHistory_Click(object sender, EventArgs e)
        {
            ClearFields();
            dataGridView1.DataSource = (from p in _entity.Products_log
                                        join c in _entity.ProductTypes on p.ProductTypeId equals c.ProductTypeId
                                        orderby p.AsOf
                                        select new ProductsInfo_log { ProductName = c.ProductName, Action = p.Action,
                                            Quantity = p.Quantity, AsOf = p.AsOf, ExpiryDate = p.ExpiryDate }).ToList();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int quantity = -1;
                    if (int.TryParse(row.Cells[1].Value.ToString(), out quantity) && quantity > 0)
                    {
                        cbProducts.Text = row.Cells[0].Value.ToString();
                        tbQuantity.Text = row.Cells[1].Value.ToString();
                        dateTimePicker1.Text = row.Cells[3].Value.ToString();
                    }
                }
            }
        }       
        private void ClearFields()
        {
            cbProducts.Text = string.Empty;
            tbQuantity.Text = string.Empty;
            dateTimePicker1.Text = string.Empty;
        }
        private void DisplayProducts()
        {
            dataGridView1.DataSource = AppHelper.GetAllLiveProducts();
        }
        private RefrigeratorEntities _entity = null;
    }
}
