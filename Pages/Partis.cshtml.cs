using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Politicore.Data;
using Politicore.Models;

namespace Politicore.Pages
{
    public class PartisModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public PartisModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<User> Profiles { get; private set; } = new List<User>();

        [BindProperty(SupportsGet = true)]
        public int? MinAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxAge { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Orientation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PartyId { get; set; }

        public IList<Party> AllParties { get; private set; } = new List<Party>();

        public IList<string> AllOrientations { get; private set; } = new List<string>();

        public async Task OnGetAsync()
        {
            // charger listes prédéfinies
            AllParties = await _db.Parties.AsNoTracking().ToListAsync();
            AllOrientations = await _db.Users
                .Select(u => u.PoliticalOrientation)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToListAsync();

            // construire requête filtrée
            var q = _db.Users.Include(u => u.Parties).AsNoTracking().AsQueryable();

            // filtrer par âge en comparant les dates de naissance (traduisible par EF)
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (MinAge.HasValue)
            {
                // Age >= MinAge  <=> DateOfBirth <= today.AddYears(-MinAge)
                var minBirthDate = today.AddYears(-MinAge.Value);
                q = q.Where(u => u.DateOfBirth <= minBirthDate);
            }

            if (MaxAge.HasValue)
            {
                // Age <= MaxAge  <=> DateOfBirth >= today.AddYears(-MaxAge)
                var maxBirthDate = today.AddYears(-MaxAge.Value);
                q = q.Where(u => u.DateOfBirth >= maxBirthDate);
            }

            if (!string.IsNullOrWhiteSpace(Orientation))
            {
                q = q.Where(u => u.PoliticalOrientation == Orientation);
            }

            if (PartyId.HasValue)
            {
                q = q.Where(u => u.Parties.Any(p => p.Id == PartyId.Value));
            }

            Profiles = await q.ToListAsync();
        }
    }
}
