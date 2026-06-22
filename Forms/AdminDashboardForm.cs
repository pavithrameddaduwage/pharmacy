using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class AdminDashboardForm : Form
    {
        private readonly Admin _admin;
        private Label lblSales;
        private Label lblStock;
        private Label lblActiveOrders;
        private Label lblExpiring;

        public AdminDashboardForm(Admin admin)
        {
            _admin = admin;
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            Text = "Admin Dashboard - SmartMedPharmacy";
            Size = new Size(820, 540);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Label header = new Label
            {
                Text = "Admin Dashboard",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 130),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            Controls.Add(header);

            lblSales = MakeCard("Total Sales", new Point(25, 70));
            lblStock = MakeCard("Medicines In Stock", new Point(285, 70));
            lblActiveOrders = MakeCard("Active Orders", new Point(545, 70));
            lblExpiring = MakeCard("Expiring Soon", new Point(25, 180));

            AddNav("Manage Medicines", new Point(285, 180), (s, e) => Open(new ManageMedicinesForm()));
            AddNav("Manage Customers", new Point(545, 180), (s, e) => Open(new ManageCustomersForm()));
            AddNav("Manage Orders", new Point(285, 250), (s, e) => Open(new ManageOrdersForm()));
            AddNav("Reports", new Point(545, 250), (s, e) => Open(new ReportsForm()));

            Button btnRefresh = new Button
            {
                Text = "Refresh Stats",
                Location = new Point(25, 320),
                Width = 230,
                Height = 40,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => RefreshStats();
            Controls.Add(btnRefresh);

            Button btnLogout = new Button
            {
                Text = "Logout",
                Location = new Point(25, 420),
                Width = 230,
                Height = 40,
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.Click += (s, e) => Close();
            Controls.Add(btnLogout);
        }

        private Label MakeCard(string caption, Point location)
        {
            Panel panel = new Panel
            {
                Location = location,
                Size = new Size(230, 90),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 248, 248)
            };
            Label captionLabel = new Label
            {
                Text = caption,
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };
            Label valueLabel = new Label
            {
                Text = "-",
                Location = new Point(10, 40),
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 130)
            };
            panel.Controls.Add(captionLabel);
            panel.Controls.Add(valueLabel);
            Controls.Add(panel);
            return valueLabel;
        }

        private void AddNav(string text, Point location, EventHandler handler)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Width = 230,
                Height = 55,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btn.Click += handler;
            Controls.Add(btn);
        }

        private void Open(Form form)
        {
            using (form)
                form.ShowDialog();
            RefreshStats();
        }

        private void RefreshStats()
        {
            DataManager dm = DataManager.Instance;
            decimal totalSales = dm.Orders
                .Where(o => o.Status == "Delivered")
                .Sum(o => o.Total);
            int stock = dm.Medicines.Sum(m => m.Stock);
            int active = dm.Orders.Count(o => o.Status != "Delivered");
            int expiring = dm.Medicines.Count(m => m.IsExpiringSoon());

            lblSales.Text = totalSales.ToString("C");
            lblStock.Text = stock.ToString();
            lblActiveOrders.Text = active.ToString();
            lblExpiring.Text = expiring.ToString();
        }
    }
}
