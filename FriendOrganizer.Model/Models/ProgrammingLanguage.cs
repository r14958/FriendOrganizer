using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FriendOrganizer.Domain.Models
{
    [DebuggerDisplay("{Id}, {Name}")]
    public class ProgrammingLanguage : EntityBase
    {
        public string Name { get; set; }

    }
}
