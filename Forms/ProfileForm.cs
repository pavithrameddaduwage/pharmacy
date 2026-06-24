using System;
using System.Drawing;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class ProfileForm : Form
    {
        private readonly Customer _customer;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtPassword;

        public ProfileForm(Customer customer)
        {
            _customer = customer;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "My Profile";
            Size = new Size(440, 480);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            int x = 30;
            int inputX = 150;
            int y = 30;

            txtFullName = AddField("Full Name", x, inputX, ref y);
            txtEmail = AddField("Email", x, inputX, ref y);
            txtPhone = AddField("Phone", x, inputX, ref y);
            txtAddress = AddField("Address", x, inputX, ref y);
            txtPassword = AddField("Password", x, inputX, ref y);
            txtPassword.UseSystemPasswordChar = true;

            Button btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(inputX, y + 10),
                Width = 220,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            Button btnBack = new Button
            {
                Text = "Back",
                Location = new Point(inputX, y + 55),
                Width = 220,
                Height = 35,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) => Close();
            Controls.Add(btnBack);
        }

        private TextBox AddField(string caption, int x, int inputX, ref int y)
        {
            Label lbl = new Label { Text = caption, Location = new Point(x, y + 3), AutoSize = true };
            TextBox box = new TextBox { Location = new Point(inputX, y), Width = 220 };
            Controls.Add(lbl);
            Controls.Add(box);
            y += 45;
            return box;
        }

        private void LoadData()
        {
            txtFullName.Text = _customer.FullName;
            txtEmail.Text = _customer.Email;
            txtPhone.Text = _customer.PhoneNumber;
            txtAddress.Text = _customer.Address;
            txtPassword.Text = _customer.Password;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Full name and password are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _customer.FullName = txtFullName.Text.Trim();
            _customer.Email = txtEmail.Text.Trim();
            _customer.PhoneNumber = txtPhone.Text.Trim();
            _customer.Address = txtAddress.Text.Trim();
            _customer.Password = txtPassword.Text;
            DataManager.Instance.SaveAll();
            MessageBox.Show("Profile updated.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
