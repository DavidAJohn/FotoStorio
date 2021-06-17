using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FotoStorio.Server.Data
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _repositoryContext;

        public RepositoryBase(ApplicationDbContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IQueryable<T> GetAll()
        {
            return _repositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            return _repositoryContext.Set<T>()
                .Where(expression).AsNoTracking();
        }
    }
}
