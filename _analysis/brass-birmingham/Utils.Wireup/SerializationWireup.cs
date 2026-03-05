using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.Serialization;

namespace Utils.Wireup;

public class SerializationWireup : Wireup
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(SerializationWireup));

	public SerializationWireup(Wireup inner, ISerialize serializer, string name)
		: base(inner)
	{
		Container.Register(serializer).Named(name);
	}

	public SerializationWireup Compress()
	{
		Logger.Verbose("Configuring serializer to compress the serialized payload.");
		ISerialize serialize = Container.Resolve<ISerialize>();
		Logger.Verbose("Wrapping serializer of type '{0}' in GZipSerializer.", serialize.GetType());
		Container.Register((ISerialize)new GzipSerializer(serialize));
		return this;
	}

	public SerializationWireup EncryptWith(byte[] encryptionKey)
	{
		Logger.Verbose("Configuring serializer to encrypt the serialized payload.");
		ISerialize serialize = Container.Resolve<ISerialize>();
		Logger.Verbose("Wrapping serializer of type '{0}' in RijndaelSerializer.", serialize.GetType());
		Container.Register((ISerialize)new RijndaelSerializer(serialize, encryptionKey));
		return this;
	}
}
