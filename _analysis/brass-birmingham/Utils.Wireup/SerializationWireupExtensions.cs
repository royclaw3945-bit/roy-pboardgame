using BrassApplication.Persistence.Serialization;
using Frameworks.Persistence.Serialization;

namespace Utils.Wireup;

public static class SerializationWireupExtensions
{
	public static SerializationWireup UsingBinarySerialization(this GameStoragePersistenceWireup wireup)
	{
		return wireup.UsingCustomSerialization(new BinarySerializer(), "GameStorage");
	}

	public static SerializationWireup UsingJsonSerialization(this GameStoragePersistenceWireup wireup)
	{
		return wireup.UsingCustomSerialization(new JsonSerializer(), "GameStorage");
	}

	public static SerializationWireup UsingBinarySerialization(this SnapshotStoragePersistenceWireup wireup)
	{
		return wireup.UsingCustomSerialization(new BinarySerializer(), SnapshotStoragePersistenceWireup.Name);
	}

	public static SerializationWireup UsingJsonSerialization(this SnapshotStoragePersistenceWireup wireup)
	{
		return wireup.UsingCustomSerialization(new JsonSerializer(), SnapshotStoragePersistenceWireup.Name);
	}

	public static SerializationWireup UsingCustomSerialization(this Wireup wireup, ISerialize serializer, string name)
	{
		return new SerializationWireup(wireup, serializer, name);
	}
}
