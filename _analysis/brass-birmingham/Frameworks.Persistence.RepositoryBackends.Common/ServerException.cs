using BrassApplication.Persistence.Repository;

namespace Frameworks.Persistence.RepositoryBackends.Common;

public class ServerException : RepositoryException
{
	public ServerException(string message)
		: base(message)
	{
	}
}
