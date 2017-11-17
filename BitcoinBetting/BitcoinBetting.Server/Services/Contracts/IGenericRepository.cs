using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Create(TEntity item);

        TEntity FindById(int id);

        IEnumerable<TEntity> Get(params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        void Remove(TEntity item);

        void Update(TEntity item);
    }
}
