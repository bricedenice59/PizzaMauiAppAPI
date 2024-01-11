using System.Linq.Expressions;

namespace PizzaMauiApp.API.Core.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> WhereCriteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set;  }
    public int Take { get; private set;}
    public int Skip { get; private set;}
    public bool IsPaginationEnabled { get; private set;}

    public BaseSpecification()
    {
        
    }
    
    public BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        WhereCriteria = criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
    
    protected void AddOrderBy(Expression<Func<T, object>> orderExpression)
    {
        OrderBy = orderExpression;
    }
    
    protected void AddOrderByDescending(Expression<Func<T, object>> orderExpression)
    {
        OrderByDescending = orderExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPaginationEnabled = true;
    }
}