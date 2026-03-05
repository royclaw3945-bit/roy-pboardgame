using BrassApplication.Persistence.RepositoryBackends;

namespace Frameworks.Persistence.RepositoryBackends.GuaranteedNetworkDelivery;

public interface IJob
{
	void ProcessUsing(IGameStorage gameStorage);
}
