using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmartMedPharmacy.Models;

namespace SmartMedPharmacy.Data
{
    public class DataManager
    {
        private static DataManager _instance;
        public static DataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataManager();
                return _instance;
            }
        }

        private readonly JsonStorage<Admin> _adminStore;
        private readonly JsonStorage<Customer> _customerStore;
        private readonly JsonStorage<Medicine> _medicineStore;
        private readonly JsonStorage<Order> _orderStore;

        public List<Admin> Admins { get; private set; }
        public List<Customer> Customers { get; private set; }
        public List<Medicine> Medicines { get; private set; }
        public List<Order> Orders { get; private set; }

        private DataManager()
        {
            _adminStore = new JsonStorage<Admin>("admins.json");
            _customerStore = new JsonStorage<Customer>("customers.json");
            _medicineStore = new JsonStorage<Medicine>("medicines.json");
            _orderStore = new JsonStorage<Order>("orders.json");
            LoadAll();
        }

        public void LoadAll()
        {
            try
            {
                Admins = _adminStore.Load();
                Customers = _customerStore.Load();
                Medicines = _medicineStore.Load();
                Orders = _orderStore.Load();
                SeedIfEmpty();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Admins = new List<Admin>();
                Customers = new List<Customer>();
                Medicines = new List<Medicine>();
                Orders = new List<Order>();
            }
        }

        private void SeedIfEmpty()
        {
            bool seeded = false;

            if (Admins.Count == 0)
            {
                Admins.Add(new Admin
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    FullName = "System Administrator",
                    Email = "admin@smartmed.com"
                });
                seeded = true;
            }

            if (Medicines.Count == 0)
            {
                Medicines.Add(new Medicine { Id = 1, Name = "Paracetamol", Category = "Pain Relief", Dosage = "500mg", Price = 5.50m, Stock = 200, Supplier = "MediSupply", ExpiryDate = DateTime.Now.AddMonths(18), DiscountPercent = 0, RequiresPrescription = false });
                Medicines.Add(new Medicine { Id = 2, Name = "Amoxicillin", Category = "Antibiotic", Dosage = "250mg", Price = 12.00m, Stock = 80, Supplier = "PharmaCorp", ExpiryDate = DateTime.Now.AddDays(20), DiscountPercent = 10, RequiresPrescription = true });
                Medicines.Add(new Medicine { Id = 3, Name = "Cetirizine", Category = "Antihistamine", Dosage = "10mg", Price = 7.25m, Stock = 150, Supplier = "MediSupply", ExpiryDate = DateTime.Now.AddMonths(12), DiscountPercent = 5, RequiresPrescription = false });
                Medicines.Add(new Medicine { Id = 4, Name = "Ibuprofen", Category = "Pain Relief", Dosage = "400mg", Price = 6.75m, Stock = 120, Supplier = "HealthPlus", ExpiryDate = DateTime.Now.AddMonths(24), DiscountPercent = 0, RequiresPrescription = false });
                Medicines.Add(new Medicine { Id = 5, Name = "Metformin", Category = "Diabetes", Dosage = "500mg", Price = 9.40m, Stock = 60, Supplier = "PharmaCorp", ExpiryDate = DateTime.Now.AddDays(25), DiscountPercent = 0, RequiresPrescription = true });
                seeded = true;
            }

            if (seeded)
                SaveAll();
        }

        public void Reload()
        {
            LoadAll();
        }

        public void SaveAll()
        {
            try
            {
                _adminStore.Save(Admins);
                _customerStore.Save(Customers);
                _medicineStore.Save(Medicines);
                _orderStore.Save(Orders);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Admin FindAdmin(string username, string password)
        {
            return Admins.FirstOrDefault(a => a.Username == username && a.CheckPassword(password));
        }

        public Customer FindCustomer(string username, string password)
        {
            return Customers.FirstOrDefault(c => c.Username == username && c.CheckPassword(password));
        }

        public bool UsernameExists(string username)
        {
            return Admins.Any(a => a.Username == username) || Customers.Any(c => c.Username == username);
        }

        public int NextCustomerId()
        {
            return Customers.Count == 0 ? 1 : Customers.Max(c => c.Id) + 1;
        }

        public int NextMedicineId()
        {
            return Medicines.Count == 0 ? 1 : Medicines.Max(m => m.Id) + 1;
        }

        public int NextOrderId()
        {
            return Orders.Count == 0 ? 1 : Orders.Max(o => o.Id) + 1;
        }

        public Customer GetCustomerById(int id)
        {
            return Customers.FirstOrDefault(c => c.Id == id);
        }

        public Medicine GetMedicineById(int id)
        {
            return Medicines.FirstOrDefault(m => m.Id == id);
        }
    }
}
