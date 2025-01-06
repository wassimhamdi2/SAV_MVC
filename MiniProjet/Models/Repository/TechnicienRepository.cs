using Microsoft.EntityFrameworkCore;

namespace MiniProjet.Models.Repository
{
    public class TechnicienRepository : IRepository<Technicien>
    {
        private readonly AppDbContext _context;

        public TechnicienRepository(AppDbContext context)
        {
            _context = context;
        }

        public Technicien Get(int Id)
        {
            return _context.Techniciens
                           .Include(t => t.Interventions) // Include related Interventions if needed
                           .FirstOrDefault(t => t.Id == Id);
        }

        public Technicien Add(Technicien t)
        {
            _context.Techniciens.Add(t);
            _context.SaveChanges();
            return t;
        }

        public Technicien Update(Technicien t)
        {
            var technicien = _context.Techniciens.Attach(t);
            technicien.State = EntityState.Modified;
            _context.SaveChanges();
            return t;
        }

        public Technicien Delete(int Id)
        {
            Technicien technicien = _context.Techniciens.Find(Id);
            if (technicien != null)
            {
                _context.Techniciens.Remove(technicien);
                _context.SaveChanges();
            }
            return technicien;
        }

        public IEnumerable<Technicien> GetAll()
        {
            var techniciens = _context.Techniciens.ToList();
            return techniciens ?? new List<Technicien>(); // Ensure it doesn't return null
        }


        public IQueryable<Technicien> GetAllWithIncludes()
        {
            // Example if more specific includes are needed
            return _context.Techniciens
                           .Include(t => t.Interventions);
        }

        public Task<IEnumerable<Technicien>> GetInterventionsByAsync()
        {
            throw new NotImplementedException();
        }
    }
}
