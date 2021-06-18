using System.Linq;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FotoStorio.Server.Data
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Modify the queryable entity (inputQuery) using the spec's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Aggregate any Includes
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            return query;
        }
    }
}