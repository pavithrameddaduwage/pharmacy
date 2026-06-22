# SmartMedPharmacy

A desktop pharmacy management application for a pharmacy chain, built as a
**C# Windows Forms** project targeting **.NET Framework 4.6.1** and compatible
with **Visual Studio 2015 or higher**. Data is stored locally in JSON files using
the **Newtonsoft.Json** library.

---

## 1. Requirements

- **Operating system:** Windows (Windows Forms / .NET Framework runs on Windows only).
- **IDE:** Visual Studio 2015 or newer (Community edition is fine), with the
  **.NET desktop development** workload installed.
- **.NET Framework 4.6.1** developer pack (installed automatically with the
  workload above).
- **Newtonsoft.Json 13.0.3** (restored automatically via NuGet — see below).

> Note: This project cannot be compiled or run on macOS or Linux, because Windows
> Forms depends on the Windows API. Use a Windows PC or a Windows virtual machine.

---

## 2. How to run the program

1. **Clone or download** this repository.
   ```
   git clone https://github.com/pavithrameddaduwage/pharmacy.git
   ```
2. **Open the solution.** Double-click `SmartMedPharmacy.sln` to open it in Visual Studio.
3. **Restore the NuGet package.** Newtonsoft.Json restores automatically the first
   time you build. If it does not, open
   **Tools → NuGet Package Manager → Package Manager Console** and run:
   ```
   Update-Package -reinstall
   ```
4. **Build and run.** Press **F5** (or **Ctrl+F5** to run without debugging).
   Visual Studio compiles the project and launches the login window.
5. The executable is produced at `bin\Debug\SmartMedPharmacy.exe`. The JSON data
   files are created/updated next to the executable as you use the app.

### Login details

| Role     | Username | Password   |
|----------|----------|------------|
| Admin    | `admin`  | `admin123` |
| Customer | `john`   | `john123`  |

You can also create a new customer account from the login window via
**Register as Customer**.

---

## 3. Features

### Admin
- Secure admin login.
- **Dashboard:** total sales, total medicines in stock, active orders, items expiring soon.
- **Manage Medicines:** add, update, delete (name, category, dosage, price, stock, supplier, expiry, discount, prescription flag).
- **Manage Customers:** view and update customer details.
- **Manage Orders:** view all orders, see order items, change status (Pending → Ready for Pickup → Delivered).
- **Reports:** sales report, stock report, customer order history, and export of order history to CSV (opens in Excel).

### Customer
- Register and log in.
- **Search medicines** by name, category, and price range; an optional exact-name **binary search**.
- **Cart and checkout:** add items with quantities, then place an order (stock is reduced automatically).
- **Track orders** and view their status.
- **Profile management:** update personal and contact details.

### Additional
- **Discounts/promotions:** a discount percentage per medicine; the effective price is shown to customers.
- **Expiry tracking:** medicines expiring within 30 days are flagged in the admin grid and shown to customers as a notification on login.
- **Prescription upload:** customers pick a prescription file for medicines that require one.
- **Export:** order history exports to a CSV/Excel file.

---

## 4. Project structure

```
SmartMedPharmacy/
├── Models/        Domain classes (data + behaviour)
│   ├── User.cs            abstract base class
│   ├── Admin.cs           inherits User
│   ├── Customer.cs        inherits User (adds phone, address)
│   ├── Medicine.cs        IsExpiringSoon(), EffectivePrice()
│   ├── Order.cs
│   └── OrderItem.cs
├── Data/          Persistence and search logic
│   ├── JsonStorage.cs     generic JsonStorage<T> with Load()/Save()
│   ├── DataManager.cs     loads all data, saves on change, seeds defaults
│   └── MedicineSearch.cs  linear search, filters, binary search
├── Forms/         User interface (one form per task)
│   ├── LoginForm.cs
│   ├── RegisterForm.cs
│   ├── AdminDashboardForm.cs
│   ├── ManageMedicinesForm.cs
│   ├── ManageCustomersForm.cs
│   ├── ManageOrdersForm.cs
│   ├── ReportsForm.cs
│   ├── CustomerDashboardForm.cs
│   ├── SearchMedicinesForm.cs
│   ├── CartForm.cs
│   ├── TrackOrdersForm.cs
│   └── ProfileForm.cs
├── Program.cs     application entry point
├── *.json         local data files (admins, customers, medicines, orders)
└── SmartMedPharmacy.sln / .csproj
```

---

## 5. Data storage

Data persists to JSON files saved next to the executable:

- `admins.json`
- `customers.json`
- `medicines.json`
- `orders.json`

The generic `JsonStorage<T>` class handles reading and writing each file, and
`DataManager` loads everything on startup, seeds a default admin and sample
medicines if the files are empty, and saves whenever data changes.

---

## 6. Search methods

- **Linear search** — scans medicines and matches names containing the search term.
- **Price-range filtering** — keeps medicines whose price is within a min/max range.
- **Binary search** — the "more efficient" optional method; sorts medicines by name
  and performs a binary search for an exact name match.

---

## 7. Validation and error handling

- Required fields cannot be empty; numeric fields must be numeric; price, stock and
  discount cannot be negative (discount is capped at 100%).
- File reading, writing and JSON parsing are wrapped in `try/catch`, so the app shows
  a friendly message box instead of crashing.

---

## 8. Note on AI assistance

Parts of this project were produced with the help of a generative AI assistant.
This is disclosed here in line with academic integrity requirements; the design,
understanding and explanation of the code remain the author's responsibility.
