using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Model.Entity;
using Villa_VillaAPI.Repository.IRepository;

namespace Villa_VillaAPI.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Villa entity)
        {
            await _db.Villas.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
        {
            IQueryable<Villa> query = _db.Villas;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter); 
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null)
        {
            // Create a query that starts with the Villas table (but doesn't run yet)
            IQueryable<Villa> query = _db.Villas;

            // If a filter condition is passed (example: "only villas with rate > 100")
            if (filter != null)
            {
                // Add that filter to the query (still not executed yet)
                query = query.Where(filter);
            }

            // Run the query against the database and return the results as a list
            return await query.ToListAsync();

        }

        public async Task RemoveAsync(Villa entity)
        {
            _db.Villas.Remove(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(Villa entity)
        {
            _db.Villas.Update(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        
    }
}
