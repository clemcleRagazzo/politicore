using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Politicore.Data;
using Politicore.Dtos;
using Politicore.Models;

namespace Politicore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _db.Users.Include(u => u.Parties).AsNoTracking().ToListAsync();
            return users.Select(u => UserDto.FromEntity(u)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.Include(u => u.Parties).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return UserDto.FromEntity(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                PoliticalOrientation = dto.PoliticalOrientation,
                Parties = new List<Party>()
            };

            // Map party names to Party entities (create if missing)
            var names = dto.Parties?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
            foreach (var name in names)
            {
                var existing = await _db.Parties.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (existing == null)
                {
                    existing = new Party { Name = name };
                    _db.Parties.Add(existing);
                }
                user.Parties.Add(existing);
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.Id }, UserDto.FromEntity(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(int id, UserDto dto)
        {
            var existing = await _db.Users.Include(u => u.Parties).FirstOrDefaultAsync(u => u.Id == id);
            if (existing == null) return NotFound();

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.PoliticalOrientation = dto.PoliticalOrientation;

            // Synchronize parties
            var desiredNames = dto.Parties?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();

            // Remove parties not desired
            var toRemove = existing.Parties.Where(p => !desiredNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            foreach (var r in toRemove) existing.Parties.Remove(r);

            // Add missing desired parties
            foreach (var name in desiredNames)
            {
                if (!existing.Parties.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    var party = await _db.Parties.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                    if (party == null)
                    {
                        party = new Party { Name = name };
                        _db.Parties.Add(party);
                    }
                    existing.Parties.Add(party);
                }
            }

            await _db.SaveChangesAsync();
            return UserDto.FromEntity(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();
            _db.Users.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
