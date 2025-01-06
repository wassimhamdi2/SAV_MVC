using Microsoft.EntityFrameworkCore;

namespace MiniProjet.Models.Repository
{
    public class InterventionRepository : IRepository<Intervention>
    {
        private readonly AppDbContext _context;

        public InterventionRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Intervention>> GetInterventionsByComplaintIdAsync(int complaintId)
        {
            return await _context.Interventions
                .Where(i => i.ComplaintId == complaintId)
                .ToListAsync();
        }
        public Intervention Get(int Id)
        {
            return _context.Interventions.Find(Id);
        }

        public Intervention Add(Intervention t)
        {
            _context.Interventions.Add(t);
            _context.SaveChanges();
            return t;
        }

        public Intervention Update(Intervention t)
        {
            var intervention = _context.Interventions.Attach(t);
            intervention.State = EntityState.Modified;
            _context.SaveChanges();
            return t;
        }

        public Intervention Delete(int Id)
        {
            Intervention intervention = _context.Interventions.Find(Id);
            if (intervention != null)
            {
                _context.Interventions.Remove(intervention);
                _context.SaveChanges();
            }
            return intervention;
        }

        public IEnumerable<Intervention> GetAll()
        {
            return _context.Interventions.ToList(); 
        }

        public IQueryable<Intervention> GetAllWithIncludes()
        {
            return _context.Interventions.Include(i => i.Complaint); // Eagerly load related data
        }
    }
}