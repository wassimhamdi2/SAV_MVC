using Microsoft.AspNetCore.Identity;

namespace MiniProjet.Models
{
    public class Complaint
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public IdentityUser Customer { get; set; }
        public ICollection<Intervention> Interventions { get; set; }

        // Foreign Key
        public int ArticleId { get; set; }   // Use int? for optional foreign key
        public Article article { get; set; }
    }

}
