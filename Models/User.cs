using System;

namespace SmartMedPharmacy.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public bool CheckPassword(string input)
        {
            return Password == input;
        }

        public abstract string GetRole();
    }
}
