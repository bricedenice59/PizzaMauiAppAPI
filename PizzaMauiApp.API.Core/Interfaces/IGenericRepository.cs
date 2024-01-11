using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Core.Specifications;

namespace PizzaMauiApp.API.Core.Interfaces;

public interface IGenericRepository<T> where T : BaseModel
{
    Task<T> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> ListAllAsync();

    Task<T> GetEntityWithSpecification(ISpecification<T> specification);

    Task<IReadOnlyList<T>> ListAsyncWithSpecification(ISpecification<T> specification);
    
    Task<int> CountAsync(ISpecification<T> specification);
}