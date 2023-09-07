using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace SampleAuthServer.API.Core.Services
{
	public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class
	{
		Task<Response<TDto>> GetByIdAsync(int id);

		Task<Response<IEnumerable<TDto>>> GetAllAsync();

		Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

		Task<Response<TDto>> AddAsync(TDto entity);

		Task<Response<NoContentDto>> Remove(int id);

		Task<Response<NoContentDto>> Update(TDto entity, int id);
	}
}