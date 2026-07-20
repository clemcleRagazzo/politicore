using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Politicore.Data;
using Politicore.Dtos;

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
            var users = await _db.Users.AsNoTracking().ToListAsync();
            return users.Select(u => UserDto.FromEntity(u)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return UserDto.FromEntity(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserDto dto)
        {
            var user = dto.ToEntity();
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.Id }, UserDto.FromEntity(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(int id, UserDto dto)
        {
            var existing = await _db.Users.FindAsync(id);
            if (existing == null) return NotFound();

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.DateOfBirth = dto.DateOfBirth;
            existing.PoliticalOrientation = dto.PoliticalOrientation;
            existing.Parties = new List<string>(dto.Parties);

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
