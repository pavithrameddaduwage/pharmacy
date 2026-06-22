using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class TrackOrdersForm : Form
    {
        private readonly Customer _customer;
        private DataGridView grid;
        private DataGridView itemsGrid;

        public TrackOrdersForm(Customer customer)
        {
            _customer = customer;
            InitializeComponent();
            LoadGrid();
        }

        private void InitializeComponent()
        {
            Text = "Track My Orders";
            Size = new Size(820, 540);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            grid = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(760, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            Controls.Add(grid);

            Label lblItems = new Label { Text = "Order Items", Location = new Point(20, 310), AutoSize = true };
            Controls.Add(lblItems);

            itemsGrid = new DataGridView
            {
                Location = new Point(20, 335),
                Size = new Size(760, 160),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(itemsGrid);
        }

        private void LoadGrid()
        {
            grid.DataSource = null;
            grid.DataSource = DataManager.Instance.Orders
                .Where(o => o.CustomerId == _customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    Date = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    o.Status,
                    o.Total,
                    Prescription = string.IsNullOrEmpty(o.PrescriptionFilePath) ? "None" : "Attached"
                }).ToList();
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
                return;
            object cell = grid.CurrentRow.Cells["Id"].Value;
            if (cell == null)
                return;
            int id = Convert.ToInt32(cell);
            Order o = DataManager.Instance.Orders.FirstOrDefault(x => x.Id == id);
            if (o == null)
                return;
            itemsGrid.DataSource = o.Items.Select(i => new
            {
                i.MedicineName,
                i.Quantity,
                i.UnitPrice,
                i.LineTotal
            }).ToList();
        }
    }
}
