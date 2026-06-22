using System;
using System.Drawing;
using System.Windows.Forms;
using SmartMedPharmacy.Data;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Forms
{
    public class LoginForm : Form
    {
        private ComboBox cmbRole;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "SmartMedPharmacy - Login";
            Size = new Size(420, 360);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            Label title = new Label
            {
                Text = "SmartMedPharmacy",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 130),
                AutoSize = true,
                Location = new Point(80, 25)
            };

            Label lblRole = new Label { Text = "Login as", Location = new Point(50, 90), AutoSize = true };
            cmbRole = new ComboBox
            {
                Location = new Point(160, 87),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[] { "Admin", "Customer" });
            cmbRole.SelectedIndex = 0;

            Label lblUser = new Label { Text = "Username", Location = new Point(50, 130), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(160, 127), Width = 200 };

            Label lblPass = new Label { Text = "Password", Location = new Point(50, 170), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(160, 167), Width = 200, UseSystemPasswordChar = true };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(160, 215),
                Width = 200,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Register as Customer",
                Location = new Point(160, 260),
                Width = 200,
                Height = 30,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.Click += BtnRegister_Click;

            Controls.Add(title);
            Controls.Add(lblRole);
            Controls.Add(cmbRole);
            Controls.Add(lblUser);
            Controls.Add(txtUsername);
            Controls.Add(lblPass);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);

            AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRole.SelectedItem.ToString() == "Admin")
            {
                Admin admin = DataManager.Instance.FindAdmin(username, password);
                if (admin == null)
                {
                    MessageBox.Show("Invalid admin credentials.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Hide();
                using (AdminDashboardForm dash = new AdminDashboardForm(admin))
                    dash.ShowDialog();
                Show();
                txtPassword.Clear();
            }
            else
            {
                Customer customer = DataManager.Instance.FindCustomer(username, password);
                if (customer == null)
                {
                    MessageBox.Show("Invalid customer credentials.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Hide();
                using (CustomerDashboardForm dash = new CustomerDashboardForm(customer))
                    dash.ShowDialog();
                Show();
                txtPassword.Clear();
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm reg = new RegisterForm())
                reg.ShowDialog();
        }
    }
}
