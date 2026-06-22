using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class ManageCustomersForm : Form
    {
        private DataGridView grid;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;

        public ManageCustomersForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "Manage Customers";
            Size = new Size(900, 520);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(520, 440),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            int x = 560;
            int y = 20;
            txtFullName = AddField("Full Name", x, ref y);
            txtEmail = AddField("Email", x, ref y);
            txtPhone = AddField("Phone", x, ref y);
            txtAddress = AddField("Address", x, ref y);

            Button btnUpdate = new Button
            {
                Text = "Update Customer",
                Location = new Point(x + 90, y + 10),
                Width = 220,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUpdate.Click += BtnUpdate_Click;
            Controls.Add(btnUpdate);
        }

        private TextBox AddField(string caption, int x, ref int y)
        {
            Label lbl = new Label { Text = caption, Location = new Point(x, y + 3), AutoSize = true };
            TextBox box = new TextBox { Location = new Point(x + 90, y), Width = 220 };
            Controls.Add(lbl);
            Controls.Add(box);
            y += 45;
            return box;
        }

        private void LoadGrid()
        {
            grid.DataSource = null;
            grid.DataSource = DataManager.Instance.Customers
                .Select(c => new
                {
                    c.Id,
                    c.Username,
                    c.FullName,
                    c.Email,
                    c.PhoneNumber,
                    c.Address
                }).ToList();
        }

        private Customer Selected()
        {
            if (grid.CurrentRow == null)
                return null;
            object cell = grid.CurrentRow.Cells["Id"].Value;
            if (cell == null)
                return null;
            return DataManager.Instance.GetCustomerById(Convert.ToInt32(cell));
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            Customer c = Selected();
            if (c == null)
                return;
            txtFullName.Text = c.FullName;
            txtEmail.Text = c.Email;
            txtPhone.Text = c.PhoneNumber;
            txtAddress.Text = c.Address;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            Customer c = Selected();
            if (c == null)
            {
                MessageBox.Show("Select a customer first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full name cannot be empty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            c.FullName = txtFullName.Text.Trim();
            c.Email = txtEmail.Text.Trim();
            c.PhoneNumber = txtPhone.Text.Trim();
            c.Address = txtAddress.Text.Trim();
            DataManager.Instance.SaveAll();
            LoadGrid();
            MessageBox.Show("Customer updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
