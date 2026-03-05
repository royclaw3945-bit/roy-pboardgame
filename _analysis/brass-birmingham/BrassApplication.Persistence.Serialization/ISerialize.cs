using System.IO;

namespace BrassApplication.Persistence.Serialization;

public interface ISerialize
{
	void Serialize<T>(Stream output, T graph);

	T Deserialize<T>(Stream input);
}
