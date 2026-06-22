namespace SmartMedPharmacy.Models
{
    public class Admin : User
    {
        public override string GetRole()
        {
            return "Admin";
        }
    }
}
