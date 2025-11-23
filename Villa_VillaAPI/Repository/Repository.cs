using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Villa_VillaAPI.Data;
using Villa_VillaAPI.Repository.IRepository;

namespace Villa_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        // "internal" → only code inside the same project can use this (not outside projects).
        // "DbSet<T>" → represents a table in the database, but it's generic.
        // If T = Villa → dbSet = Villas table
        // If T = Category → dbSet = Categories table
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            // Save the database connection that is passed into this class
            _db = db;
            // Get the correct table from the database based on T (the generic type).
            // Example: if T = Villa → this.dbSet = _db.Villas
            //          if T = Category → this.dbSet = _db.Categories
            this.dbSet = _db.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                // Loop through each property name in the comma-separated list of 'includeProperties'
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // For each property name, add it to the Entity Framework query
                    // 'Include()' is used for eager loading — it loads related entities along with the main entity
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1)
        {
            // _db.Villas represents the DbSet for the Villa table(from your EF Core DbContext).
            // IQueryable means it’s a query that hasn’t run yet — it can be built step by step.
            // EF Core translates this query into SQL only when executed(lazy execution).
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (pageSize > 0)
            {
                // If the client requests more than 100 items, limit it to 100 to prevent very large queries.
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                // Apply pagination:
                // Skip = how many items to skip before starting the page
                // Take = how many items to return for the page
                // Example:
                // pageNumber = 2, pageSize = 10
                // Skip = 10 * (2 - 1) = 10 → skip first 10 items
                // Take = 10 → return the next 10 items
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }


            // This executes the query and fetches data from the database.
            // ToListAsync() converts the result into a List<Villa> asynchronously.
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
