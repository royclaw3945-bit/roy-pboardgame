using System;
using System.IO;
using System.Text;
using BoardGameRules.Utils.Logging;
using BrassApplication.Persistence.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Frameworks.Persistence.Serialization;

public class JsonSerializer : ISerialize
{
	private static readonly ILog Logger = LogFactory.BuildLogger(typeof(JsonSerializer));

	private readonly Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer
	{
		TypeNameHandling = TypeNameHandling.All,
		DefaultValueHandling = DefaultValueHandling.Include,
		NullValueHandling = NullValueHandling.Ignore,
		Formatting = Formatting.Indented,
		ObjectCreationHandling = ObjectCreationHandling.Replace,
		MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
		Converters = { (JsonConverter)new StringEnumConverter() }
	};

	public virtual void Serialize<T>(Stream output, T graph)
	{
		Logger.Verbose("Serializing type: {0}.", typeof(T));
		using StreamWriter textWriter = new StreamWriter(output, Encoding.UTF8);
		Serialize(new JsonTextWriter(textWriter), graph);
	}

	public virtual T Deserialize<T>(Stream input)
	{
		Logger.Verbose("Deserializing to type: {0}.", typeof(T));
		using StreamReader reader = new StreamReader(input, Encoding.UTF8);
		return Deserialize<T>(new JsonTextReader(reader));
	}

	protected virtual void Serialize(JsonWriter writer, object graph)
	{
		using (writer)
		{
			serializer.Serialize(writer, graph);
		}
	}

	protected virtual T Deserialize<T>(JsonReader reader)
	{
		Type typeFromHandle = typeof(T);
		using (reader)
		{
			return (T)serializer.Deserialize(reader, typeFromHandle);
		}
	}
}
