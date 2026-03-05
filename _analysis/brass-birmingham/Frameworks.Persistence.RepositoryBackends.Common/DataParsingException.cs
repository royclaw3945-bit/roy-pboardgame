using BrassApplication.Persistence.Repository;

namespace Frameworks.Persistence.RepositoryBackends.Common;

public class DataParsingException : RepositoryException
{
	public DataParsingException(string message)
		: base(message)
	{
	}
}
