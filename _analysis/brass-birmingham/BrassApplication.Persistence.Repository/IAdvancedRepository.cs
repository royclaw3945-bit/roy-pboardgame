using BrassApplication.Persistence.RepositoryBackends;

namespace BrassApplication.Persistence.Repository;

public interface IAdvancedRepository
{
	IGameStorage GameStorage { get; }

	ISnapshotStorage SnapshotStorage { get; }
}
