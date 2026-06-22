using System;
using System.Windows.Forms;
using SmartMedPharmacy.Forms;

namespace SmartMedPharmacy
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
