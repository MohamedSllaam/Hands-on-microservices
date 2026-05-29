
using Domain.Common.Specifications;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Infrastructure.Persistence.Repositories;


public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
{
    public static IQueryable<TEntity> GetQuery(
        IQueryable<TEntity> inputQuery,
        ISpecifications<TEntity> spec)
    {
        var query = inputQuery;

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);

        if (spec.OrderByDescending is not null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        // EF Core specific - This is infrastructure concern!
        query = spec.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        return query;
    }
}