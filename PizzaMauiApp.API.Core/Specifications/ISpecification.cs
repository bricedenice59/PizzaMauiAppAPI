using System.Linq.Expressions;

namespace PizzaMauiApp.API.Core.Specifications;

public interface ISpecification<T>
{
    bool HasEntityTracking { get; }
    
    Expression<Func<T, bool>> WhereCriteria { get; }
    
    List<Expression<Func<T, object>>> Includes { get; }
    
    Expression<Func<T, object>> OrderBy { get; }
    
    Expression<Func<T, object>> OrderByDescending { get; }
    
    int Take  { get; }
    
    int Skip { get; }
    
    bool IsPaginationEnabled { get; }
}