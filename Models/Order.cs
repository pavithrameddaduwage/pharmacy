using System;
using System.Collections.Generic;

namespace SmartMedPharmacy.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal Total { get; set; }
        public string PrescriptionFilePath { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            Status = "Pending";
        }
    }
}
