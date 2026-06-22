namespace SmartMedPharmacy.Models
{
    public class Customer : User
    {
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public override string GetRole()
        {
            return "Customer";
        }
    }
}
