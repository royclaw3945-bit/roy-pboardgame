using System.IO;
using System.IO.Compression;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Serialization;

namespace Frameworks.Persistence.Serialization;

public class GzipSerializer : ISerialize
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(GzipSerializer));

	private readonly ISerialize _inner;

	public GzipSerializer(ISerialize inner)
	{
		_inner = inner;
	}

	public virtual void Serialize<T>(Stream output, T graph)
	{
		Logger.Verbose("Serializing type: {0}.", typeof(T));
		using DeflateStream output2 = new DeflateStream(output, CompressionMode.Compress, leaveOpen: true);
		_inner.Serialize(output2, graph);
	}

	public virtual T Deserialize<T>(Stream input)
	{
		Logger.Verbose("Deserializing to type: {0}.", typeof(T));
		using DeflateStream input2 = new DeflateStream(input, CompressionMode.Decompress, leaveOpen: true);
		return _inner.Deserialize<T>(input2);
	}
}
