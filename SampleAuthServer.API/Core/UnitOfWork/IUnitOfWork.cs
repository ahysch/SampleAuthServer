namespace SampleAuthServer.API.Core.UnitOfWork
{
	public interface IUnitOfWork
	{
		Task CommitAsync();

		void Commit();
	}
}