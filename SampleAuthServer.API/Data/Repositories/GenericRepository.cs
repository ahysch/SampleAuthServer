using Microsoft.EntityFrameworkCore;
using SampleAuthServer.API.Core.Repositories;
using System.Linq.Expressions;

namespace SampleAuthServer.API.Data.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		private readonly DbContext _context;
		private readonly DbSet<TEntity> _entity;

		public GenericRepository(AppDbContext context)
		{
			_context = context;
			_entity = context.Set<TEntity>();
		}

		public async Task AddAsync(TEntity entity)
		{
			await _entity.AddAsync(entity);
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await _entity.ToListAsync();
		}

		public async Task<TEntity> GetByIdAsync(int id)
		{
			var entity = await _entity.FindAsync(id);

			if (entity != null)
			{
				_context.Entry(entity).State = EntityState.Detached;
			}

			return entity;
		}

		public void Remove(TEntity entity)
		{
			_entity.Remove(entity);
		}

		public TEntity Update(TEntity entity)
		{
			_context.Entry(entity).State = EntityState.Modified;

			return entity;
		}

		public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
		{
			return _entity.Where(predicate);
		}
	}
}