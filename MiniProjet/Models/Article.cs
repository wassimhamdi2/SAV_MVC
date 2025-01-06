using System.ComponentModel.DataAnnotations;

namespace MiniProjet.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Désignation { get; set; }
        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Prix { get; set; }
        [Required]
        [Display(Name = "Image :")]
        public string Image { get; set; }

        // Navigation property
        public ICollection<Complaint> Complaints { get; set; } // Navigation property
    }

}
