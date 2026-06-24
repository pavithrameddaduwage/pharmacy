using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class SearchMedicinesForm : Form
    {
        private readonly List<OrderItem> _cart;
        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtCategory;
        private TextBox txtMinPrice;
        private TextBox txtMaxPrice;
        private NumericUpDown numQuantity;
        private List<Medicine> _current;

        public string PrescriptionPath { get; private set; }

        public SearchMedicinesForm(List<OrderItem> cart)
        {
            _cart = cart;
            InitializeComponent();
            ShowResults(DataManager.Instance.Medicines.ToList());
        }

        private void InitializeComponent()
        {
            Text = "Search Medicines";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            Label lblName = new Label { Text = "Name", Location = new Point(20, 20), AutoSize = true };
            txtName = new TextBox { Location = new Point(80, 17), Width = 150 };

            Label lblCat = new Label { Text = "Category", Location = new Point(250, 20), AutoSize = true };
            txtCategory = new TextBox { Location = new Point(320, 17), Width = 130 };

            Label lblMin = new Label { Text = "Min $", Location = new Point(470, 20), AutoSize = true };
            txtMinPrice = new TextBox { Location = new Point(515, 17), Width = 70 };

            Label lblMax = new Label { Text = "Max $", Location = new Point(600, 20), AutoSize = true };
            txtMaxPrice = new TextBox { Location = new Point(645, 17), Width = 70 };

            Button btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(730, 15),
                Width = 130,
                Height = 28,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += BtnSearch_Click;

            Button btnBinary = new Button
            {
                Text = "Exact Name (Binary Search)",
                Location = new Point(20, 55),
                Width = 220,
                Height = 28,
                FlatStyle = FlatStyle.Flat
            };
            btnBinary.Click += BtnBinary_Click;

            Button btnReset = new Button
            {
                Text = "Show All",
                Location = new Point(260, 55),
                Width = 120,
                Height = 28,
                FlatStyle = FlatStyle.Flat
            };
            btnReset.Click += (s, e) => { ClearFilters(); ShowResults(DataManager.Instance.Medicines.ToList()); };

            grid = new DataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(840, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Label lblQty = new Label { Text = "Quantity", Location = new Point(20, 470), AutoSize = true };
            numQuantity = new NumericUpDown { Location = new Point(90, 467), Width = 80, Minimum = 1, Maximum = 1000, Value = 1 };

            Button btnAdd = new Button
            {
                Text = "Add to Cart",
                Location = new Point(190, 463),
                Width = 150,
                Height = 32,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += BtnAdd_Click;

            Button btnPrescription = new Button
            {
                Text = "Upload Prescription",
                Location = new Point(360, 463),
                Width = 180,
                Height = 32,
                FlatStyle = FlatStyle.Flat
            };
            btnPrescription.Click += BtnPrescription_Click;

            Button btnBack = new Button
            {
                Text = "Back to Dashboard",
                Location = new Point(560, 463),
                Width = 180,
                Height = 32,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);

            Controls.Add(lblName);
            Controls.Add(txtName);
            Controls.Add(lblCat);
            Controls.Add(txtCategory);
            Controls.Add(lblMin);
            Controls.Add(txtMinPrice);
            Controls.Add(lblMax);
            Controls.Add(txtMaxPrice);
            Controls.Add(btnSearch);
            Controls.Add(btnBinary);
            Controls.Add(btnReset);
            Controls.Add(grid);
            Controls.Add(lblQty);
            Controls.Add(numQuantity);
            Controls.Add(btnAdd);
            Controls.Add(btnPrescription);
        }

        private void ClearFilters()
        {
            txtName.Clear();
            txtCategory.Clear();
            txtMinPrice.Clear();
            txtMaxPrice.Clear();
        }

        private void ShowResults(List<Medicine> medicines)
        {
            _current = medicines;
            grid.DataSource = null;
            grid.DataSource = medicines.Select(m => new
            {
                m.Id,
                m.Name,
                m.Category,
                m.Dosage,
                Price = m.Price,
                Discount = m.DiscountPercent,
                YouPay = m.EffectivePrice(),
                m.Stock,
                Rx = m.RequiresPrescription,
                Status = m.ExpiryStatus()
            }).ToList();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            List<Medicine> results = MedicineSearch.LinearSearchByName(DataManager.Instance.Medicines, txtName.Text);
            results = MedicineSearch.FilterByCategory(results, txtCategory.Text);

            decimal min = 0;
            decimal max = decimal.MaxValue;
            if (!string.IsNullOrWhiteSpace(txtMinPrice.Text) &&
                !decimal.TryParse(txtMinPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out min))
            {
                MessageBox.Show("Min price must be numeric.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtMaxPrice.Text) &&
                !decimal.TryParse(txtMaxPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out max))
            {
                MessageBox.Show("Max price must be numeric.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            results = MedicineSearch.FilterByPriceRange(results, min, max);
            ShowResults(results);
        }

        private void BtnBinary_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Enter the exact medicine name to use binary search.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Medicine found = MedicineSearch.BinarySearchByName(DataManager.Instance.Medicines, txtName.Text);
            if (found == null)
            {
                MessageBox.Show("No medicine found with that exact name.", "Result",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowResults(new List<Medicine>());
                return;
            }
            ShowResults(new List<Medicine> { found });
        }

        private Medicine Selected()
        {
            if (grid.CurrentRow == null)
                return null;
            object cell = grid.CurrentRow.Cells["Id"].Value;
            if (cell == null)
                return null;
            return DataManager.Instance.GetMedicineById(Convert.ToInt32(cell));
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Medicine m = Selected();
            if (m == null)
            {
                MessageBox.Show("Select a medicine first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int qty = (int)numQuantity.Value;
            if (qty > m.Stock)
            {
                MessageBox.Show("Only " + m.Stock + " in stock.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (m.RequiresPrescription && string.IsNullOrEmpty(PrescriptionPath))
            {
                MessageBox.Show(m.Name + " requires a prescription. Please upload one before adding.", "Prescription Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal unit = m.EffectivePrice();
            OrderItem existing = _cart.FirstOrDefault(i => i.MedicineId == m.Id);
            if (existing != null)
            {
                existing.Quantity += qty;
                existing.LineTotal = existing.Quantity * existing.UnitPrice;
            }
            else
            {
                _cart.Add(new OrderItem
                {
                    MedicineId = m.Id,
                    MedicineName = m.Name,
                    Quantity = qty,
                    UnitPrice = unit,
                    LineTotal = unit * qty
                });
            }
            MessageBox.Show(qty + " x " + m.Name + " added to cart.", "Added",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPrescription_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Documents and Images|*.pdf;*.png;*.jpg;*.jpeg;*.doc;*.docx|All files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PrescriptionPath = dialog.FileName;
                    MessageBox.Show("Prescription selected:\n" + PrescriptionPath, "Prescription",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
