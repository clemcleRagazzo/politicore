using Politicore.Models;

namespace Politicore.Dtos
{
    public class PartyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public static PartyDto FromEntity(Party p) => new PartyDto { Id = p.Id, Name = p.Name };

        public Party ToEntity() => new Party { Id = Id, Name = Name };
    }
}
