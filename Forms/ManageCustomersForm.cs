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
        private DataGridView ordersGrid;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private Label lblOrders;

        public ManageCustomersForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "Manage Customers";
            Size = new Size(940, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(520, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(240, 248, 248) }
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            // Relationship view: the selected customer's orders.
            lblOrders = new Label
            {
                Text = "Orders for selected customer",
                Location = new Point(20, 330),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 130)
            };
            Controls.Add(lblOrders);

            ordersGrid = new DataGridView
            {
                Location = new Point(20, 355),
                Size = new Size(520, 200),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(240, 248, 248) }
            };
            Controls.Add(ordersGrid);

            int x = 560;
            int y = 20;
            txtUsername = AddField("Username", x, ref y);
            txtPassword = AddField("Password", x, ref y);
            txtFullName = AddField("Full Name", x, ref y);
            txtEmail = AddField("Email", x, ref y);
            txtPhone = AddField("Phone", x, ref y);
            txtAddress = AddField("Address", x, ref y);

            Button btnAdd = MakeActionButton("Add Customer", x + 90, y + 10, Color.FromArgb(0, 150, 90));
            btnAdd.Click += BtnAdd_Click;

            Button btnUpdate = MakeActionButton("Update Customer", x + 90, y + 50, Color.FromArgb(0, 120, 130));
            btnUpdate.Click += BtnUpdate_Click;

            Button btnDelete = MakeActionButton("Delete Customer", x + 90, y + 90, Color.FromArgb(180, 60, 60));
            btnDelete.Click += BtnDelete_Click;

            Button btnNew = MakeActionButton("Clear / New", x + 90, y + 130, Color.FromArgb(110, 110, 110));
            btnNew.Click += (s, e) => ClearFields();

            Button btnBack = MakeActionButton("Back", x + 90, y + 175, Color.FromArgb(70, 70, 70));
            btnBack.Click += (s, e) => Close();
        }

        private Button MakeActionButton(string text, int x, int y, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Width = 220,
                Height = 35,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(btn);
            return btn;
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            ordersGrid.DataSource = null;
            lblOrders.Text = "Orders for selected customer";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Username, password and full name are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DataManager.Instance.UsernameExists(username))
            {
                MessageBox.Show("That username is already taken.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Customer c = new Customer
            {
                Id = DataManager.Instance.NextCustomerId(),
                Username = username,
                Password = password,
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };
            DataManager.Instance.Customers.Add(c);
            DataManager.Instance.SaveAll();
            LoadGrid();
            ClearFields();
            MessageBox.Show("Customer added.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txtUsername.Text = c.Username;
            txtPassword.Text = c.Password;
            txtFullName.Text = c.FullName;
            txtEmail.Text = c.Email;
            txtPhone.Text = c.PhoneNumber;
            txtAddress.Text = c.Address;
            LoadOrders(c);
        }

        private void LoadOrders(Customer c)
        {
            var orders = DataManager.Instance.Orders
                .Where(o => o.CustomerId == c.Id)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    Date = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    o.Status,
                    Items = o.Items.Count,
                    o.Total
                }).ToList();
            ordersGrid.DataSource = null;
            ordersGrid.DataSource = orders;
            lblOrders.Text = "Orders for " + c.FullName + " (" + orders.Count + ")";
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

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Customer c = Selected();
            if (c == null)
            {
                MessageBox.Show("Select a customer first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int orderCount = DataManager.Instance.Orders.Count(o => o.CustomerId == c.Id);
            string message = orderCount > 0
                ? "Delete '" + c.FullName + "'? This customer has " + orderCount +
                  " order(s), which will also be removed."
                : "Delete '" + c.FullName + "'?";

            DialogResult confirm = MessageBox.Show(message, "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            DataManager.Instance.Orders.RemoveAll(o => o.CustomerId == c.Id);
            DataManager.Instance.Customers.Remove(c);
            DataManager.Instance.SaveAll();

            LoadGrid();
            ordersGrid.DataSource = null;
            lblOrders.Text = "Orders for selected customer";
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            MessageBox.Show("Customer deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
