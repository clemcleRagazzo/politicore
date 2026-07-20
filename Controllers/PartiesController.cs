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
    public class PartiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PartiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartyDto>>> GetAll([FromQuery] string? q)
        {
            var query = _db.Parties.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var prefix = q.Trim();
                query = query.Where(p => p.Name.StartsWith(prefix));
            }
            var items = await query.OrderBy(p => p.Name).ToListAsync();
            return items.Select(PartyDto.FromEntity).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartyDto>> GetById(int id)
        {
            var p = await _db.Parties.FindAsync(id);
            if (p == null) return NotFound();
            return PartyDto.FromEntity(p);
        }

        [HttpPost]
        public async Task<ActionResult<PartyDto>> Create(PartyDto dto)
        {
            var exists = await _db.Parties.AnyAsync(x => x.Name == dto.Name);
            if (exists) return Conflict("Party already exists");
            var p = dto.ToEntity();
            _db.Parties.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, PartyDto.FromEntity(p));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PartyDto>> Update(int id, PartyDto dto)
        {
            var existing = await _db.Parties.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = dto.Name;
            await _db.SaveChangesAsync();
            return PartyDto.FromEntity(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Parties.FindAsync(id);
            if (existing == null) return NotFound();
            _db.Parties.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
