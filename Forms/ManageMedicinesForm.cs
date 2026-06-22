using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class ManageMedicinesForm : Form
    {
        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtCategory;
        private TextBox txtDosage;
        private TextBox txtPrice;
        private TextBox txtStock;
        private TextBox txtSupplier;
        private DateTimePicker dtpExpiry;
        private TextBox txtDiscount;
        private CheckBox chkPrescription;

        public ManageMedicinesForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "Manage Medicines";
            Size = new Size(960, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(560, 480),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            int x = 600;
            int y = 20;
            txtName = AddField("Name", x, ref y);
            txtCategory = AddField("Category", x, ref y);
            txtDosage = AddField("Dosage", x, ref y);
            txtPrice = AddField("Price", x, ref y);
            txtStock = AddField("Stock", x, ref y);
            txtSupplier = AddField("Supplier", x, ref y);

            Label lblExpiry = new Label { Text = "Expiry", Location = new Point(x, y + 3), AutoSize = true };
            dtpExpiry = new DateTimePicker { Location = new Point(x + 90, y), Width = 220, Format = DateTimePickerFormat.Short };
            Controls.Add(lblExpiry);
            Controls.Add(dtpExpiry);
            y += 40;

            txtDiscount = AddField("Discount %", x, ref y);

            chkPrescription = new CheckBox { Text = "Requires Prescription", Location = new Point(x + 90, y), AutoSize = true };
            Controls.Add(chkPrescription);
            y += 40;

            Button btnAdd = MakeButton("Add", x, y, Color.FromArgb(0, 120, 130));
            btnAdd.Click += BtnAdd_Click;
            Button btnUpdate = MakeButton("Update", x + 105, y, Color.FromArgb(0, 90, 160));
            btnUpdate.Click += BtnUpdate_Click;
            Button btnDelete = MakeButton("Delete", x + 210, y, Color.FromArgb(180, 60, 60));
            btnDelete.Click += BtnDelete_Click;
            y += 50;

            Button btnClear = MakeButton("Clear Fields", x, y, Color.Gray);
            btnClear.Width = 315;
            btnClear.Click += (s, e) => ClearFields();
        }

        private TextBox AddField(string caption, int x, ref int y)
        {
            Label lbl = new Label { Text = caption, Location = new Point(x, y + 3), AutoSize = true };
            TextBox box = new TextBox { Location = new Point(x + 90, y), Width = 220 };
            Controls.Add(lbl);
            Controls.Add(box);
            y += 40;
            return box;
        }

        private Button MakeButton(string text, int x, int y, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Width = 100,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(btn);
            return btn;
        }

        private void LoadGrid()
        {
            grid.DataSource = null;
            grid.DataSource = DataManager.Instance.Medicines
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Category,
                    m.Dosage,
                    m.Price,
                    m.Stock,
                    m.Supplier,
                    Expiry = m.ExpiryDate.ToString("yyyy-MM-dd"),
                    Discount = m.DiscountPercent,
                    Rx = m.RequiresPrescription,
                    ExpiringSoon = m.IsExpiringSoon()
                }).ToList();
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            Medicine m = Selected();
            if (m == null)
                return;
            txtName.Text = m.Name;
            txtCategory.Text = m.Category;
            txtDosage.Text = m.Dosage;
            txtPrice.Text = m.Price.ToString(CultureInfo.InvariantCulture);
            txtStock.Text = m.Stock.ToString();
            txtSupplier.Text = m.Supplier;
            dtpExpiry.Value = m.ExpiryDate;
            txtDiscount.Text = m.DiscountPercent.ToString(CultureInfo.InvariantCulture);
            chkPrescription.Checked = m.RequiresPrescription;
        }

        private Medicine Selected()
        {
            if (grid.CurrentRow == null)
                return null;
            object cell = grid.CurrentRow.Cells["Id"].Value;
            if (cell == null)
                return null;
            int id = Convert.ToInt32(cell);
            return DataManager.Instance.GetMedicineById(id);
        }

        private bool ValidateInputs(out decimal price, out int stock, out decimal discount)
        {
            price = 0;
            stock = 0;
            discount = 0;

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Name and category are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out price) || price < 0)
            {
                MessageBox.Show("Price must be a non-negative number.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtStock.Text, out stock) || stock < 0)
            {
                MessageBox.Show("Stock must be a non-negative whole number.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                if (!decimal.TryParse(txtDiscount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out discount) || discount < 0 || discount > 100)
                {
                    MessageBox.Show("Discount must be between 0 and 100.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            decimal price, discount;
            int stock;
            if (!ValidateInputs(out price, out stock, out discount))
                return;

            Medicine m = new Medicine
            {
                Id = DataManager.Instance.NextMedicineId(),
                Name = txtName.Text.Trim(),
                Category = txtCategory.Text.Trim(),
                Dosage = txtDosage.Text.Trim(),
                Price = price,
                Stock = stock,
                Supplier = txtSupplier.Text.Trim(),
                ExpiryDate = dtpExpiry.Value,
                DiscountPercent = discount,
                RequiresPrescription = chkPrescription.Checked
            };
            DataManager.Instance.Medicines.Add(m);
            DataManager.Instance.SaveAll();
            LoadGrid();
            ClearFields();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            Medicine m = Selected();
            if (m == null)
            {
                MessageBox.Show("Select a medicine to update.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal price, discount;
            int stock;
            if (!ValidateInputs(out price, out stock, out discount))
                return;

            m.Name = txtName.Text.Trim();
            m.Category = txtCategory.Text.Trim();
            m.Dosage = txtDosage.Text.Trim();
            m.Price = price;
            m.Stock = stock;
            m.Supplier = txtSupplier.Text.Trim();
            m.ExpiryDate = dtpExpiry.Value;
            m.DiscountPercent = discount;
            m.RequiresPrescription = chkPrescription.Checked;
            DataManager.Instance.SaveAll();
            LoadGrid();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Medicine m = Selected();
            if (m == null)
            {
                MessageBox.Show("Select a medicine to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Delete " + m.Name + "?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            DataManager.Instance.Medicines.Remove(m);
            DataManager.Instance.SaveAll();
            LoadGrid();
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtCategory.Clear();
            txtDosage.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            txtSupplier.Clear();
            txtDiscount.Clear();
            chkPrescription.Checked = false;
            dtpExpiry.Value = DateTime.Now;
        }
    }
}
