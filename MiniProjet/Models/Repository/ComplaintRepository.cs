using Microsoft.EntityFrameworkCore;

namespace MiniProjet.Models.Repository
{
    public class ComplaintRepository : IRepository<Complaint>
    {
        private readonly AppDbContext _context;

        public ComplaintRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsWithInterventionsAsync()
        {
            return await _context.Complaints
                .Include(c => c.Interventions)
                .ToListAsync();
        }
        

        public Complaint Get(int Id)
        {
            return _context.Complaints.Find(Id);
        }

        public Complaint Add(Complaint t)
        {
            _context.Complaints.Add(t);
            _context.SaveChanges();
            return t;
        }

        public Complaint Update(Complaint t)
        {
            var com =  _context.Complaints.Attach(t);
            com.State = EntityState.Modified;
            _context.SaveChanges();
            return t;
        }

        public Complaint Delete(int Id)
        {
            Complaint P = _context.Complaints.Find(Id);
            if (P != null)
            {
                _context.Complaints.Remove(P);
                _context.SaveChanges();
            }
            return P;
        }

        public IEnumerable<Complaint> GetAll()
        {
            return _context.Complaints;
        }
        public IQueryable<Complaint> GetAllWithIncludes()
        {
            return _context.Complaints
                .Include(c => c.Customer)
                .Include(c => c.article); // Include the related Article
        }


    }
}
