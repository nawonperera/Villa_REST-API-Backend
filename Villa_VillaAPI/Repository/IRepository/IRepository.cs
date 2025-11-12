using System.Linq.Expressions;

namespace Villa_VillaAPI.Repository.IRepository
{

    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null);

        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}

/*

    // "IRepository<T>" → a generic interface. 
    // "T" → placeholder for any class type (like Villa, Category, Product, etc.).
    // "where T : class" → constraint: T must be a reference type (a class, not int, bool, etc.).
    // In short: IRepository<T> is a reusable blueprint for repositories.
    // Example: IRepository<Villa>, IRepository<Category>, etc.

Explanation of the methods:

1. Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filter = null);
   - Asynchronously gets a list of all Villa objects from the database.
   - You can give a filter (like v => v.Rate > 100) to get only Villas that match the condition.
   - If no filter is provided (null), it returns all Villas.

2. Task<Villa> Get(Expression<Func<Villa, bool>> filter = null, bool tracked = true);
   - Asynchronously gets a single Villa object that matches the filter condition.
   - The 'tracked' parameter decides if EF Core should track changes for this object.
     - tracked = true: EF Core tracks changes (default, needed if you plan to update it later)
     - tracked = false: EF Core does not track changes (faster if you just want to read it)

3. Task Create(Villa entity);
   - Adds a new Villa to the database asynchronously.

4. Task Remove(Villa entity);
   - Deletes the given Villa from the database asynchronously.

5. Task Save();
   - Saves all pending changes to the database asynchronously (like calling SaveChangesAsync in EF Core).
*/

/*
Expression<Func<Villa, bool>> explained:

1. Func<Villa, bool>
   - A function that takes a Villa object as input and returns true or false.
   - Example: v => v.Rate > 100
     - Input: a Villa object "v"
     - Output: true if the villa's rate is greater than 100, false otherwise

2. Expression<...>
   - Wraps the function in an expression tree.
   - EF Core can read this and convert it into SQL.
   - Without Expression, filtering happens in memory (slower).
   - With Expression, EF Core translates it to a SQL WHERE clause and fetches only matching rows.

3. Combined meaning:
   - Expression<Func<Villa, bool>> is a filter function for Villas
     that EF Core can convert into SQL for efficient querying.

Example usage:
var villas = await villaRepo.GetAll(v => v.Occupancy > 4);  // gets all Villas with occupancy > 4
var villa = await villaRepo.Get(v => v.Id == 1);             // gets the Villa with Id = 1


*/
