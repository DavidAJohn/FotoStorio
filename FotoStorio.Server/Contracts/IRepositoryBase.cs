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
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }
}
