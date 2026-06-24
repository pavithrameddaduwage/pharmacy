using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class CustomerDashboardForm : Form
    {
        private readonly Customer _customer;
        private readonly List<OrderItem> _cart = new List<OrderItem>();
        private string _prescriptionPath;

        public CustomerDashboardForm(Customer customer)
        {
            _customer = customer;
            InitializeComponent();
            ShowExpiryNotice();
        }

        private void InitializeComponent()
        {
            Text = "Customer Dashboard - SmartMedPharmacy";
            Size = new Size(560, 480);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Label header = new Label
            {
                Text = "Welcome, " + _customer.FullName,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 130),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            Controls.Add(header);

            AddNav("Search Medicines", new Point(25, 80), (s, e) => OpenSearch());
            AddNav("View Cart / Checkout", new Point(290, 80), (s, e) => OpenCart());
            AddNav("Track Orders", new Point(25, 160), (s, e) => Open(new TrackOrdersForm(_customer)));
            AddNav("My Profile", new Point(290, 160), (s, e) => Open(new ProfileForm(_customer)));

            Button btnLogout = new Button
            {
                Text = "Logout",
                Location = new Point(25, 360),
                Width = 235,
                Height = 40,
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.Click += (s, e) => Close();
            Controls.Add(btnLogout);
        }

        private void AddNav(string text, Point location, EventHandler handler)
        {
            Button btn = new Button
            {
                Text = text,
                Location = location,
                Width = 235,
                Height = 60,
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
        }

        private void OpenSearch()
        {
            using (SearchMedicinesForm form = new SearchMedicinesForm(_cart))
            {
                form.ShowDialog();
                if (!string.IsNullOrEmpty(form.PrescriptionPath))
                    _prescriptionPath = form.PrescriptionPath;
            }
        }

        private void OpenCart()
        {
            using (CartForm form = new CartForm(_customer, _cart, _prescriptionPath))
                form.ShowDialog();
        }

        private void ShowExpiryNotice()
        {
            // Customers are only notified about items expiring soon, never expired stock.
            var expiring = DataManager.Instance.Medicines.Where(m => m.IsExpiringSoon()).ToList();
            if (expiring.Count == 0)
                return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Medicines expiring within 30 days:");
            foreach (Medicine m in expiring)
                sb.AppendLine(" - " + m.Name + " (expires " + m.ExpiryDate.ToString("yyyy-MM-dd") + ")");
            MessageBox.Show(sb.ToString(), "Expiry Notice",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
