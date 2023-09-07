using Microsoft.EntityFrameworkCore;
using SampleAuthServer.API.Core.Repositories;
using SampleAuthServer.API.Core.Services;
using SampleAuthServer.API.Core.UnitOfWork;
using SampleAuthServer.API.Service.Mapping;
using SharedLibrary.Dtos;
using System.Linq.Expressions;
using System.Net;

namespace SampleAuthServer.API.Service.Services
{
	public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<TEntity> _genericRepository;

		public GenericService(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
		{
			_unitOfWork = unitOfWork;
			_genericRepository = genericRepository;
		}

		public async Task<Response<TDto>> AddAsync(TDto entity)
		{
			var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

			await _genericRepository.AddAsync(newEntity);

			await _unitOfWork.CommitAsync();

			var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);

			return Response<TDto>.Success(newDto, StatusCodes.Status200OK);
		}

		public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
		{
			var entities = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());

			return Response<IEnumerable<TDto>>.Success(entities, StatusCodes.Status200OK);
		}

		public async Task<Response<TDto>> GetByIdAsync(int id)
		{
			var entity = await _genericRepository.GetByIdAsync(id);

			if (entity == null)
			{
				return Response<TDto>.Fail("Id not found", StatusCodes.Status404NotFound, true);
			}

			return Response<TDto>.Success(ObjectMapper.Mapper.Map<TDto>(entity), StatusCodes.Status200OK);
		}

		public async Task<Response<NoContentDto>> Remove(int id)
		{
			var entity = await _genericRepository.GetByIdAsync(id);
			if (entity == null)
			{
				return Response<NoContentDto>.Fail("Id not found", StatusCodes.Status404NotFound, true);
			}

			_genericRepository.Remove(entity);

			await _unitOfWork.CommitAsync();

			return Response<NoContentDto>.Success(StatusCodes.Status204NoContent);
		}

		public async Task<Response<NoContentDto>> Update(TDto entity, int id)
		{
			var dbEntity = await _genericRepository.GetByIdAsync(id);
			if (dbEntity == null)
			{
				return Response<NoContentDto>.Fail("Id not found", StatusCodes.Status404NotFound, true);
			}

			var updateEntity = ObjectMapper.Mapper.Map<TEntity>(dbEntity);

			_genericRepository.Update(updateEntity);

			await _unitOfWork.CommitAsync();

			return Response<NoContentDto>.Success(StatusCodes.Status204NoContent);
		}

		public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
		{
			var entities = _genericRepository.Where(predicate);

			return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await entities.ToListAsync()), StatusCodes.Status200OK);
		}
	}
}