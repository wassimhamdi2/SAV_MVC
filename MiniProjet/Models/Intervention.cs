namespace MiniProjet.Models
{
    public class Intervention
    {
        public int Id { get; set; }
        public DateTime ScheduledAt { get; set; }
        public bool IsUnderWarranty { get; set; }
        public decimal TotalCost { get; set; }
        public int ComplaintId { get; set; }
        public Complaint? Complaint { get; set; }

        // Foreign Key
        public int TechnicienId { get; set; }
        public Technicien? Technicien { get; set; }
    }

}
