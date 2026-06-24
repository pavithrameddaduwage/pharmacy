using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class ManageOrdersForm : Form
    {
        private DataGridView grid;
        private DataGridView itemsGrid;
        private ComboBox cmbStatus;

        public ManageOrdersForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "Manage Orders";
            Size = new Size(940, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(540, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            itemsGrid = new DataGridView
            {
                Location = new Point(20, 395),
                Size = new Size(880, 120),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(itemsGrid);

            Label lblStatus = new Label { Text = "Change Status", Location = new Point(580, 30), AutoSize = true };
            cmbStatus = new ComboBox
            {
                Location = new Point(580, 55),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Pending", "Ready for Pickup", "Delivered" });
            Controls.Add(lblStatus);
            Controls.Add(cmbStatus);

            Button btnApply = new Button
            {
                Text = "Apply Status",
                Location = new Point(580, 95),
                Width = 250,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnApply.Click += BtnApply_Click;
            Controls.Add(btnApply);

            Button btnDelete = new Button
            {
                Text = "Delete Order",
                Location = new Point(580, 140),
                Width = 250,
                Height = 35,
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDelete_Click;
            Controls.Add(btnDelete);

            Button btnBack = new Button
            {
                Text = "Back",
                Location = new Point(580, 185),
                Width = 250,
                Height = 35,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Order o = Selected();
            if (o == null)
            {
                MessageBox.Show("Select an order first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult confirm = MessageBox.Show("Delete order #" + o.Id + "?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;
            DataManager.Instance.Orders.Remove(o);
            DataManager.Instance.SaveAll();
            LoadGrid();
            itemsGrid.DataSource = null;
            MessageBox.Show("Order deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadGrid()
        {
            DataManager.Instance.Reload();
            grid.DataSource = null;
            grid.DataSource = DataManager.Instance.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    Customer = CustomerName(o.CustomerId),
                    Date = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    o.Status,
                    o.Total,
                    Items = o.Items.Count
                }).ToList();
        }

        private string CustomerName(int customerId)
        {
            Customer c = DataManager.Instance.GetCustomerById(customerId);
            return c == null ? "Unknown" : c.FullName;
        }

        private Order Selected()
        {
            if (grid.CurrentRow == null)
                return null;
            object cell = grid.CurrentRow.Cells["Id"].Value;
            if (cell == null)
                return null;
            int id = Convert.ToInt32(cell);
            return DataManager.Instance.Orders.FirstOrDefault(o => o.Id == id);
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            Order o = Selected();
            if (o == null)
                return;
            cmbStatus.SelectedItem = o.Status;
            itemsGrid.DataSource = o.Items
                .Select(i => new
                {
                    i.MedicineName,
                    i.Quantity,
                    i.UnitPrice,
                    i.LineTotal
                }).ToList();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            Order o = Selected();
            if (o == null)
            {
                MessageBox.Show("Select an order first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cmbStatus.SelectedItem == null)
                return;
            o.Status = cmbStatus.SelectedItem.ToString();
            DataManager.Instance.SaveAll();
            LoadGrid();
            MessageBox.Show("Order status updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
