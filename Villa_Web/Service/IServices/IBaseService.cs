using Villa_Web.Models;

namespace Villa_Web.Service.IServices
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
        // 1. Task<T> (the return type)
        // This says what the method will eventually give back.
        // Example: Task<User>, Task<List<Product>>, etc.
        // But at this point, C# still doesn’t know what T is.

        // 📌 2. SendAsync<T> (the method generic definition)
        // This is where you tell the compiler:
        // “Hey, this method is generic. It has a type parameter called T, 
        // and the caller will decide what T is.”
        //
        // Without <T> here, the compiler would have no clue what T in Task<T> means.
        // It would give an error: “The type or namespace name 'T' could not be found”.

        // 📌 Why both are needed?
        // Think of it like this:
        // Task<T> is using the type parameter.
        // SendAsync<T> is declaring the type parameter.
    }
}
