using FotoStorio.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FotoStorio.Server.Contracts
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
    }
}
