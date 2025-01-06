using System.ComponentModel.DataAnnotations;

namespace MiniProjet.Models
{
    public class Technicien
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
        public ICollection<Intervention>? Interventions { get; set; }
    }
}
