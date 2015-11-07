using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DebReg.Data {
    public interface IRepository<T> {

        IList<T> GetWithRawSql(string query, params object[] parameters);

        IList<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        T GetById(params object[] iD);

        void Insert(T entity);
        void Delete(params object[] iD);
        void Delete(T entity);

        void Update(T entity);
    }
}
