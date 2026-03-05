using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestSharp.Deserializers;

public class DotNetXmlDeserializer : IDeserializer
{
	public string DateFormat { get; set; }

	public string Namespace { get; set; }

	public string RootElement { get; set; }

	public T Deserialize<T>(IRestResponse response)
	{
		if (string.IsNullOrEmpty(response.Content))
		{
			return default(T);
		}
		using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Content));
		return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
	}
}
