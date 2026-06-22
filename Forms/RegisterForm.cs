using System;
using System.Drawing;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Customer Registration";
            Size = new Size(420, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            int labelX = 30;
            int inputX = 150;
            int y = 30;

            AddRow("Full Name", labelX, inputX, ref y, out txtFullName);
            AddRow("Username", labelX, inputX, ref y, out txtUsername);
            AddRow("Password", labelX, inputX, ref y, out txtPassword);
            txtPassword.UseSystemPasswordChar = true;
            AddRow("Email", labelX, inputX, ref y, out txtEmail);
            AddRow("Phone", labelX, inputX, ref y, out txtPhone);
            AddRow("Address", labelX, inputX, ref y, out txtAddress);

            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(inputX, y + 10),
                Width = 220,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.Click += BtnRegister_Click;
            Controls.Add(btnRegister);
        }

        private void AddRow(string caption, int labelX, int inputX, ref int y, out TextBox box)
        {
            Label lbl = new Label { Text = caption, Location = new Point(labelX, y + 3), AutoSize = true };
            box = new TextBox { Location = new Point(inputX, y), Width = 220 };
            Controls.Add(lbl);
            Controls.Add(box);
            y += 45;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Full name, username and password are required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DataManager.Instance.UsernameExists(txtUsername.Text.Trim()))
            {
                MessageBox.Show("That username is already taken.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Customer customer = new Customer
            {
                Id = DataManager.Instance.NextCustomerId(),
                FullName = txtFullName.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text,
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };

            DataManager.Instance.Customers.Add(customer);
            DataManager.Instance.SaveAll();

            MessageBox.Show("Registration successful. You can now log in.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}
