using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class CartForm : Form
    {
        private readonly Customer _customer;
        private readonly List<OrderItem> _cart;
        private readonly string _prescriptionPath;
        private DataGridView grid;
        private Label lblTotal;

        public CartForm(Customer customer, List<OrderItem> cart, string prescriptionPath)
        {
            _customer = customer;
            _cart = cart;
            _prescriptionPath = prescriptionPath;
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "My Cart";
            Size = new Size(640, 480);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(580, 320),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(grid);

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Location = new Point(20, 350),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            Controls.Add(lblTotal);

            Button btnRemove = new Button
            {
                Text = "Remove Selected",
                Location = new Point(20, 390),
                Width = 150,
                Height = 35,
                FlatStyle = FlatStyle.Flat
            };
            btnRemove.Click += BtnRemove_Click;
            Controls.Add(btnRemove);

            Button btnPlace = new Button
            {
                Text = "Place Order",
                Location = new Point(450, 390),
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPlace.Click += BtnPlace_Click;
            Controls.Add(btnPlace);

            Button btnBack = new Button
            {
                Text = "Back",
                Location = new Point(245, 390),
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);
        }

        private void LoadGrid()
        {
            grid.DataSource = null;
            grid.DataSource = _cart.Select(i => new
            {
                i.MedicineId,
                i.MedicineName,
                i.Quantity,
                i.UnitPrice,
                i.LineTotal
            }).ToList();
            lblTotal.Text = "Total: " + Money.Format(_cart.Sum(i => i.LineTotal));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
                return;
            object cell = grid.CurrentRow.Cells["MedicineId"].Value;
            if (cell == null)
                return;
            int id = Convert.ToInt32(cell);
            OrderItem item = _cart.FirstOrDefault(i => i.MedicineId == id);
            if (item != null)
                _cart.Remove(item);
            LoadGrid();
        }

        private void BtnPlace_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataManager dm = DataManager.Instance;
            foreach (OrderItem item in _cart)
            {
                Medicine m = dm.GetMedicineById(item.MedicineId);
                if (m == null)
                {
                    MessageBox.Show(item.MedicineName + " is no longer available.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (item.Quantity > m.Stock)
                {
                    MessageBox.Show("Not enough stock for " + m.Name + ". Available: " + m.Stock, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Order order = new Order
            {
                Id = dm.NextOrderId(),
                CustomerId = _customer.Id,
                OrderDate = DateTime.Now,
                Status = "Pending",
                Items = _cart.Select(i => new OrderItem
                {
                    MedicineId = i.MedicineId,
                    MedicineName = i.MedicineName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList(),
                Total = _cart.Sum(i => i.LineTotal),
                PrescriptionFilePath = _prescriptionPath
            };

            foreach (OrderItem item in order.Items)
            {
                Medicine m = dm.GetMedicineById(item.MedicineId);
                m.Stock -= item.Quantity;
            }

            dm.Orders.Add(order);
            dm.SaveAll();
            _cart.Clear();
            LoadGrid();
            MessageBox.Show("Order #" + order.Id + " placed. Status: Pending.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
