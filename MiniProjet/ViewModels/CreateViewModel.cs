using System.ComponentModel.DataAnnotations;

namespace MiniProjet.ViewModels
{
    public class CreateViewModel
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
        public IFormFile ImagePath { get; set; }

    }
}
