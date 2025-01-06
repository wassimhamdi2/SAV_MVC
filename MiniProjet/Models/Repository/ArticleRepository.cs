using Microsoft.EntityFrameworkCore;

namespace MiniProjet.Models.Repository
{
    public class ArticleRepository : IRepository<Article>
    {
        private readonly AppDbContext _context;

        public ArticleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Article Get(int Id)
        {
            return _context.Articles.Find(Id);
        }

        public Article Add(Article t)
        {
            _context.Articles.Add(t);
            _context.SaveChanges();
            return t;
        }

        public Article Update(Article t)
        {
            var article = _context.Articles.Attach(t);
            article.State = EntityState.Modified;
            _context.SaveChanges();
            return t;
        }

        public Article Delete(int Id)
        {
            Article sparePart = _context.Articles.Find(Id);
            if (sparePart != null)
            {
                _context.Articles.Remove(sparePart);
                _context.SaveChanges();
            }
            return sparePart;
        }

        public IEnumerable<Article> GetAll()
        {
            return _context.Articles;
        }

        public IQueryable<Article> GetAllWithIncludes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Article>> GetInterventionsByAsync()
        {
            throw new NotImplementedException();
        }
    }
}
