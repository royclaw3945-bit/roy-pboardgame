using System;
using System.Runtime.Serialization;

namespace BrassApplication.Persistence.Repository;

public abstract class RepositoryException : Exception
{
	protected RepositoryException(string message)
		: base(message)
	{
	}

	protected RepositoryException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
