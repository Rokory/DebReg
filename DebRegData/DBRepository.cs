using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DebReg.Data {
    public class DBRepository<TEntity> : IRepository<TEntity> where TEntity : class {
        private DbContext context;
        private DbSet<TEntity> dbSet;

        public DBRepository(DbContext context) {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }


        #region IRepository<TEntity> Members

        public virtual IList<TEntity> GetWithRawSql(string query, params object[] parameters) {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public virtual IList<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "") {

            IQueryable<TEntity> query = dbSet;

            if (filter != null) {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            }
            else {
                return query.ToList();
            }
        }

        public virtual TEntity GetById(params object[] iD) {
            return dbSet.Find(iD);
        }

        public virtual void Insert(TEntity entity) {
            dbSet.Add(entity);
        }

        public virtual void Delete(params object[] iD) {
            TEntity entityToDelete = dbSet.Find(iD);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete) {
            if (context.Entry(entityToDelete).State == EntityState.Detached) {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate) {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        #endregion
    }
}