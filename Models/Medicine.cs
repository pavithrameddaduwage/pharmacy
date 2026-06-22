using System;

namespace SmartMedPharmacy.Models
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Dosage { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Supplier { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool RequiresPrescription { get; set; }

        public bool IsExpiringSoon()
        {
            return ExpiryDate <= DateTime.Now.AddDays(30);
        }

        public decimal EffectivePrice()
        {
            if (DiscountPercent <= 0)
                return Price;
            return Math.Round(Price - (Price * DiscountPercent / 100m), 2);
        }
    }
}
