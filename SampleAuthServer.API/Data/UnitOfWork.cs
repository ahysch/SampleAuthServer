﻿using Microsoft.EntityFrameworkCore;
using SampleAuthServer.API.Core.UnitOfWork;

namespace SampleAuthServer.API.Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DbContext _context;

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
		}

		public void Commit()
		{
			_context.SaveChanges();
		}

		public async Task CommitAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}