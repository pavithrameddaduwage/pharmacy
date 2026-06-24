using System.Globalization;

namespace SmartMedPharmacy.Models
{
    // Central currency formatting so all screens show Sri Lankan Rupees (LKR).
    public static class Money
    {
        public static string Format(decimal amount)
        {
            return "Rs. " + amount.ToString("N2", CultureInfo.InvariantCulture);
        }
    }
}
