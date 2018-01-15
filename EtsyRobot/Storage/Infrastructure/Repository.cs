using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using LinqKit;

namespace EtsyRobot.Storage.Infrastructure
{
	public class Repository<TEntity> where TEntity : class
	{
		public Repository(DbContext dbContext)
		{
			this._dbContext = dbContext;
		}

		public virtual TEntity Load(int id)
		{
			return this._dbContext.Set<TEntity>().Find(id);
		}

		public IEnumerable<TEntity> LoadPaged<TProperty>(int pageIndex, int pageCount,
			Expression<Func<TEntity, bool>> filterExpression,
			Expression<Func<TEntity, TProperty>> orderByExpression,
			bool ascending)
		{
			IQueryable<TEntity> filteredQuery = this.CreateQuery().AsExpandable().Where(filterExpression);

			IQueryable<TEntity> orderedQuery = @ascending 
				? filteredQuery.OrderBy(orderByExpression)
				: filteredQuery.OrderByDescending(orderByExpression);

			return orderedQuery.Skip(pageCount * (pageIndex - 1)).Take(pageCount).AsEnumerable();
		}

		public int GetCount(Expression<Func<TEntity, bool>> filterExpression)
		{
			IQueryable<TEntity> filteredQuery = this.CreateQuery().AsExpandable().Where(filterExpression);
			return filteredQuery.Count();
		}

		public IEnumerable<TEntity> FindMatching(Expression<Func<TEntity, bool>> filterExpression)
		{
            IQueryable<TEntity> query = this.CreateQuery().Where(filterExpression);
			return query.AsEnumerable();
		}

        public IQueryable<TEntity> FilteredQuery(Expression<Func<TEntity, bool>> filterExpression)
        {
            IQueryable<TEntity> query = this.CreateQuery().Where(filterExpression);
            return query;
        }

        public IQueryable<TEntity> FilteredQueryExpandable(Expression<Func<TEntity, bool>> filterExpression)
        {
            IQueryable<TEntity> query = this.CreateQuery().AsExpandable().Where(filterExpression);
            return query;
        }

		public bool Exists(Expression<Func<TEntity, bool>> filterExpression)
		{
			return this._dbContext.Set<TEntity>().Any(filterExpression);
		}

		public virtual void Add(TEntity entity)
		{
			this._dbContext.Set<TEntity>().Add(entity);
		}

		public virtual void Update(TEntity entity)
		{
			this._dbContext.Entry(entity).State = EntityState.Modified;
		}

		public virtual void Delete(TEntity entity)
		{
			this._dbContext.Set<TEntity>().Attach(entity);
			this._dbContext.Set<TEntity>().Remove(entity);
		}

		protected virtual IQueryable<TEntity> CreateQuery()
		{
			return this._dbContext.Set<TEntity>();
		}

		protected DbContext DbContext
		{
			get { return this._dbContext; }
		}

		private readonly DbContext _dbContext;
	}
}