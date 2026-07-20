using System;
using System.Collections.Generic;

namespace Politicore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string PoliticalOrientation { get; set; } = string.Empty;
        // Navigation property for many-to-many relation with Party
        public ICollection<Party> Parties { get; set; } = new List<Party>();

        public int Age => CalculateAge();

        private int CalculateAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (today < DateOfBirth.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
