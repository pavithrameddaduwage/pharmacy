using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class ReportsForm : Form
    {
        private TextBox txtOutput;

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Reports";
            Size = new Size(760, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Button btnSales = MakeButton("Sales Report", new Point(20, 20));
            btnSales.Click += (s, e) => ShowSalesReport();
            Button btnStock = MakeButton("Stock Report", new Point(200, 20));
            btnStock.Click += (s, e) => ShowStockReport();
            Button btnHistory = MakeButton("Customer Order History", new Point(380, 20));
            btnHistory.Width = 200;
            btnHistory.Click += (s, e) => ShowOrderHistory();

            Button btnExport = MakeButton("Export Order History (CSV/Excel)", new Point(20, 65));
            btnExport.Width = 320;
            btnExport.BackColor = Color.FromArgb(0, 90, 160);
            btnExport.Click += (s, e) => ExportOrderHistory();

            txtOutput = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(700, 390),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9.5F)
            };
            Controls.Add(txtOutput);

            Button btnBack = new Button
            {
                Text = "Back",
                Location = new Point(20, 510),
                Width = 700,
                Height = 32,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);
        }

        private Button MakeButton(string text, Point location)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Width = 170,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(btn);
            return btn;
        }

        private void ShowSalesReport()
        {
            DataManager.Instance.Reload();
            DataManager dm = DataManager.Instance;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================= SALES REPORT =========================");
            sb.AppendLine();
            decimal delivered = dm.Orders.Where(o => o.Status == "Delivered").Sum(o => o.Total);
            decimal pending = dm.Orders.Where(o => o.Status != "Delivered").Sum(o => o.Total);
            sb.AppendLine("Total orders                 : " + dm.Orders.Count);
            sb.AppendLine("Total sales (all orders)     : " + Money.Format(dm.Orders.Sum(o => o.Total)));
            sb.AppendLine("Delivered sales              : " + Money.Format(delivered));
            sb.AppendLine("Outstanding (not delivered)  : " + Money.Format(pending));
            sb.AppendLine();
            sb.AppendLine(string.Format("{0,-6} {1,-22} {2,-12} {3,-18} {4,12}",
                "Id", "Customer", "Date", "Status", "Total"));
            sb.AppendLine(new string('-', 74));
            foreach (Order o in dm.Orders.OrderByDescending(o => o.OrderDate))
            {
                Customer c = dm.GetCustomerById(o.CustomerId);
                sb.AppendLine(string.Format("{0,-6} {1,-22} {2,-12} {3,-18} {4,12}",
                    o.Id, Trim(c == null ? "Unknown" : c.FullName, 22),
                    o.OrderDate.ToString("yyyy-MM-dd"), o.Status, Money.Format(o.Total)));
            }
            txtOutput.Text = sb.ToString();
        }

        private void ShowStockReport()
        {
            DataManager.Instance.Reload();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================= STOCK REPORT =========================");
            sb.AppendLine();
            sb.AppendLine(string.Format("{0,-4} {1,-18} {2,-16} {3,7} {4,-12} {5,-12}",
                "Id", "Name", "Category", "Stock", "Expiry", "Expiring"));
            sb.AppendLine(new string('-', 74));
            foreach (Medicine m in DataManager.Instance.Medicines.OrderBy(m => m.Name))
            {
                sb.AppendLine(string.Format("{0,-4} {1,-18} {2,-16} {3,7} {4,-12} {5,-12}",
                    m.Id, Trim(m.Name, 18), Trim(m.Category, 16), m.Stock,
                    m.ExpiryDate.ToString("yyyy-MM-dd"),
                    m.IsExpiringSoon() ? "YES <<" : "no"));
            }
            txtOutput.Text = sb.ToString();
        }

        private string Trim(string value, int max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Length <= max ? value : value.Substring(0, max - 1) + "…";
        }

        private void ShowOrderHistory()
        {
            DataManager.Instance.Reload();
            DataManager dm = DataManager.Instance;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===== CUSTOMER ORDER HISTORY =====");
            foreach (Customer c in dm.Customers.OrderBy(c => c.FullName))
            {
                var orders = dm.Orders.Where(o => o.CustomerId == c.Id).ToList();
                sb.AppendLine(c.FullName + " (" + orders.Count + " orders)");
                foreach (Order o in orders)
                {
                    sb.AppendLine("   Order #" + o.Id + " - " + o.OrderDate.ToString("yyyy-MM-dd") +
                        " - " + o.Status + " - " + Money.Format(o.Total));
                }
            }
            txtOutput.Text = sb.ToString();
        }

        private void ExportOrderHistory()
        {
            try
            {
                DataManager.Instance.Reload();
                if (DataManager.Instance.Orders.Count == 0)
                {
                    MessageBox.Show("There are no orders to export yet. Place an order first.",
                        "Nothing to Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "CSV files (*.csv)|*.csv";
                    dialog.FileName = "order_history.csv";
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    DataManager dm = DataManager.Instance;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("OrderId,Customer,Date,Status,MedicineName,Quantity,UnitPrice,LineTotal,OrderTotal");
                    foreach (Order o in dm.Orders)
                    {
                        Customer c = dm.GetCustomerById(o.CustomerId);
                        string customerName = c == null ? "Unknown" : c.FullName;
                        foreach (OrderItem item in o.Items)
                        {
                            sb.AppendLine(string.Join(",",
                                o.Id,
                                Escape(customerName),
                                o.OrderDate.ToString("yyyy-MM-dd"),
                                Escape(o.Status),
                                Escape(item.MedicineName),
                                item.Quantity,
                                item.UnitPrice,
                                item.LineTotal,
                                o.Total));
                        }
                    }
                    File.WriteAllText(dialog.FileName, sb.ToString());
                    MessageBox.Show("Exported to " + dialog.FileName, "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            if (value.Contains(",") || value.Contains("\""))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }
    }
}
