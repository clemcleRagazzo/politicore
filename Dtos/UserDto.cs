using System;
using System.Collections.Generic;
using Politicore.Models;

namespace Politicore.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string PoliticalOrientation { get; set; } = string.Empty;
        public List<string> Parties { get; set; } = new();
        public int Age => CalculateAge();

        public static UserDto FromEntity(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PoliticalOrientation = user.PoliticalOrientation,
                Parties = new List<string>(user.Parties)
            };
        }

        public User ToEntity()
        {
            return new User
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth,
                PoliticalOrientation = PoliticalOrientation,
                Parties = new List<string>(Parties)
            };
        }

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
