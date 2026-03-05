using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.Serialization;

public class BinarySerializer : ISerialize
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(BinarySerializer));

	private readonly IFormatter _formatter = new BinaryFormatter();

	public virtual void Serialize<T>(Stream output, T graph)
	{
		Logger.Verbose("Serializing type: {0}.", typeof(T));
		_formatter.Serialize(output, graph);
	}

	public virtual T Deserialize<T>(Stream input)
	{
		Logger.Verbose("Deserializing to type: {0}.", typeof(T));
		return (T)_formatter.Deserialize(input);
	}
}
