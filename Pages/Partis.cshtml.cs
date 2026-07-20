using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task OnGetAsync()
        {
            Profiles = await _db.Users.Include(u => u.Parties).AsNoTracking().ToListAsync();
        }
    }
}
